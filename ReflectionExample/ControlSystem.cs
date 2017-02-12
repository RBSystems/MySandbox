using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.DeviceSupport;
using Utilities.Interfaces;
using Utilities.JsonParser;
using System.Collections.Generic;

namespace ReflectionExample
{
    public class ControlSystem : CrestronControlSystem
    {
        private List<IDisplay> displays;
        private Dictionary<string, BasicTriListWithSmartObject> Interfaces;
        private Thread readConfigThread;
        private Thread CreateDisplaysThread;
        private Thread CreateInterfaceThread;
        private Thread CreateAvSwitcherThread;
        private CEvent WaitForSwitchers;
        private static string CONFIG_PATH = @"\NVRAM\SystemConfiguration.json";

        /// <summary>
        /// ControlSystem Constructor. Starting point for the SIMPL#Pro program.
        /// Use the constructor to:
        /// * Initialize the maximum number of threads (max = 400)
        /// * Register devices
        /// * Register event handlers
        /// * Add Console Commands
        /// 
        /// Please be aware that the constructor needs to exit quickly; if it doesn't
        /// exit in time, the SIMPL#Pro program will exit.
        /// 
        /// You cannot send / receive data in the constructor
        /// </summary>
        public ControlSystem()
            : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(ControlSystem_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(ControlSystem_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(ControlSystem_ControllerEthernetEventHandler);

                CrestronConsole.AddNewConsoleCommand(DoWork, "dowork", "Run the currently prepared sandbox experiment.", ConsoleAccessLevelEnum.AccessOperator);

                displays = new List<IDisplay>();
                Interfaces = new Dictionary<string, BasicTriListWithSmartObject>();
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        /// <summary>
        /// InitializeSystem - this method gets called after the constructor 
        /// has finished. 
        /// 
        /// Use InitializeSystem to:
        /// * Start threads
        /// * Configure ports, such as serial and verisports
        /// * Start and initialize socket connections
        /// Send initial device configurations
        /// 
        /// Please be aware that InitializeSystem needs to exit quickly also; 
        /// if it doesn't exit in time, the SIMPL#Pro program will exit.
        /// </summary>
        public override void InitializeSystem()
        {
            try
            {
                readConfigThread = new Thread(ReadConfig, null, Thread.eThreadStartOptions.CreateSuspended);
                WaitForSwitchers = new CEvent(false, false);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        void ControlSystem_ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        void ControlSystem_ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    break;
            }

        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        void ControlSystem_ControllerSystemEventHandler(eSystemEventType systemEventType)
        {
            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    //Removable media was detected on the system
                    break;
                case (eSystemEventType.DiskRemoved):
                    //Removable media was detached from the system
                    break;
                case (eSystemEventType.Rebooting):
                    //The system is rebooting. 
                    //Very limited time to preform clean up and save any settings to disk.
                    break;
            }

        }

        private void DoWork(string args)
        {
            Cleanup();

            CrestronConsole.PrintLine("Starting work...");
            if (readConfigThread.ThreadState != Thread.eThreadStates.ThreadFinished)
                readConfigThread.Start();
            else
                readConfigThread = new Thread(ReadConfig, null, Thread.eThreadStartOptions.Running);
        }

        private void ReadAndBuildRxTx(object threadCallbackObject)
        {
            bool wasSet = WaitForSwitchers.Wait(5000);
            if (wasSet) // Build displays only if AV Switcher triggered event.
            {
                CrestronConsole.PrintLine("Building Tx and Rx devices...");

                //TODO create all receiver & transmitter objects that are in the configuration data.
                ReadAndBuildDisplays(threadCallbackObject);
            }
            else
            {
                ErrorLog.Warn("Tx/Rx devices and displays not configured -- AV Switcher configuration did not respond.");
            }
        }

        private object ReadAndBuildDisplays(object obj)
        {
            CrestronConsole.PrintLine("Building display objects...");

            // Create display objects from DLL
            SystemConfiguration configData = (SystemConfiguration)obj;
            displays = ReflectionHelper.GetIDisplays(configData);

            // Print results to console
            CrestronConsole.PrintLine("Result of display configuration build:");
            foreach (IDisplay d in displays)
            {
                CrestronConsole.PrintLine("{0} - pwr={1}; mute={2}", d.ID, d.Power, d.ShowVideo);
            }
            return null;
        }

        private object ReadAndBuildInterfaces(object obj)
        {
            CrestronConsole.PrintLine("Building User Interfaces...");
            SystemConfiguration config = (SystemConfiguration)obj;

            Interfaces = ReflectionHelper.GetUserInterfaces(config, this);

            foreach (var kvp in Interfaces)
            {
                if (Interfaces[kvp.Key].Register() == eDeviceRegistrationUnRegistrationResponse.Success)
                {
                    Interfaces[kvp.Key].OnlineStatusChange += InterfacesOnlineStatusChange;
                    Interfaces[kvp.Key].SigChange += InterfaceSigChange;
                    CrestronConsole.PrintLine("{0} - {1}", kvp.Key, kvp.Value.GetType().FullName);
                }
                else
                {
                    ErrorLog.Error("Unable to register {0} at ID {1}.", Interfaces[kvp.Key], Interfaces[kvp.Key].ID);
                }
            }
            return null;
        }

        private object ReadAndBuildAvSwitchers(object obj)
        {
            //TODO Call JSON helper and get all AV Switcher configurations
            //TODO Create thread for building all transmitters and receiver devices, then connect them to the AV Switch
            Thread.Sleep(2000);
            WaitForSwitchers.Set();
            return null;
        }

        private void InterfaceSigChange(BasicTriList currentDevice, SigEventArgs args)
        {
            switch (args.Sig.Type)
            {
                case eSigType.Bool:
                    if (args.Sig.BoolValue)
                    {
                        // Print results to console
                        CrestronConsole.PrintLine("Result of display configuration build:");
                        foreach (IDisplay d in displays)
                        {
                            CrestronConsole.PrintLine("{0} - pwr={1}; mute={2}", d.ID, d.Power, d.ShowVideo);
                        }
                    }
                    break;
                case eSigType.String:
                    break;
                case eSigType.UShort:
                    break;
                case eSigType.NA:
                default:
                    break;
            }
        }

        private void InterfacesOnlineStatusChange(GenericBase currentDevice, OnlineOfflineEventArgs args)
        {
            CrestronConsole.PrintLine("Online status changed for {0}: {1}", currentDevice.ID, args.DeviceOnLine);
            if (args.DeviceOnLine)
            {
                ErrorLog.Notice("{0} has come online.", currentDevice.ID);
            }
        }

        private object ReadConfig(object callbackObject)
        {
            CrestronConsole.PrintLine("Reading configuration file...");
            string configJson = string.Empty;
            SystemConfiguration config = null;
            try
            {
                // Read config file and create objects from serialized data
                configJson = JsonHelper.ReadJsonData(CONFIG_PATH);
                config = JsonHelper.ParseConfigJson(configJson);
            }
            catch (Exception e)
            {
                ErrorLog.Error("CONFIGURATION READ ERROR: {0}", e);
            }
            CreateDevices(config);
            return config;
        }

        private void CreateDevices(SystemConfiguration config)
        {
            CreateDisplaysThread = new Thread(ReadAndBuildDisplays, config, Thread.eThreadStartOptions.Running);
            CreateInterfaceThread = new Thread(ReadAndBuildInterfaces, config, Thread.eThreadStartOptions.Running);

            //TODO Have Display & Endpoint/Transmitter configuration wait for swicher to be built (Use CEvent)
            CreateAvSwitcherThread = new Thread(ReadAndBuildAvSwitchers, config, Thread.eThreadStartOptions.Running);
        }

        private void Cleanup()
        {
            foreach (IDisplay d in displays)
            {
                //TODO dispose of display
            }

            foreach (var ui in Interfaces)
            {
                ui.Value.Dispose();
            }

            displays.Clear();
            Interfaces.Clear();
        }
    }
}