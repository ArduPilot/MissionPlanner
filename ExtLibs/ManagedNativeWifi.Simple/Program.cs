using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagedNativeWifi.Simple
{
	class Program
	{
		static void Main(string[] args)
		{
			if (!Debugger.IsAttached)
				Debug.Listeners.Add(new TextWriterTraceListener(Console.Out));

			Debug.WriteLine("[Available Network SSIDs]");
			NativeWifi.GetAvailableNetworkSsids().ToArray();

			Debug.WriteLine("[Connected Network SSIDs]");
			NativeWifi.GetConnectedNetworkSsids().ToArray();
		}
	}
}