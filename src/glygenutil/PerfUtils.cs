using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace glygenutil
{
	public class Benchmark : IDisposable
	{
		private readonly Stopwatch timer = new Stopwatch();
		private readonly string benchmarkName;

		public Benchmark(string benchmarkName)
		{
			this.benchmarkName = benchmarkName;
			timer.Start();
		}

		public void Dispose()
		{
			timer.Stop();
			Console.WriteLine($"{benchmarkName} {timer.Elapsed}");
		}
	}
}
