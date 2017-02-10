using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.JsonParser
{
    public class Display
    {
        public string ClassName { get; set; }
        public string ID { get; set; }
        public int ReceiverId { get; set; }
    }

    public class Displays
    {
        public string LibraryPath { get; set; }
        public List<Display> Display { get; set; }
    }
}