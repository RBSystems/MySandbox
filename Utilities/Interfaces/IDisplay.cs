using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.Interfaces
{
    public interface IDisplay : IBaseDevice
    {
        bool ShowVideo { get; set; }
        bool Power { get; set; }
    }
}