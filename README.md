# VideoTimecode

`VideoTimecode` is library for [SMPTE timecodes](https://en.wikipedia.org/wiki/SMPTE_timecode) calculation. It is originally forked from [Timecode4net](https://github.com/ailen0ada/Timecode4net).

## Features

- supports 23.98, 24, 25, 29.97, 30, 48, 50, 59.94, 60 fps
- supports drop-frame and non-drop-frame codes
- instantiate timecodes from frame count, string time code
- (WIP)timecode arithmetics: adding frame counts and other timecodes

## Usage

```cs
var fromFrames = new Timecode(input, FrameRate.FrameRate23_976);
Console.WriteLine(fromFrames.ToString()); // 00:01:00;02
```

## Credits

[ailen0ada/Timecode4net](https://github.com/ailen0ada/Timecode4net)
[smpte-timecode](https://github.com/CrystalComputerCorp/smpte-timecode)
[Drop-Frame Timecode](http://www.davidheidelberger.com/2010/06/10/drop-frame-timecode/)
[Time Codes: The Amazing Truth](http://www.andrewduncan.net/timecodes/)
