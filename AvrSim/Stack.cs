using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{

	public class Stack
	{
		public ushort TopOfStack { get; }

		public Stack(MemoryBus memory, ushort topOfStack)
		{
			Memory = memory;
			TopOfStack = topOfStack;
		}

		public MemoryBus Memory { get; }

		public RegisterFile Pop(RegisterFile registerFile, out byte value)
		{
			if (registerFile.StackPointer >= TopOfStack)
			{
				throw new StackUnderflowException();
			}

			value = Memory.Load(registerFile.StackPointer);
			return registerFile.WithStackPointer(sp => (ushort) (sp + 1));
		}

		public RegisterFile Push(RegisterFile registerFile, byte value)
		{
			Memory.Store(registerFile.StackPointer, value);
			return registerFile.WithStackPointer(sp => (ushort)(sp - 1));
		}

		public RegisterFile PushWide(RegisterFile registerFile, ushort value)
		{
			registerFile = Push(registerFile, (byte)(value >> 8));
			return Push(registerFile, (byte)(value & 0xFF));
		}

		public RegisterFile PopWide(RegisterFile registerFile, out ushort value)
		{
			registerFile = Pop(registerFile, out var h);
			registerFile = Pop(registerFile, out var l);

			value = (ushort) (h | (l << 8));

			return registerFile;
		}
	}
}
