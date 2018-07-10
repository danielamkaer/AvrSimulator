using System;
using System.Diagnostics;

namespace AvrSim.Instructions
{
	public static class Misc
	{
		[InstructionHandler("1001_0101_1001_1000")]
		public static RegisterFile Break(RegisterFile registerFile)
		{
			Debugger.Break();

			return registerFile;
		}

		[InstructionHandler("1011_0AAd_dddd_AAAA")]
		public static RegisterFile In(RegisterFile registerFile, byte A, byte d, MemoryBus memoryBus)
		{
			// I/O Addresses are offset 0x20 on the memory bus.
			return registerFile.WithRegister(d, memoryBus.Load((ushort)(A + 0x20)));
		}

		[InstructionHandler("1011_1AAr_rrrr_AAAA")]
		public static RegisterFile Out(RegisterFile registerFile, byte A, byte r, MemoryBus memoryBus)
		{
			// I/O Addresses are offset 0x20 on the memory bus.
			memoryBus.Store((ushort)(A + 0x20), registerFile[r]);

			return registerFile;
		}
	}
}
