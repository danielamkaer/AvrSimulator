using System;
namespace AvrSim.Instructions
{
	public static class Memory
	{
		[InstructionHandler("10q0_qq0d_dddd_0qqq")]
		public static RegisterFile Ldd_Z(RegisterFile registerFile, byte d, byte q, MemoryBus memoryBus)
		{
			return registerFile.WithRegister(d, memoryBus.Load((ushort)(registerFile.GetWide(Core.R_Z) + q)));
		}

		[InstructionHandler("1001_000d_dddd_0001")]
		public static RegisterFile Ld_Z_PostIncrement(RegisterFile registerFile, byte d, MemoryBus memoryBus)
		{
			return registerFile
				.WithRegister(d, memoryBus.Load(registerFile.GetWide(Core.R_Z)))
				.WithWide(Core.R_Z, (ushort)(registerFile.GetWide(Core.R_Z) + 1));
		}

		[InstructionHandler("1001_000d_dddd_0010")]
		public static RegisterFile Ld_Z_PreDecrement(RegisterFile registerFile, byte d, MemoryBus memoryBus)
		{
			return registerFile
				.WithRegister(d, memoryBus.Load(registerFile.GetWide(Core.R_Z - 1)))
				.WithWide(Core.R_Z, (ushort)(registerFile.GetWide(Core.R_Z) - 1));
		}

		[InstructionHandler("1000_000d_dddd_0000")]
		public static RegisterFile Ld_Z(RegisterFile registerFile, byte d, MemoryBus memoryBus)
		{
			return registerFile
				.WithRegister(d, memoryBus.Load(registerFile.GetWide(Core.R_Z)));
		}

		[InstructionHandler("10q0_qq0d_dddd_1qqq")]
		public static RegisterFile Ldd_Y(RegisterFile registerFile, byte d, byte q, MemoryBus memoryBus)
		{
			return registerFile.WithRegister(d, memoryBus.Load((ushort)(registerFile.GetWide(Core.R_Y) + q)));
		}

		[InstructionHandler("1001_000d_dddd_1001")]
		public static RegisterFile Ld_Y_PostIncrement(RegisterFile registerFile, byte d, MemoryBus memoryBus)
		{
			return registerFile
				.WithRegister(d, memoryBus.Load(registerFile.GetWide(Core.R_Y)))
				.WithWide(Core.R_Y, (ushort)(registerFile.GetWide(Core.R_Y) + 1));
		}

		[InstructionHandler("1001_000d_dddd_1010")]
		public static RegisterFile Ld_Y_PreDecrement(RegisterFile registerFile, byte d, MemoryBus memoryBus)
		{
			return registerFile
				.WithRegister(d, memoryBus.Load(registerFile.GetWide(Core.R_Y - 1)))
				.WithWide(Core.R_Y, (ushort)(registerFile.GetWide(Core.R_Y) - 1));
		}

		[InstructionHandler("1000_000d_dddd_1000")]
		public static RegisterFile Ld_Y(RegisterFile registerFile, byte d, MemoryBus memoryBus)
		{
			return registerFile
				.WithRegister(d, memoryBus.Load(registerFile.GetWide(Core.R_Y)));
		}

		[InstructionHandler("1001_000d_dddd_1101")]
		public static RegisterFile Ld_X_PostIncrement(RegisterFile registerFile, byte d, MemoryBus memoryBus)
		{
			return registerFile
				.WithRegister(d, memoryBus.Load(registerFile.GetWide(Core.R_X)))
				.WithWide(Core.R_X, (ushort)(registerFile.GetWide(Core.R_X) + 1));
		}

		[InstructionHandler("1001_000d_dddd_1110")]
		public static RegisterFile Ld_X_PreDecrement(RegisterFile registerFile, byte d, MemoryBus memoryBus)
		{
			return registerFile
				.WithRegister(d, memoryBus.Load(registerFile.GetWide(Core.R_X - 1)))
				.WithWide(Core.R_X, (ushort)(registerFile.GetWide(Core.R_X) - 1));
		}

		[InstructionHandler("1001_000d_dddd_1100")]
		public static RegisterFile Ld_X(RegisterFile registerFile, byte d, MemoryBus memoryBus)
		{
			return registerFile
				.WithRegister(d, memoryBus.Load(registerFile.GetWide(Core.R_X)));
		}

