using System.Collections.Generic;

namespace AvrSim
{
	public class Instruction
	{
		public ushort Address { get; set; }
		public string Opcode { get; set; }
		public Dictionary<char, uint> Values { get; set; }

		public uint this[char value]
		{
			get
			{
				return Values[value];
			}
		}
	}
}
