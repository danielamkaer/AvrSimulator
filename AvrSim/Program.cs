using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	class Program
	{
		public class PeripheralMemory : IMemory
		{
			public ushort Size => 64;

			public byte Load(ushort address)
			{
				throw new NotImplementedException();
			}

			public void Store(ushort address, byte value)
			{
				throw new NotImplementedException();
			}
		}

		static void Main(string[] args)
		{
			var program = File.ReadAllBytes(args[0]);

			var core = new Core(new Flash(program), new Ram(2048));
			core.MemoryBus.AddMap(0x20, 64, new PeripheralMemory());

			core.Run();
		}
	}
}
