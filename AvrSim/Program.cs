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
				return 0;
			}

			public void Store(ushort address, byte value)
			{
				if (address == 0)
				{
					Console.Write((char)value);
				}
			}
		}

		static void Main(string[] args)
		{
			//Console.SetError(new StreamWriter(new MemoryStream()));
			var program = File.ReadAllBytes(args[0]);

			var core = new Core(new Flash(program), new Ram(2048));

			core.MemoryBus.AddMap(0x20, 64, new PeripheralMemory());

			try
			{
				core.Run();
			}
			catch (StackUnderflowException)
			{
				Console.WriteLine($"Program returned from main.");
			}
		}
	}
}
