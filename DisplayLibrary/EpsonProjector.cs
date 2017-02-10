using System;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       				// For Basic SIMPL#Pro classes
using Utilities.Interfaces;

namespace DisplayLibrary
{
    public class EpsonProjector : IDisplay, ISwitcher, IAudioControl
    {
        public bool ShowVideo { get; set; }
        public bool Power { get; set; }
        public bool Mute { get; set; }
        public uint NumberOfInputs { get; private set; }
        public uint NumberOfOutputs {get; private set;}
        public uint Level { get; set; }
        public string ID { get; private set; }
        public string Manufacturer { get; private set; }
        public string Model { get; private set; }

        private uint[] outputs;
        private CTimer rampTimer;
        private bool isRamping;

        public bool Initialize(string id)
        {
            bool result = false;
            try
            {
                NumberOfInputs = 1;
                outputs = new uint[1];
                ID = id;
                result = true;
                Manufacturer = "Epson";
                Model = "Generic Epson Driver";
            }
            catch (Exception e)
            {
                ErrorLog.Error("Epson Projector INIT failure: {0}", e);
            }
            return result;
        }

        public void Route(uint input, uint output)
        {
            if (input <= NumberOfInputs && output <= NumberOfOutputs)
            {
                outputs[output] = input;
            }
        }

        public void ClearAllRoutes()
        {
            for (ushort i = 0; i < outputs.Length; i++)
            {
                outputs[i] = 0;
            }
        }

        public uint CurrentRoutedSource(uint output)
        {
            if (output <= NumberOfOutputs)
            {
                return outputs[output];
            }
            else
            {
                return 0;
            }
        }

        public void LevelRampUp()
        {
            if (!isRamping)
            {
                isRamping = true;
                // start incrementing level by 5 ever 3 seconds
                if (rampTimer == null)
                {
                    rampTimer = new CTimer(ramperCallback, 1, 3000);
                }
                else
                {
                    rampTimer.Reset();
                }
            }
        }

        public void LevelRampDown()
        {
            if (!isRamping)
            {
                isRamping = true;
                // start incrementing level by 5 ever 3 seconds
                if (rampTimer == null)
                {
                    rampTimer = new CTimer(ramperCallback, 0, 3000);
                }
                else
                {
                    rampTimer.Reset();
                }
            }
        }

        public void LevelRampStop()
        {
            if (rampTimer != null)
            {
                rampTimer.Stop();
                rampTimer.Dispose();
            }
            isRamping = false;
        }

        private void ramperCallback(Object obj)
        {
            if ((uint)obj == 1) // Ramp up
            {
                Level = (Level < 100) ? Level += 10 : 100;
            }
            else // Ramp down
            {
                Level = (Level > 10) ? Level - 10 : 0;
            }
        }
    }
}

