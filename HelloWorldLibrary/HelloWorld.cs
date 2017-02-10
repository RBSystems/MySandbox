using System;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       				// For Basic SIMPL#Pro classes

namespace HelloWorldLibrary
{
    public class HelloWorld
    {
        public bool Enabled { get; set; }

        public void PrintMessage()
        {
            if (Enabled)
                CrestronConsole.PrintLine("Hello, world!");
            else
                CrestronConsole.PrintLine("Hello World example not enabled.");
        }

        public void PrintCustomMessage(string msg)
        {
            if (Enabled)
                CrestronConsole.PrintLine(msg);
            else
                CrestronConsole.PrintLine("Hello World example not enabled.");
            
        }
    }
}

