namespace AvrSim
{
	public class Ram : IMemory
	{
		byte[] storage;

		public Ram(ushort sizeInBytes)
		{
			storage = new byte[sizeInBytes];

			for (var address = 0; address < storage.Length; address++)
			{
				storage[address] = 0;
			}
		}

		public ushort Size => (ushort)storage.Length;

		public byte Load(ushort address)
		{
			return storage[address];
		}

		public void Store(ushort address, byte value)
		{
			storage[address] = value;
		}
	}
}
