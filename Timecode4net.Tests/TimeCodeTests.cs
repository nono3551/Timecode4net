using Xunit;

namespace VideoTimecode.Tests
{
    public class TimecodeTests
    {
        [Theory, MemberData(nameof(TestData))]
        public void CreateByFrameTest(int frames, FrameRate frameRate, string expected)
        {
            var actual = new Timecode(frames, frameRate);
            Assert.Equal(expected, actual.ToString());
        }

        [Theory, MemberData(nameof(TestData))]
        public void CreateByStringTest(int expected, FrameRate frameRate, string input)
        {
            var actual = new Timecode(input, frameRate);
            Assert.Equal(expected, actual.TotalFrames);
        }

        [Theory, MemberData(nameof(TestData))]
        public void CreateByTimeSpanTest(int expected, FrameRate frameRate, string input)
        {
            var attempt = new Timecode(input, frameRate);
            var actual = new Timecode(attempt.ToTimeSpan(), frameRate);
            Assert.Equal(expected, actual.TotalFrames);
        }

        public static object[][] TestData =
        {
            // 23.975
            new object[] {0, FrameRate.FrameRate23_976, "00:00:00:00"},
            new object[] {1, FrameRate.FrameRate23_976, "00:00:00:01"},
            new object[] {2, FrameRate.FrameRate23_976, "00:00:00:02"},
            new object[] {22, FrameRate.FrameRate23_976, "00:00:00:22"},
            new object[] {23, FrameRate.FrameRate23_976, "00:00:00:23"},
            new object[] {24, FrameRate.FrameRate23_976, "00:00:01:00"},
            new object[] {25, FrameRate.FrameRate23_976, "00:00:01:01"},
            new object[] {46, FrameRate.FrameRate23_976, "00:00:01:22"},
            new object[] {47, FrameRate.FrameRate23_976, "00:00:01:23"},
            new object[] {48, FrameRate.FrameRate23_976, "00:00:02:00"},
            new object[] {1442, FrameRate.FrameRate23_976, "00:01:00:02"},
            new object[] {216271, FrameRate.FrameRate23_976, "02:30:11:07"},
            new object[] {1802, FrameRate.FrameRate23_976, "00:01:15:02"},

            // 24
            new object[] {0, FrameRate.FrameRate24, "00:00:00:00"},
            new object[] {1, FrameRate.FrameRate24, "00:00:00:01"},
            new object[] {2, FrameRate.FrameRate24, "00:00:00:02"},
            new object[] {22, FrameRate.FrameRate24, "00:00:00:22"},
            new object[] {23, FrameRate.FrameRate24, "00:00:00:23"},
            new object[] {24, FrameRate.FrameRate24, "00:00:01:00"},
            new object[] {25, FrameRate.FrameRate24, "00:00:01:01"},
            new object[] {26, FrameRate.FrameRate24, "00:00:01:02"},
            new object[] {1800, FrameRate.FrameRate24, "00:01:15:00"},

            // 25
            new object[] {0, FrameRate.FrameRate25, "00:00:00:00"},
            new object[] {1, FrameRate.FrameRate25, "00:00:00:01"},
            new object[] {2, FrameRate.FrameRate25, "00:00:00:02"},
            new object[] {22, FrameRate.FrameRate25, "00:00:00:22"},
            new object[] {23, FrameRate.FrameRate25, "00:00:00:23"},
            new object[] {24, FrameRate.FrameRate25, "00:00:00:24"},
            new object[] {25, FrameRate.FrameRate25, "00:00:01:00"},
            new object[] {26, FrameRate.FrameRate25, "00:00:01:01"},
            new object[] {15023, FrameRate.FrameRate25, "00:10:00:23"},
            new object[] {1800, FrameRate.FrameRate25, "00:01:12:00"},

            // 29.97 DF
            new object[] {3596, FrameRate.FrameRate29_97_DF, "00:01:59;28"},
            new object[] {3597, FrameRate.FrameRate29_97_DF, "00:01:59;29"},
            new object[] {3598, FrameRate.FrameRate29_97_DF, "00:02:00;02"},
            new object[] {3599, FrameRate.FrameRate29_97_DF, "00:02:00;03"},
            new object[] {3600, FrameRate.FrameRate29_97_DF, "00:02:00;04"},
            new object[] {3601, FrameRate.FrameRate29_97_DF, "00:02:00;05"},
            new object[] {3625, FrameRate.FrameRate29_97_DF, "00:02:00;29"},
            new object[] {3626, FrameRate.FrameRate29_97_DF, "00:02:01;00"},
            new object[] {3627, FrameRate.FrameRate29_97_DF, "00:02:01;01"},

