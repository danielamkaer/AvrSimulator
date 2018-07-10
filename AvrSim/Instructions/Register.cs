using System;
namespace AvrSim.Instructions
{
	public static class Register
	{

		[InstructionHandler("0010_11rd_dddd_rrrr")]
		public static RegisterFile Mov(RegisterFile registerFile, byte r, byte d)
		{
			return registerFile.WithRegister(d, registerFile[r]);
		}

		[InstructionHandler("1110_KKKK_dddd_KKKK")]
		public static RegisterFile Ldi(RegisterFile registerFile, byte d, byte K)
		{
			d = (byte)(16 + d);

			return registerFile.WithRegister(d, K);
		}
	}
}
