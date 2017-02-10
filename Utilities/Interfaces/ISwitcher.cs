using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.Interfaces
{
    public interface ISwitcher
    {
        uint NumberOfInputs { get; }
        uint NumberOfOutputs { get; }

        void Route(uint input, uint output);
        void ClearAllRoutes();
        uint CurrentRoutedSource(uint output);
    }
}