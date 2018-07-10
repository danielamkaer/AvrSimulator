using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace AvrSim
{
	public struct RegisterFile
	{
		public RegisterFile(byte[] registers, StatusRegister statusRegister, uint programCounter, ushort stackPointer)
		{
			Registers = registers;
			StatusRegister = statusRegister;
			ProgramCounter = programCounter;
			StackPointer = stackPointer;
		}

		public RegisterFile WithStatusRegister(StatusRegister statusRegister)
		{
			return new RegisterFile(Registers, statusRegister, ProgramCounter, StackPointer);
		}

		public RegisterFile WithStatusRegister(Func<StatusRegister, StatusRegister> func)
		{
			return WithStatusRegister(func(StatusRegister));
		}

		public RegisterFile WithRegisters(byte[] registers)
		{
			return new RegisterFile(registers, StatusRegister, ProgramCounter, StackPointer);
		}

		public RegisterFile WithRegister(byte registerNumber, byte value)
		{
			var registers = Registers.ToArray();
			registers[registerNumber] = value;

			return new RegisterFile(registers, StatusRegister, ProgramCounter, StackPointer);
		}

		public RegisterFile WithRegister(byte register, Func<byte, byte> func)
		{
			return WithRegister(register, func(this[register]));
		}

		public ushort GetWide(byte register)
		{
			register = (byte)(register & ~(1));

			return (ushort)(Registers[register + 1] << 8 | Registers[register]);
		}

		public RegisterFile WithWide(byte register, ushort value)
		{
			register = (byte)(register & ~(1));

			return WithRegister((byte)(register + 1), (byte)(value >> 8)).WithRegister(register, (byte)(value & 0xff));
		}

		public RegisterFile WithWide(byte register, Func<ushort, ushort> func)
		{
			return WithWide(register, func(GetWide(register)));
		}

		public RegisterFile WithProgramCounter(uint programCounter)
		{
			return new RegisterFile(Registers, StatusRegister, programCounter, StackPointer);
		}

		public RegisterFile WithProgramCounter(Func<uint, uint> func)
		{
			return WithProgramCounter(func(ProgramCounter));
		}

		public RegisterFile WithStackPointer(ushort stackPointer)
		{
			return new RegisterFile(Registers, StatusRegister, ProgramCounter, stackPointer);
		}

		public RegisterFile WithStackPointer(Func<ushort, ushort> func)
		{
			return WithStackPointer(func(StackPointer));
		}

		byte[] Registers { get; }

		public uint ProgramCounter { get; }
		public ushort StackPointer { get; }

		public byte this[int index]
		{
			get => Registers[index];
		}

		public StatusRegister StatusRegister { get; }

		public override string ToString()
		{
			var regs = Registers;
			var registers = string.Join(" ", Enumerable.Range(0, Registers.Length).Select(i => string.Format("{0:X2}", regs[i])));
			return $"SREG: [{StatusRegister}] SP: {StackPointer:X4} PC: {ProgramCounter*2:X4} : {registers}";
		}
	}
}
