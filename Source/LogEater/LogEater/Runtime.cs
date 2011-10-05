using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace LogEater
{
	public static class Runtime
	{
		#region Public Static Fields		
		public static string VersionString
		{
			get
			{
				Version version = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version;
				return version.Major + "." + version.Minor + "." + version.Build;
			}
		}

		public static DateTime CompileDate
		{
			get
			{
				System.Version version = System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version;
				return new DateTime (version.Build * TimeSpan.TicksPerDay + version.Revision * TimeSpan.TicksPerSecond * 2).AddYears (1999).AddDays (-1);
			}
		}
		#endregion
						
		#region Public Static Methods				
		public static void SetProcessName (string name)
		{
			if (Environment.OSVersion.Platform == PlatformID.Unix)
			{
				try
				{
					UnixSetProcessName (name);
				}
				catch
				{
				}
			}
		}

		static void UnixSetProcessName (string name)
		{
			try
			{
				if (prctl (15, Encoding.ASCII.GetBytes (name + "\0"), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero) != 0)
				{
					throw new ApplicationException ("Error setting process name: " + Mono.Unix.Native.Stdlib.GetLastError ());
				}
			}
			catch (EntryPointNotFoundException)
			{
				// Not every BSD has setproctitle
				try
				{
					setproctitle (Encoding.ASCII.GetBytes ("%s\0"), Encoding.ASCII.GetBytes (name + "\0"));
				}
				catch (EntryPointNotFoundException)
				{
					
				}
			}
		}
		#endregion

		#region Private Static Methods
		[DllImport("libc")]
		// Linux
		private static extern int prctl (int option, byte[] arg2, IntPtr arg3, IntPtr arg4, IntPtr arg5);

		[DllImport("libc")]
		// BSD
		private static extern void setproctitle (byte[] fmt, byte[] str_arg);
		#endregion
	}
}