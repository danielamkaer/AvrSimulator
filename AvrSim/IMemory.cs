namespace AvrSim
{
	public interface IMemory
	{
		ushort Size { get; }
		byte Load(ushort address);
		void Store(ushort address, byte value);
	}
}
