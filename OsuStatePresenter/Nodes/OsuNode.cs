using DVPF.Core;
using OsuMemoryDataProvider;
using System.Threading;
using System.Reflection;
using System;

namespace OsuStatePresenter.Nodes
{
    abstract class OsuNode : Node
    {
        protected static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        protected IOsuMemoryReader _memoryReader;
        //protected IOsuMemoryReader _memoryReader2;

        public OsuNode()
        {
            // TODO: One Node can hang another Node because they share the same OsuMemoryDataProvider instance - and the Node must wait for the reader to become "available" before it can read.

            /*
             * almost got it working, except wasn't able to type convert IOsuMemoryReader from the 2nd OsuMemoryDataProvider dll to the 1st.
             * 
             */
            /*
            var dllPath = @"C:\Users\waffle\Documents\Code\RTSP\rtspv5\RTSP.Osu\OsuMemoryDataProvider2.dll";
            //var assembly = domain.Load("OsuMemoryDataProvider");
            //AssemblyName name = AssemblyName.GetAssemblyName(dllPath);
            //var domain = AppDomain.CreateDomain("OsuMemory");
            var assembly = Assembly.LoadFile(dllPath);
            var dpType = assembly.GetType("OsuMemoryDataProvider.DataProvider");
            var readerType = assembly.GetType("OsuMemoryDataProvider.IOsuMemoryReader");

            dpType.GetMethod("Initalize").Invoke(null, null);
            var instanceField = dpType.GetField("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField); //  | BindingFlags.Instance
            var instance = instanceField.GetValue(null);

            var t = instance.GetType(); // QBH$pwEBMDeVPhQUOkTRZhk1zwwdrjHJZ$CZRGRU6rF7TwFRAnjNbRlZX_aju4J2vg

            var customMemReader = new IOsuMemoryReader_Custom(instance);

            dynamic changedObj = Convert.ChangeType(customMemReader, readerType);
            //_memoryReader2 = changedObj.Method();
            _memoryReader2 = changedObj;

            */







            //var DLL = Assembly.LoadFile(@"C:\Users\waffle\Documents\Code\RTSP\rtspv5\RTSP.Osu\OsuMemoryDataProvider2.dll");
            //var domain = AppDomain.CreateDomain("OsuMemory");
            //domain.DoCallBack(LoadAssemblyAndCallStaticMethod);
            //domain.DoCallBack(PrintLoadedAssemblies);










            //AppDomain domain = AppDomain.CreateDomain("OsuMemory");
            ////Do other things to the domain like set the security policy
            //string pathToDll = @"C:\Users\waffle\Documents\Code\RTSP\rtspv5\RTSP.Osu\OsuMemoryDataProvider2.dll"; //Full path to dll you want to load
            //Type t = typeof(IOsuMemoryReader);
            //IOsuMemoryReader myObject = (IOsuMemoryReader)domain.CreateInstanceAndUnwrap(pathToDll, "OsuMemoryDataProvider.DataProvider");
            //Console.WriteLine(myObject);









            //foreach (Type type in DLL.GetExportedTypes())
            //{
            //    if (type.ToString().Equals("OsuMemoryDataProvider.DataProvider"))
            //    {
            //        type.GetMethod("Initalize").Invoke(null, null);
            //        FieldInfo instance = type.GetField("Instance");

            //        var o = instance.GetValue(null);

            //        Console.WriteLine(o.GetType());

            //        break;
            //    }
            //}










            //Type IReaderType = null;

            //foreach (Type type in DLL.GetExportedTypes())
            //{
            //    if (type.ToString().Equals("OsuMemoryDataProvider.IOsuMemoryReader"))
            //    {
            //        IReaderType = type;
            //        break;
            //    }

            //}

            //foreach (Type type in DLL.GetExportedTypes())
            //{
            //    if (type.ToString().Equals("OsuMemoryDataProvider.DataProvider"))
            //    {
            //        type.GetMethod("Initalize").Invoke(null, null);
            //        FieldInfo instance = type.GetField("Instance");

            //        var o = instance.GetValue(null);

            //        //var r = (o as IReaderType.);
            //        Console.WriteLine(o.GetType());

            //        break;
            //    }
            //}


            //}







            OsuMemoryDataProvider.DataProvider.Initalize();
            _memoryReader = OsuMemoryDataProvider.DataProvider.Instance;
        }

        // Loads StaticMethodInHere.dll to the current AppDomain and calls static method 
        // StaticClass.DoSomething.  
        static void LoadAssemblyAndCallStaticMethod()
        {
            //var assembly = Assembly.LoadFrom(@"C:\Users\waffle\Documents\Code\RTSP\rtspv5\RTSP.Osu\OsuMemoryDataProvider2.dll");
            var assembly = Assembly.Load("OsuMemoryDataProvider");

            var dpType = assembly.GetType("OsuMemoryDataProvider.DataProvider");

            dpType.GetMethod("Initalize").Invoke(null, null);

            var instance = dpType.GetField("Instance", BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField); //  | BindingFlags.Instance

            var o = (IOsuMemoryReader)instance.GetValue(null);
        }

        // Prints the loaded assebmlies in the current AppDomain. For testing purposes.
        static void PrintLoadedAssemblies()
        {
            Console.WriteLine("/ Assemblies in {0} -------------------------------",
                              AppDomain.CurrentDomain.FriendlyName);

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Console.WriteLine(assembly.FullName);
            }
        }

    }

    class IOsuMemoryReader_Custom : IOsuMemoryReader, IConvertible
    {
        public IOsuMemoryReader_Custom(object instance)
        {
            Instance = instance;
            AssemblyQualifiedName = instance.GetType().AssemblyQualifiedName;
        }

        public object Instance { get; }
        public string AssemblyQualifiedName { get; }

        public OsuMemoryStatus GetCurrentStatus(out int statusNumber)
        {
            throw new NotImplementedException();
        }

        public float GetMapAr()
        {
            throw new NotImplementedException();
        }

        public float GetMapCs()
        {
            throw new NotImplementedException();
        }

        public string GetMapFolderName()
        {
            throw new NotImplementedException();
        }

        public float GetMapHp()
        {
            throw new NotImplementedException();
        }

        public int GetMapId()
        {
            throw new NotImplementedException();
        }

        public string GetMapMd5()
        {
            throw new NotImplementedException();
        }

        public float GetMapOd()
        {
            throw new NotImplementedException();
        }

        public float GetMapSetId()
        {
            throw new NotImplementedException();
        }

        public int GetMods()
        {
            throw new NotImplementedException();
        }

        public string GetOsuFileName()
        {
            throw new NotImplementedException();
        }

        public void GetPlayData(PlayContainer playContainer)
        {
            throw new NotImplementedException();
        }

        public string GetSongString()
        {
            throw new NotImplementedException();
        }

        public TypeCode GetTypeCode()
        {
            throw new NotImplementedException();
        }

        public double ReadDisplayedPlayerHp()
        {
            throw new NotImplementedException();
        }

        public int ReadPlayedGameMode()
        {
            throw new NotImplementedException();
        }

        public double ReadPlayerHp()
        {
            throw new NotImplementedException();
        }

        public int ReadPlayTime()
        {
            return ((IOsuMemoryReader)Instance).ReadPlayTime();
        }

        public int ReadScore()
        {
            throw new NotImplementedException();
        }

        public int ReadSongSelectGameMode()
        {
            throw new NotImplementedException();
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public char ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public string ToString(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            return (IOsuMemoryReader)this;
        }

        public ushort ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }
    }
}
