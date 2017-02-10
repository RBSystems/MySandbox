using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.Interfaces
{
    public interface IAudioControl : IBaseDevice
    {
        bool Mute { get; set; }
        uint Level { get; set; }

        void LevelRampUp();
        void LevelRampDown();
        void LevelRampStop();
    }
}