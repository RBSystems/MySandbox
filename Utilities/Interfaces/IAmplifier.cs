using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.Interfaces
{
    public interface IAmplifier
    {
        bool Power { get; set; }
        string Id { get; set; }
    }
}