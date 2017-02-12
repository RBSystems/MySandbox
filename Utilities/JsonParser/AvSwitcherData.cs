using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.JsonParser
{
    public class InputCard
    {
        public string ClassName { get; set; }
        public int SlotNumber { get; set; }
    }

    public class OutputCard
    {
        public string ClassName { get; set; }
        public int SlotNumber { get; set; }
    }

    public class AvSwitch
    {
        public string ClassName { get; set; }
        public string ID { get; set; }
        public int IpId { get; set; }
        public List<InputCard> InputCards { get; set; }
        public List<OutputCard> OutputCards { get; set; }
    }

    public class AvSwitchers
    {
        public string LibraryPath { get; set; }
        public List<AvSwitch> AvSwitch { get; set; }
    }
}