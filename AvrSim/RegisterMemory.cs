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
			return core.Registers[address];
		}

		public void Store(ushort address, byte value)
		{
			core.Registers[address] = value;
		}
	}
}
