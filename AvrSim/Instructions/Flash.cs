using System;
namespace AvrSim.Instructions
{
	public static class Flash
	{
		[InstructionHandler("1001_000d_dddd_0101")]
		public static RegisterFile Lpm_PostIncrement(RegisterFile registerFile, byte d, AvrSim.Flash programMemory)
		{
			return _Lpm(registerFile, d, programMemory).WithWide(Core.R_Z, (ushort)(registerFile.GetWide(Core.R_Z) + 1));
		}

		[InstructionHandler("1001_000d_dddd_0100")]
		public static RegisterFile Lpm(RegisterFile registerFile, byte d, AvrSim.Flash programMemory)
		{
			return _Lpm(registerFile, d, programMemory);
		}

		[InstructionHandler("1001_0101_1100_1000")]
		public static RegisterFile Lpm_R0(RegisterFile registerFile, AvrSim.Flash programMemory)
		{
			return _Lpm(registerFile, 0, programMemory);
		}

		static RegisterFile _Lpm(RegisterFile registerFile, byte d, AvrSim.Flash programMemory)
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

			return registerFile.WithRegister(d, value);
		}
	}
}
