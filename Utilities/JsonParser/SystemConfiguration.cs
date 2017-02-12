using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.JsonParser
{
    public class SystemConfiguration
    {
        public Displays Displays { get; set; }
        public UserInterfaces UserInterfaces { get; set; }
        public AvSwitchers AvSwitchers { get; set; }
    }
}