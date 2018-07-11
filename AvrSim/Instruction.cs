using System.Collections.Generic;

namespace AvrSim
{
	public class Instruction
	{
		public ushort Address { get; set; }
		public string Opcode { get; set; }
		public byte[] Bytes { get; set; }
		public Dictionary<char, InstructionValue> Values { get; set; }

		public InstructionValue this[char value]
		{
			get
			{
				return Values[value];
			}
		}
	}
}
