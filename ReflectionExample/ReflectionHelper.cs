using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.Reflection;
using Utilities.Interfaces;

namespace ReflectionExample
{
    public class ReflectionHelper
    {
        private Assembly targetAssembly;    // Used to dynamically load a DLL
        private Object refObject;           // The Reflected class will be loaded to this reference
        private CType refType;              // Used to gather the refObject type and make calling fields/methods possible

        public ReflectionHelper() {}

        public void DisplayAllHelloWorldClasses()
        {
            targetAssembly = Assembly.LoadFrom(@"\NVRAM\HelloWorldLibrary.dll"); // Load DLL file into memory
            if (targetAssembly.FullName.ToLower().Contains("helloworldlibrary"))
            {
                refObject = targetAssembly.CreateInstance("HelloWorldLibrary.HelloWorld"); // Creat a HelloWorld object
                refType = refObject.GetType();

                // Print out all class data to the console.
                foreach (CType type in targetAssembly.GetTypes())
                {
                    CrestronConsole.PrintLine("Type of obj: {0}", type.FullName);

                    // Print out all constructors and their parameters
                    foreach (ConstructorInfo ci in type.GetConstructors())
                    {
                        CrestronConsole.PrintLine("Constructor {0}", ci.Name);
                        foreach (ParameterInfo p in ci.GetParameters())
                        {
                            CrestronConsole.PrintLine("Param: {0} - {1}", p.Name, p.ParameterType);
                        }
                    }

                    // Print out all properties and their read/write access
                    foreach (PropertyInfo prop in type.GetProperties())
                    {
                        CrestronConsole.PrintLine("{0} {1} get: {2}; set {3}", prop.PropertyType.Name, prop.Name, prop.CanRead, prop.CanWrite);
                    }

                    // Print out all fields and their types
                    foreach (FieldInfo fi in type.GetFields())
                    {
                        CrestronConsole.PrintLine("{0} {1}", fi.FieldType.Name, fi.Name);
                    }

                    // Print out all methods and their parameters
                    foreach (MethodInfo mi in type.GetMethods())
                    {
                        CrestronConsole.PrintLine("{0} {1}", mi.ReturnType.Name, mi.Name);
                        foreach (ParameterInfo mip in mi.GetParameters())
                        {
                            CrestronConsole.PrintLine("{0} - {1}", mip.ParameterType.Name, mip.Name);
                        }
                    }
                    CrestronConsole.PrintLine(string.Empty);
                    CrestronConsole.PrintLine(string.Empty);
                }
            }
            else
            {
                CrestronConsole.PrintLine("Unable to find HelloWorldLibrary.dll in NVRAM.");
            }
        }

        public void HelloWorld()
        {
            targetAssembly = Assembly.LoadFrom(@"\NVRAM\HelloWorldLibrary.dll"); // Load DLL file into memory
            if (targetAssembly.FullName.ToLower().Contains("helloworldlibrary"))
            {
                refObject = targetAssembly.CreateInstance("HelloWorldLibrary.HelloWorld"); // Creat a HelloWorld object
                refType = refObject.GetType();

                // Get a known property from the reference object and set it's value;
                PropertyInfo refInfo = refType.GetProperty("Enabled");
                if(!(bool)refInfo.GetValue(refObject,new Object[] {}))
                {
                    refInfo.SetValue(refObject, true, new Object[] {});
                }

                // Get expected method from reference type and call that method with no arguments
                MethodInfo refMethod = refType.GetMethod("PrintMessage");
                refMethod.Invoke(refObject, new Object[] { });

                // Get an expected method from reference type and call that method with appropriate arguments
                MethodInfo refMethod2 = refType.GetMethod("PrintCustomMessage");
                refMethod2.Invoke(refObject, new Object[] { "This is a custom message." });
            }
            else
            {
                ErrorLog.Error("Unable to find HelloWorldLibrary.dll in NVRAM.");
            }
        }

        public void ReportDisplays()
        {
            targetAssembly = Assembly.LoadFrom(@"\NVRAM\DisplayLibrary.dll");
            foreach (CType type in targetAssembly.GetTypes())
            {
                CrestronConsole.PrintLine("Object Type: {0}", type.FullName);
                CrestronConsole.PrintLine("Constructors: ");
                foreach (ConstructorInfo c in type.GetConstructors())
                {
                    CrestronConsole.Print("\t{0}(", c.Name);
                    foreach (ParameterInfo p in c.GetParameters())
                    {
                        CrestronConsole.Print("{0} {1}, ", p.ParameterType, p.Name);
                    }
                    CrestronConsole.Print(")");
                }
                CrestronConsole.PrintLine(string.Empty);

                CrestronConsole.PrintLine("Properties: ");
                foreach (PropertyInfo p in type.GetProperties())
                {
                    CrestronConsole.PrintLine("\t{0} {1} Get: {2}; Set: {3}", p.PropertyType.Name, p.Name, p.CanRead, p.CanWrite);
                }
                CrestronConsole.PrintLine(string.Empty);

                CrestronConsole.PrintLine("Methods:");
                foreach (MethodInfo m in type.GetMethods())
                {
                    CrestronConsole.Print("{0}(",m.Name);
                    foreach (ParameterInfo p in m.GetParameters())
                    {
                        CrestronConsole.Print("{0} {1}, ", p.ParameterType, p.Name);
                    }
                    CrestronConsole.Print(");");
                    CrestronConsole.PrintLine(string.Empty);
                }
            }
        }

        public IDisplay GetIDisplayObject()
        {
            IDisplay display = null;
            
            return display;
        }

        private bool searchForInterface(CType[] types, string interfaceName)
        {
            bool result = false;
            foreach (CType type in types)
            {
                if (type.Name == interfaceName)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}