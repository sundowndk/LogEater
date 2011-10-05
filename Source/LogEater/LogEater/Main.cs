using System;
using System.IO;
using System.Collections.Generic;

using Toolbox;

namespace LogEater
{
	class MainClass
	{		
		public static void WriteLogSegment (string Path, List<string> Data)
		{
			TextWriter textfile = new StreamWriter (Path, false);
			foreach (string line in Data)
			{
				textfile.WriteLine (line);
			}
			textfile.Close ();
			textfile.Dispose ();
		}
		
		public static void Main (string[] args)
		{														
			Toolbox.Arg.Add ("output", "", "{0} to output log files", true, "PATH", "output");
			Toolbox.Arg.Add ("lines", "", "Number of lines in each log segment", true, "NUM", "lines");
//			Toolbox.Arg.Add ("forceinterval", "", "Force log segmentation every specified minute count. For slow growing logs.", true, "NUM", "forceinterval");		
//			Toolbox.Arg.Add ("verbose", "v", "Output statistics when done.");
			Toolbox.Arg.Add ("help", "h", "Show this help", "help");						
			
			Arg.Parse (args);
			
			#region HELP
			if (Toolbox.Arg.Found ("help"))
			{
				Console.WriteLine ("LogEater v"+ Runtime.VersionString +" ["+ Runtime.CompileDate +"]\n");
				Console.WriteLine("Usage: logeater [OPTIONS]");
				Console.WriteLine (Toolbox.Arg.Description);				
				Environment.Exit (0);
			} 		
			#endregion
			
			#region PROCESS
			if (Toolbox.Arg.Found ("output"))
			{
				#region OUTPUT
				string path = Toolbox.Arg.Value ("output");
				
				if (!Directory.Exists (path))
				{
					Console.WriteLine ("Error: --output, path does not exist.");
					Environment.Exit (0);
				}
				
				#endregion
				
				#region LINES
				int lines = 1000;
				if (Toolbox.Arg.Found ("lines"))
				{
					try
					{
						lines = int.Parse (Toolbox.Arg.Value ("lines"));
					}
					catch
					{
						Console.WriteLine ("Error: --lines, expected an integer.");
						Environment.Exit (0);
					}
				}
				#endregion
				
				#region FORCEINTERVAL
				int forceinterval = 0;
				if (Toolbox.Arg.Found ("forceinterval"))
				{
					try
					{
						forceinterval = int.Parse (Toolbox.Arg.Value ("forceinterval"));	
					}
					catch
					{
						Console.WriteLine ("Error: --forceinterval, expected an integer.");
						Environment.Exit (0);
					}
				}
				#endregion
											
				string line = string.Empty;
				List<string> segment = new List<string> ();						
				while ((line = Console.ReadLine()) != null)
				{
					segment.Add (line);
				
					if (segment.Count > lines)
					{
						WriteLogSegment (path +"/"+ Guid.NewGuid ().ToString (), segment);						
						segment.Clear ();
					}
				}
			
				WriteLogSegment (path +"/"+ Guid.NewGuid ().ToString (), segment);						
				Environment.Exit (0);
			}
			else
			{
				Console.WriteLine ("Error: No output defined.");
				Environment.Exit (0);
			}
			#endregion
		}
	}
}