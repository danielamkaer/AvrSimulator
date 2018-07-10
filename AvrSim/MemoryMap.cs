namespace AvrSim
{
	public class MemoryMap
	{
		public MemoryMap(ushort address, ushort size, IMemory memory)
		{
			Address = address;
			Size = size;
			Memory = memory;
		}

		public ushort Address { get; }
		public ushort Size { get; }
		public IMemory Memory { get; }
	}
}
