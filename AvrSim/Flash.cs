using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	public class Flash
	{
		ushort[] storage = new ushort[32 * 1024 / 2];

		public Flash(byte[] binary)
		{
			int i;
			for (i = 0; i < binary.Length; i += 2)
			{
				ushort word = (ushort)((binary[i + 1] << 8) | (binary[i]));
				storage[i / 2] = word;
			}

			for (int a = i / 2; a < storage.Length; a++)
			{
				storage[a] = 0b1001_0101_1001_1000; // The BREAK instruction.
			}
		}

		public ushort GetInstruction(uint programCounter)
		{
			return storage[programCounter];
		}
	}
}
