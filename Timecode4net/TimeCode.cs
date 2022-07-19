using System;
using System.Text.RegularExpressions;

namespace Timecode4net
{
    public class Timecode
    {
        private const int SecondsInHour = 3600;
        private const int SecondsInMinute = 60;
        private const int MillisecondsInSecond = 1000;
        private const int HoursInDay = 24;
        private const double Imposter = 0.066666f;

        private const string TimeCodePattern = @"^(?<hours>[0-2][0-9]):(?<minutes>[0-5][0-9]):(?<seconds>[0-5][0-9])[:|;|\.](?<frames>[0-9]{2,3})$";

        public FrameRate FrameRate { get; }
        private bool IsDropFrameRate => FrameRate.DropFramesCount != 0;
        public int TotalFrames { get; private set; }
        public int Hours { get; private set; }
        public int Minutes { get; private set; }
        public int Seconds { get; private set; }
        public int Frames { get; private set; }


        private void UpdateByTotalFrames()
        {
            var frameCount = TotalFrames;
            if (IsDropFrameRate)
            {
                var fps = FrameRate.Rate;
                var dropFrames = Math.Round(fps * Imposter, MidpointRounding.AwayFromZero);
                var framesPerHour = Math.Round(fps * SecondsInHour, MidpointRounding.AwayFromZero);
                var framesPer24H = framesPerHour * HoursInDay;
                var framesPer10M = Math.Round(fps * SecondsInMinute * 10, MidpointRounding.AwayFromZero);
                var framesPerMin = Math.Round(fps * SecondsInMinute, MidpointRounding.AwayFromZero);

                frameCount %= (int)framesPer24H;
                if (frameCount < 0)
                {
                    frameCount = (int)(framesPer24H + frameCount);
                }

                var d = Math.Floor(frameCount / framesPer10M);
                var m = frameCount % framesPer10M;
                if (m > dropFrames)
                {
                    frameCount += (int)(dropFrames * 9 * d + dropFrames * Math.Floor((m - dropFrames) / framesPerMin));
                }
                else
                {
                    frameCount += (int)(dropFrames * 9 * d);
                }

                Hours = (int)Math.Floor(Math.Floor(Math.Floor(frameCount / (double)FrameRate.RateRounded) / SecondsInMinute) / SecondsInMinute);
                Minutes = (int)Math.Floor(Math.Floor(frameCount / (double)FrameRate.RateRounded) / SecondsInMinute) % SecondsInMinute;
                Seconds = (int)Math.Floor(frameCount / (double)FrameRate.RateRounded) % SecondsInMinute;
                Frames = frameCount % FrameRate.RateRounded;
            }
            else
            {
                Hours = frameCount / (SecondsInHour * FrameRate.RateRounded);
                if (Hours > 23)
                {
                    Hours %= 24;
                    frameCount -= 23 * SecondsInHour * FrameRate.RateRounded;
                }
                Minutes = frameCount % (SecondsInHour * FrameRate.RateRounded) / (SecondsInMinute * FrameRate.RateRounded);
                Seconds = frameCount % (SecondsInHour * FrameRate.RateRounded) % (SecondsInMinute * FrameRate.RateRounded) / FrameRate.RateRounded;
                Frames = frameCount % (SecondsInHour * FrameRate.RateRounded) % (SecondsInMinute * FrameRate.RateRounded) % FrameRate.RateRounded;
            }
        }

        public Timecode(int totalFrames, FrameRate frameRate)
        {
            FrameRate = frameRate;

            TotalFrames = totalFrames;

            var timespan = TimeSpan.FromMilliseconds(TotalFrames * MillisecondsInSecond / FrameRate.Rate);
            Hours = timespan.Hours;
            Minutes = timespan.Minutes;
            Seconds = timespan.Seconds;

            UpdateByTotalFrames();
        }

        public Timecode(string timecode, FrameRate frameRate)
        {
            FrameRate = frameRate;

            if (string.IsNullOrEmpty(timecode))
            {
                throw new ArgumentNullException(nameof(timecode));
            }

            var tcRegex = new Regex(TimeCodePattern);
            var match = tcRegex.Match(timecode);
            if (!match.Success)
            {
                throw new ArgumentException("Input text was not in valid timecode format.", nameof(timecode));
            }

            Hours = int.Parse(match.Groups["hours"].Value);
            Minutes = int.Parse(match.Groups["minutes"].Value);
            Seconds = int.Parse(match.Groups["seconds"].Value);
            Frames = int.Parse(match.Groups["frames"].Value);

            CalculateTotalFrames();
        }

        public Timecode(TimeSpan timespan, FrameRate frameRate)
            : this((int)Math.Floor((timespan.TotalSeconds * frameRate.RateRounded) + (0.01 * frameRate.Rate)), frameRate)
        {

        }

        public override string ToString()
        {
            var frameSeparator = IsDropFrameRate ? ";" : ":";
            return $"{Hours:D2}:{Minutes:D2}:{Seconds:D2}{frameSeparator}{Frames:D2}";
        }

        public TimeSpan ToTimeSpan()
        {
            var framesInMilliseconds = TotalFrames * MillisecondsInSecond / FrameRate.RateRounded;
            return TimeSpan.FromMilliseconds(framesInMilliseconds);
        }

        private void CalculateTotalFrames()
        {
            double frames = Hours * SecondsInHour;
            frames += Minutes * SecondsInMinute;
            frames += Seconds;
            frames *= FrameRate.RateRounded;
            frames += Frames;

            if (IsDropFrameRate)
            {
                var totalMinutes = Hours * 60 + Minutes;
                totalMinutes -= totalMinutes / 10;
                var dropFrames = totalMinutes * FrameRate.DropFramesCount;
                frames -= dropFrames;
            }

            TotalFrames = (int)Math.Floor(frames);
        }
    }
}
