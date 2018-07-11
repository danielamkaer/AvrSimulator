using System;
namespace AvrSim.Instructions
{
	public static class Stack
	{
		[InstructionHandler("1001_000d_dddd_1111", Name = "Pop")]
		public static RegisterFile Pop(RegisterFile registerFile, byte d, AvrSim.Stack stack)
		{
			registerFile = stack.Pop(registerFile, out var value);

			return registerFile.WithRegister(d, value);
		}

		[InstructionHandler("1001_001d_dddd_1111", Name = "Push")]
		public static RegisterFile Push(RegisterFile registerFile, byte d, AvrSim.Stack stack)
		{
			return stack.Push(registerFile, registerFile[d]);
		}
	}
}
