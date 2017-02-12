using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;
using Utilities.Interfaces;
using Utilities.JsonParser;
using Crestron.SimplSharpPro.DeviceSupport;
using Crestron.SimplSharpPro;

namespace ReflectionExample
{
    /// <summary>
    /// Helper class used to parse JSON objects for hardware configuration.
    /// Uses Reflection to obtain base hardware classes.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Parse JSON data for all displays used in the system.
        /// </summary>
        /// <param name="ConfigObject">a data object containing all system information</param>
        /// <returns>a list of all displays in the configure. Logs an error to the processor if opening DLL and getting data fails.</returns>
        public static List<IDisplay> GetIDisplays(SystemConfiguration ConfigObject)
        {
            List<IDisplay> displays = new List<IDisplay>();
            try
            {
                Assembly da = Assembly.LoadFrom(ConfigObject.Displays.LibraryPath);
                foreach (Display d in ConfigObject.Displays.Display)
                {
                    CType dt = da.GetType(d.ClassName);
                    ConstructorInfo ci = dt.GetConstructor(new CType[] { });
                    object displayObj = ci.Invoke(new object[] { });

                    IDisplay disp = (IDisplay)displayObj;
                    disp.Initialize(d.ID);
                    displays.Add(disp);
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Failed to created IDisplay objects: {0}", e);
            }
            return displays;
        }

        /// <summary>
        /// Creates a dictionary of touchscreen, xpanel, and other user interfaces used in a system based on a configuration object.
        /// </summary>
        /// <param name="configObject">the configuration data parsed from the config file</param>
        /// <param name="master">the control processor that the interfaces will connect to.</param>
        /// <returns>A dictionary containing all of the found interfaces. They will be assigned to 'master' but not registered.</returns>
        public static Dictionary<string, BasicTriListWithSmartObject> GetUserInterfaces(SystemConfiguration configObject, CrestronControlSystem master)
        {
            Dictionary<string, BasicTriListWithSmartObject> interfaces = new Dictionary<string, BasicTriListWithSmartObject>();

            try
            {
                Assembly uiAssembly = Assembly.LoadFrom(configObject.UserInterfaces.LibraryPath);
                foreach (Interface ui in configObject.UserInterfaces.Interface)
                {
                    CType tp = uiAssembly.GetType(ui.ClassName);
                    ConstructorInfo ci = tp.GetConstructor(new CType[] { typeof(UInt32), typeof(CrestronControlSystem) });
                    object uiObject = ci.Invoke(new object[] { Convert.ToUInt32(ui.IpId), master });

                    BasicTriListWithSmartObject uiCasted = (BasicTriListWithSmartObject)uiObject;
                    interfaces.Add(ui.ID, uiCasted);
                }
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Failed to create Interface objects: {0}", e);
            }
            return interfaces;
        }

        public static void GetAvSwitch()
        {
            //TODO Make this method build a switcher and return it to the calling object.
        }

        public static void GetRxTxDevices()
        {
            //TODO Make this method build all TX and RX objects and return them to the caller.
        }
    }
}