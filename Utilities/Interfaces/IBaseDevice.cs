using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.Interfaces
{
    public interface IBaseDevice
    {
        string ID { get; }
        string Manufacturer { get; }
        string Model { get; }

        bool Initialize(string id);
    }
}