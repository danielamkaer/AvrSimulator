using System;

namespace AvrSim
{
	public class RegisterMemory : IMemory
	{
		readonly Core core;

		public RegisterMemory(Core core)
		{
			this.core = core;
		}

		public ushort Size => 32;

		public byte Load(ushort address)
		{
			return core.RegisterFile[address];
		}

		public void Store(ushort address, byte value)
		{
			throw new NotImplementedException();
			//core.Registers[address] = value;
		}
	}
}