            // 29.97 NDF
            new object[] {215999, FrameRate.FrameRate29_97_NDF, "01:59:59:29"},
            new object[] {216000, FrameRate.FrameRate29_97_NDF, "02:00:00:00"},
            new object[] {216001, FrameRate.FrameRate29_97_NDF, "02:00:00:01"},
            new object[] {216002, FrameRate.FrameRate29_97_NDF, "02:00:00:02"},
            new object[] {216003, FrameRate.FrameRate29_97_NDF, "02:00:00:03"},
            new object[] {216029, FrameRate.FrameRate29_97_NDF, "02:00:00:29"},
            new object[] {216030, FrameRate.FrameRate29_97_NDF, "02:00:01:00"},
            new object[] {1387252, FrameRate.FrameRate29_97_NDF, "12:50:41:22"},

            // 30
            new object[] {1799, FrameRate.FrameRate30, "00:00:59:29"},
            new object[] {1800, FrameRate.FrameRate30, "00:01:00:00"},
            new object[] {1801, FrameRate.FrameRate30, "00:01:00:01"},
            new object[] {1829, FrameRate.FrameRate30, "00:01:00:29"},
            new object[] {1830, FrameRate.FrameRate30, "00:01:01:00"},
            new object[] {1831, FrameRate.FrameRate30, "00:01:01:01"},

            // 48
            new object[] {0, FrameRate.FrameRate48, "00:00:00:00"},
            new object[] {1, FrameRate.FrameRate48, "00:00:00:01"},
            new object[] {46, FrameRate.FrameRate48, "00:00:00:46"},
            new object[] {47, FrameRate.FrameRate48, "00:00:00:47"},
            new object[] {48, FrameRate.FrameRate48, "00:00:01:00"},
            new object[] {49, FrameRate.FrameRate48, "00:00:01:01"},

            // 50
            new object[] {1800, FrameRate.FrameRate50, "00:00:36:00"},
            new object[] {1801, FrameRate.FrameRate50, "00:00:36:01"},
            new object[] {1849, FrameRate.FrameRate50, "00:00:36:49"},
            new object[] {1850, FrameRate.FrameRate50, "00:00:37:00"},
            new object[] {1851, FrameRate.FrameRate50, "00:00:37:01"},

            // 59.94 DF
            new object[] {3595, FrameRate.FrameRate59_94_DF, "00:00:59;55"},
            new object[] {3596, FrameRate.FrameRate59_94_DF, "00:00:59;56"},
            new object[] {3597, FrameRate.FrameRate59_94_DF, "00:00:59;57"},
            new object[] {3598, FrameRate.FrameRate59_94_DF, "00:00:59;58"},
            new object[] {3599, FrameRate.FrameRate59_94_DF, "00:00:59;59"},
            new object[] {3600, FrameRate.FrameRate59_94_DF, "00:01:00;04"},
            new object[] {3601, FrameRate.FrameRate59_94_DF, "00:01:00;05"},
            new object[] {3625, FrameRate.FrameRate59_94_DF, "00:01:00;29"},
            new object[] {216003, FrameRate.FrameRate59_94_DF, "01:00:03;39"},

            // 59.94 NDF
            new object[] {0, FrameRate.FrameRate59_94_NDF, "00:00:00:00"},
            new object[] {1, FrameRate.FrameRate59_94_NDF, "00:00:00:01"},
            new object[] {57, FrameRate.FrameRate59_94_NDF, "00:00:00:57"},
            new object[] {58, FrameRate.FrameRate59_94_NDF, "00:00:00:58"},
            new object[] {59, FrameRate.FrameRate59_94_NDF, "00:00:00:59"},
            new object[] {60, FrameRate.FrameRate59_94_NDF, "00:00:01:00"},
            new object[] {61, FrameRate.FrameRate59_94_NDF, "00:00:01:01"},
            new object[] {62, FrameRate.FrameRate59_94_NDF, "00:00:01:02"},

            // 60
            new object[] {1799, FrameRate.FrameRate60, "00:00:29:59"},
            new object[] {1800, FrameRate.FrameRate60, "00:00:30:00"},
            new object[] {1801, FrameRate.FrameRate60, "00:00:30:01"},
        };
    }
}
