using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.JsonParser
{
    public class InputCards
    {
        public string ClassName { get; set; }
        public int SlotNumber { get; set; }
    }

    public class OutputCards
    {
        public string ClassName { get; set; }
        public int SlotNumber { get; set; }
    }
     

    public class AvSwitch
    {
        public string ClassName { get; set; }
        public string ID { get; set; }
        public string Master { get; set; }
        public InputCards[] InputCards { get; set; }
        public OutputCards[] OutputCards { get; set; }
    }

    public class AvSwitchers
    {
        public string LibraryPath { get; set; }
        public AvSwitch[] AvSwitch { get; set; }
    }
}