using System;
using System.Text.RegularExpressions;

namespace Timecode4net
{
    public class Timecode
    {
        private const int SecondsInHour = 3600;
        private const int SecondsInMinute = 60;
        private const int MillisecondsInSecond = 1000;

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
            const int secondsInHour = 3600;
            const int secondsInMinutes = 60;

            var frameCount = this.TotalFrames;
            if (IsDropFrameRate)
            {
                var fps = this.FrameRate.Rate;
                var dropFrames = Math.Round(fps * 0.066666, MidpointRounding.AwayFromZero);
                var framesPerHour = Math.Round(fps * secondsInHour, MidpointRounding.AwayFromZero);
                var framesPer24H = framesPerHour * 24;
                var framesPer10M = Math.Round(fps * secondsInMinutes * 10, MidpointRounding.AwayFromZero);
                var framesPerMin = Math.Round(fps * secondsInMinutes, MidpointRounding.AwayFromZero);

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

                this.Hours = (int)Math.Floor(Math.Floor(Math.Floor(frameCount / (double)FrameRate.RateRounded) / secondsInMinutes) / secondsInMinutes);
                this.Minutes = (int)Math.Floor(Math.Floor(frameCount / (double)FrameRate.RateRounded) / secondsInMinutes) % secondsInMinutes;
                this.Seconds = (int)Math.Floor(frameCount / (double)FrameRate.RateRounded) % secondsInMinutes;
                this.Frames = (int) (frameCount % FrameRate.RateRounded);
            }
            else
            {
                this.Hours = (int) (frameCount / (secondsInHour * FrameRate.RateRounded));
                if (this.Hours > 23)
                {
                    this.Hours %= 24;
                    frameCount -= (int) (23 * secondsInHour * FrameRate.RateRounded);
                }
                this.Minutes = (int) (frameCount % (secondsInHour * FrameRate.RateRounded) / (secondsInMinutes * FrameRate.RateRounded));
                this.Seconds = (int) (frameCount % (secondsInHour * FrameRate.RateRounded) % (secondsInMinutes * FrameRate.RateRounded) / FrameRate.RateRounded);
                this.Frames = (int) (frameCount % (secondsInHour * FrameRate.RateRounded) % (secondsInMinutes * FrameRate.RateRounded) % FrameRate.RateRounded);
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
            : this((int)Math.Ceiling(timespan.TotalSeconds * frameRate.RateRounded), frameRate)
        {

        }

        public override string ToString()
        {
            var frameSeparator = IsDropFrameRate ? ";" : ":";
            return $"{this.Hours:D2}:{this.Minutes:D2}:{this.Seconds:D2}{frameSeparator}{this.Frames:D2}";
        }

        public TimeSpan ToTimeSpan()
        {
            var framesInMilliseconds = this.TotalFrames * MillisecondsInSecond / this.FrameRate.RateRounded;
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

            TotalFrames = (int) Math.Floor(frames);
        }
    }
}
