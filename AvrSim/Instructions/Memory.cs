using System;
namespace AvrSim.Instructions
{
	public static class Memory
	{
		[InstructionHandler("10q0_qq0d_dddd_0qqq", "Z")]
		[InstructionHandler("10q0_qq0d_dddd_1qqq", "Y")]
		public static RegisterFile Ldd(RegisterFile registerFile, byte d, byte q, MemoryBus memoryBus, string[] arguments)
		{
			switch (arguments[0])
			{
				case "Z":
					return registerFile.WithRegister(d, memoryBus.Load((ushort)(registerFile.GetWide(Core.R_Z) + q)));
				case "Y":
					return registerFile.WithRegister(d, memoryBus.Load((ushort)(registerFile.GetWide(Core.R_Y) + q)));

				default:
					throw new ArgumentException(nameof(arguments));
			}
		}

		[InstructionHandler("1001_000d_dddd_0001", "Z", "+")]
		[InstructionHandler("1001_000d_dddd_0010", "Z", "-")]
		[InstructionHandler("1000_000d_dddd_0000", "Z", "")]
		[InstructionHandler("1001_000d_dddd_1001", "Y", "+")]
		[InstructionHandler("1001_000d_dddd_1010", "Y", "-")]
		[InstructionHandler("1000_000d_dddd_1000", "Y", "")]
		[InstructionHandler("1001_000d_dddd_1101", "X", "+")]
		[InstructionHandler("1001_000d_dddd_1110", "X", "-")]
		[InstructionHandler("1001_000d_dddd_1100", "X", "")]
		public static RegisterFile Ld(RegisterFile registerFile, byte d, MemoryBus memoryBus, string[] arguments)
		{
			var preDecrement = arguments[1] == "-";
			var postIncrement = arguments[1] == "+";
			var register = (byte) (arguments[0] == "Z" ? Core.R_Z : (arguments[0] == "Y" ? Core.R_Y : (arguments[0] == "X" ? Core.R_X : 0)));
			if (register == 0)
			{
				throw new ArgumentException(nameof(arguments));
			}

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

		[InstructionHandler("10q0_qq1r_rrrr_0qqq", "Z")]
		[InstructionHandler("10q0_qq1r_rrrr_1qqq", "Y")]
		public static RegisterFile Std_Z(RegisterFile registerFile, byte r, byte q, MemoryBus memoryBus, string[] arguments)
		{
			memoryBus.Store((ushort)(registerFile.GetWide(Core.R_Z) + q), registerFile[r]);

			switch (arguments[0])
			{
				case "Z":
					memoryBus.Store((ushort)(registerFile.GetWide(Core.R_Z) + q), registerFile[r]);
					break;
				case "Y":
					memoryBus.Store((ushort)(registerFile.GetWide(Core.R_Y) + q), registerFile[r]);
					break;
				default:
					throw new ArgumentException(nameof(arguments));
			}

			return registerFile;
		}

		[InstructionHandler("1001_001r_rrrr_0001", "Z", "+")]
		[InstructionHandler("1001_001r_rrrr_0010", "Z", "-")]
		[InstructionHandler("1000_001r_rrrr_0000", "Z", "")]
		[InstructionHandler("1001_001r_rrrr_1001", "Y", "+")]
		[InstructionHandler("1001_001r_rrrr_1010", "Y", "-")]
		[InstructionHandler("1000_001r_rrrr_1000", "Y", "")]
		[InstructionHandler("1001_001r_rrrr_1101", "X", "+")]
		[InstructionHandler("1001_001r_rrrr_1110", "X", "-")]
		[InstructionHandler("1001_001r_rrrr_1100", "X", "")]
		public static RegisterFile St(RegisterFile registerFile, byte r, MemoryBus memoryBus, string[] arguments)
		{

			var preDecrement = arguments[1] == "-";
			var postIncrement = arguments[1] == "+";
			var register = (byte)(arguments[0] == "Z" ? Core.R_Z : (arguments[0] == "Y" ? Core.R_Y : (arguments[0] == "X" ? Core.R_X : 0)));
			if (register == 0)
			{
				throw new ArgumentException(nameof(arguments));
			}

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

		[InstructionHandler("1001_001d_dddd_0000_kkkk_kkkk_kkkk_kkkk")]
		public static RegisterFile Sts(RegisterFile registerFile, byte d, ushort k, MemoryBus memoryBus)
		{
			memoryBus.Store(k, registerFile[d]);

			return registerFile;
		}
	}
}
