using System;
namespace AvrSim.Instructions
{
	public static class Memory
	{
		[InstructionHandler("10q0_qq0d_dddd_0qqq", Core.R_Z, Name = "Ldd")]
		[InstructionHandler("10q0_qq0d_dddd_1qqq", Core.R_Y, Name = "Ldd")]
		public static RegisterFile Ldd(RegisterFile registerFile, byte d, byte q, MemoryBus memoryBus, object[] arguments)
		{
			var register = (byte)arguments[0];

			return registerFile.WithRegister(d, memoryBus.Load((ushort)(registerFile.GetWide(register) + q)));
		}

		[InstructionHandler("1001_000d_dddd_0001", Core.R_Z, 1, Name = "Ld")]
		[InstructionHandler("1001_000d_dddd_0010", Core.R_Z, -1, Name = "Ld")]
		[InstructionHandler("1000_000d_dddd_0000", Core.R_Z, 0, Name = "Ld")]
		[InstructionHandler("1001_000d_dddd_1001", Core.R_Y, 1, Name = "Ld")]
		[InstructionHandler("1001_000d_dddd_1010", Core.R_Y, -1, Name = "Ld")]
		[InstructionHandler("1000_000d_dddd_1000", Core.R_Y, 0, Name = "Ld")]
		[InstructionHandler("1001_000d_dddd_1101", Core.R_X, 1, Name = "Ld")]
		[InstructionHandler("1001_000d_dddd_1110", Core.R_X, -1, Name = "Ld")]
		[InstructionHandler("1001_000d_dddd_1100", Core.R_X, 0, Name = "Ld")]
		public static RegisterFile Ld(RegisterFile registerFile, byte d, MemoryBus memoryBus, object[] arguments)
		{
			var register = (byte)arguments[0];
			var change = (int)arguments[1];

			var preDecrement = change == -1;
			var postIncrement = change == 1;

			if (preDecrement)
			{
				registerFile = registerFile.WithWide(register, v => (ushort) (v - 1));
			}

			registerFile = registerFile.WithRegister(d, memoryBus.Load(registerFile.GetWide(register)));

			if (postIncrement)
			{
				registerFile = registerFile.WithWide(register, v => (ushort)(v + 1));
			}

			return registerFile;
		}

		[InstructionHandler("10q0_qq1r_rrrr_0qqq", Core.R_Z, Name = "Std")]
		[InstructionHandler("10q0_qq1r_rrrr_1qqq", Core.R_Y, Name = "Std")]
		public static RegisterFile Std(RegisterFile registerFile, byte r, byte q, MemoryBus memoryBus, object[] arguments)
		{
			var register = (byte) arguments[0];

			memoryBus.Store((ushort)(registerFile.GetWide(register) + q), registerFile[r]);

			return registerFile;
		}

		[InstructionHandler("1001_001r_rrrr_0001", Core.R_Z, 1, Name = "St")]
		[InstructionHandler("1001_001r_rrrr_0010", Core.R_Z, -1, Name = "St")]
		[InstructionHandler("1000_001r_rrrr_0000", Core.R_Z, 0, Name = "St")]
		[InstructionHandler("1001_001r_rrrr_1001", Core.R_Y, 1, Name = "St")]
		[InstructionHandler("1001_001r_rrrr_1010", Core.R_Y, -1, Name = "St")]
		[InstructionHandler("1000_001r_rrrr_1000", Core.R_Y, 0, Name = "St")]
		[InstructionHandler("1001_001r_rrrr_1101", Core.R_X, 1, Name = "St")]
		[InstructionHandler("1001_001r_rrrr_1110", Core.R_X, -1, Name = "St")]
		[InstructionHandler("1001_001r_rrrr_1100", Core.R_X, 0, Name = "St")]
		public static RegisterFile St(RegisterFile registerFile, byte r, MemoryBus memoryBus, object[] arguments)
		{
			var register = (byte)arguments[0];
			var change = (int)arguments[1];

			var preDecrement = change == -1;
			var postIncrement = change == 1;

			if (preDecrement)
			{
				registerFile = registerFile.WithWide(register, v => (ushort)(v - 1));
			}

			memoryBus.Store(registerFile.GetWide(register), r);

			if (postIncrement)
			{
				registerFile = registerFile.WithWide(register, v => (ushort)(v + 1));
			}

			return registerFile;
		}

		[InstructionHandler("1001_001d_dddd_0000_kkkk_kkkk_kkkk_kkkk", Name = "Sts")]
		public static RegisterFile Sts(RegisterFile registerFile, byte d, ushort k, MemoryBus memoryBus)
		{
			memoryBus.Store(k, registerFile[d]);

			return registerFile;
		}
	}
}
