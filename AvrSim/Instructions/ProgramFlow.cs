using System;
namespace AvrSim.Instructions
{
	public static class ProgramFlow
	{
		[InstructionHandler("1001_0101_0000_1000", Name = "Ret")]
		public static RegisterFile Ret(RegisterFile registerFile, AvrSim.Stack stack)
		{
			registerFile = stack.PopWide(registerFile, out var programCounter);

			return registerFile.WithProgramCounter(programCounter);
		}

		[InstructionHandler("0000_0000_0000_0000", Name = "Nop")]
		public static RegisterFile Nop(RegisterFile registerFile)
		{
			return registerFile;
		}

		[InstructionHandler("1100_kkkk_kkkk_kkkk", Name = "Rjmp")]
		public static RegisterFile Rjmp(RegisterFile registerFile, ushort k)
		{
			return registerFile.WithProgramCounter(p => (uint)(p + k.ToSigned(12)));
		}

		[InstructionHandler("1101_kkkk_kkkk_kkkk", Name = "Rcall")]
		public static RegisterFile Rcall(RegisterFile registerFile, ushort k, AvrSim.Stack stack)
		{
			return stack.PushWide(registerFile, (ushort)registerFile.ProgramCounter).WithProgramCounter(p => p + k);
		}

		[InstructionHandler("1111_01kk_kkkk_k001", Name = "Brne")]
		public static RegisterFile Brne(RegisterFile registerFile, sbyte k)
		{
			if (!registerFile.StatusRegister.Z)
			{
				return registerFile.WithProgramCounter(p => (uint)(p + k));
			}

			return registerFile;
		}
	}
}
