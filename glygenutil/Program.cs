using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glygenutil
{
	class Program
	{
		private static void Usage(int exitCode)
		{
			Console.Out.WriteLine(Resources.usage);
			Environment.Exit(exitCode);
		}

		static void Main(string[] args)
		{
			InstallContext context = new InstallContext(null, args);
			if ( args.Length == 0 )
				Usage(1);

			switch ( (context.Parameters["command"] ?? String.Empty).ToLower() ) {
				case "resolve":
					resolve(context);
					break;
				case "validate":
					validate(context);
					break;
			}
		}

		private static void resolve(InstallContext context)
		{
			
		}

		private static void validate(InstallContext context)
		{
			
		}
	}
}
