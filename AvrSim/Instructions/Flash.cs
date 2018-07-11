using System;
namespace AvrSim.Instructions
{
	public static class Flash
	{
		[InstructionHandler("1001_000d_dddd_0101", true, Name = "Lpm")]
		[InstructionHandler("1001_000d_dddd_0100", false, Name = "Lpm")]
		public static RegisterFile Lpm(RegisterFile registerFile, byte d, AvrSim.Flash programMemory, object[] arguments)
		{
			var postIncrement = (bool)arguments[0];

			var word = (uint)(registerFile.GetWide(Core.R_Z) >> 1);
			var MSB = (registerFile.GetWide(Core.R_Z) & 1) != 0;

			var data = programMemory.GetInstruction(word);

			byte value;
			if (MSB)
			{
				value = (byte)(data >> 8);
			}
			else
			{
				value = (byte)(data & 0xff);
			}

			registerFile = registerFile.WithRegister(d, value);

			if (postIncrement)
			{
				registerFile = registerFile.WithWide(Core.R_Z, z => (ushort)(z + 1));
			}

			return registerFile;
		}

		[InstructionHandler("1001_0101_1100_1000", Name = "Lpm")]
		public static RegisterFile Lpm(RegisterFile registerFile, AvrSim.Flash programMemory)
		{

			var word = (uint)(registerFile.GetWide(Core.R_Z) >> 1);
			var MSB = (registerFile.GetWide(Core.R_Z) & 1) != 0;

			var data = programMemory.GetInstruction(word);

			byte value;
			if (MSB)
			{
				value = (byte)(data >> 8);
			}
			else
			{
				value = (byte)(data & 0xff);
			}

			return registerFile.WithRegister(0, value);
		}
	}
}