		[InstructionHandler("10q0_qq1r_rrrr_0qqq")]
		public static RegisterFile Std_Z(RegisterFile registerFile, byte r, byte q, MemoryBus memoryBus)
		{
			memoryBus.Store((ushort)(registerFile.GetWide(Core.R_Z) + q), registerFile[r]);

			return registerFile;
		}

		[InstructionHandler("10q0_qq1r_rrrr_1qqq")]
		public static RegisterFile Std_Y(RegisterFile registerFile, byte r, byte q, MemoryBus memoryBus)
		{
			memoryBus.Store((ushort)(registerFile.GetWide(Core.R_Y) + q), registerFile[r]);

			return registerFile;
		}

		[InstructionHandler("1001_001r_rrrr_0001")]
		public static RegisterFile St_Z_PostIncrement(RegisterFile registerFile, byte r, MemoryBus memoryBus)
		{
			return St(registerFile, Core.R_Z, r, memoryBus).WithWide(Core.R_Z, x => (ushort)(x + 1));
		}

		[InstructionHandler("1001_001r_rrrr_0010")]
		public static RegisterFile St_Z_PreDecrement(RegisterFile registerFile, byte r, MemoryBus memoryBus)
		{
			registerFile = registerFile.WithWide(Core.R_Z, x => (ushort)(x - 1));

			return St(registerFile, Core.R_Z, r, memoryBus);
		}

		[InstructionHandler("1000_001r_rrrr_0000")]
		public static RegisterFile St_Z(RegisterFile registerFile, byte r, MemoryBus memoryBus)
		{
			return St(registerFile, Core.R_Z, r, memoryBus);
		}

		[InstructionHandler("1001_001r_rrrr_1001")]
		public static RegisterFile St_Y_PostIncrement(RegisterFile registerFile, byte r, MemoryBus memoryBus)
		{
			return St(registerFile, Core.R_Y, r, memoryBus).WithWide(Core.R_Y, x => (ushort)(x + 1));
		}

		[InstructionHandler("1001_001r_rrrr_1010")]
		public static RegisterFile St_Y_PreDecrement(RegisterFile registerFile, byte r, MemoryBus memoryBus)
		{
			registerFile = registerFile.WithWide(Core.R_Y, x => (ushort)(x - 1));

			return St(registerFile, Core.R_Y, r, memoryBus);
		}

		[InstructionHandler("1000_001r_rrrr_1000")]
		public static RegisterFile St_Y(RegisterFile registerFile, byte r, MemoryBus memoryBus)
		{
			return St(registerFile, Core.R_Y, r, memoryBus);
		}

		[InstructionHandler("1001_001r_rrrr_1101")]
		public static RegisterFile St_X_PostIncrement(RegisterFile registerFile, byte r, MemoryBus memoryBus)
		{
			return St(registerFile, Core.R_X, r, memoryBus).WithWide(Core.R_X, x => (ushort)(x + 1));
		}

		[InstructionHandler("1001_001r_rrrr_1110")]
		public static RegisterFile St_X_PreDecrement(RegisterFile registerFile, byte r, MemoryBus memoryBus)
		{
			registerFile = registerFile.WithWide(Core.R_X, x => (ushort) (x - 1));

			return St(registerFile, Core.R_X, r, memoryBus);
		}

		[InstructionHandler("1001_001r_rrrr_1100")]
		public static RegisterFile St_X(RegisterFile registerFile, byte r, MemoryBus memoryBus)
		{
			return St(registerFile, Core.R_X, r, memoryBus);
		}

		static RegisterFile St(RegisterFile registerFile, byte register, byte r, MemoryBus memoryBus)
		{
			memoryBus.Store(registerFile.GetWide(register), registerFile[r]);

			return registerFile;
		}

		[InstructionHandler("1001_001d_dddd_0000_kkkk_kkkk_kkkk_kkkk")]
		public static RegisterFile Sts(RegisterFile registerFile, byte d, ushort k, MemoryBus memoryBus)
		{
			memoryBus.Store(k, registerFile[d]);

			return registerFile;
		}
	}
}
