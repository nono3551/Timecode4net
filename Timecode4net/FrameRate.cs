using System;
using Xunit.Abstractions;

// ReSharper disable InconsistentNaming

namespace VideoTimecode
{
    public class FrameRate : IXunitSerializable
    {
        public static readonly FrameRate FrameRate23_976 = new() { Rate = 23.976, Name = "23.976" };
        public static readonly FrameRate FrameRate24 = new() { Rate = 24, Name = "24" };
        public static readonly FrameRate FrameRate25 = new() { Rate = 25, Name = "25" };
        public static readonly FrameRate FrameRate29_97_DF = new() { Rate = 29.97, Name = "29.97 DF", DropFramesCount = 2 };
        public static readonly FrameRate FrameRate29_97_NDF = new() { Rate = 29.97, Name = "29.97 NDF" };
        public static readonly FrameRate FrameRate30 = new() { Rate = 30, Name = "30" };
        public static readonly FrameRate FrameRate48 = new() { Rate = 48, Name = "48" };
        public static readonly FrameRate FrameRate50 = new() { Rate = 50, Name = "50" };
        public static readonly FrameRate FrameRate59_94_DF = new() { Rate = 59.94, Name = "59.94 DF", DropFramesCount = 4 };
        public static readonly FrameRate FrameRate59_94_NDF = new() { Rate = 59.94, Name = "59.94 NDF" };
        public static readonly FrameRate FrameRate60 = new() { Rate = 60, Name = "60" };

        public double Rate { get; set; }
        public int RateRounded => (int)Math.Ceiling(Rate);
        public string Name { get; set; }
        public int DropFramesCount { get; set; }

        private FrameRate()
        {

        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            Rate = info.GetValue<double>("Rate");
            DropFramesCount = info.GetValue<int>("DropFramesCount");
            Name = info.GetValue<string>("Name");
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("Rate", Rate);
            info.AddValue("DropFramesCount", DropFramesCount);
            info.AddValue("Name", Name);
        }
    }
}
