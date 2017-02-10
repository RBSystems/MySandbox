using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace Utilities.JsonParser
{
    public class Interface
    {
        public string ClassName { get; set; }
        public string ID { get; set; }
        public int IpId { get; set; }
    }

    public class UserInterfaces
    {
        public string LibraryPath { get; set; }
        public List<Interface> Interface { get; set; }
    }
}