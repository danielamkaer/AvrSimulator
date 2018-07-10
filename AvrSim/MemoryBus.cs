using System;
using System.Collections.Generic;
using System.Linq;

namespace AvrSim
{
	public class MemoryBus
	{
		List<MemoryMap> MemoryMaps { get; } = new List<MemoryMap>();

		public void AddMap(ushort address, ushort size, IMemory memory)
		{
			var end = address + size - 1;

			if ((from m in MemoryMaps where m.Address <= address && address < m.Address + m.Size select m).Any() ||
				(from m in MemoryMaps where m.Address <= end && end < m.Address + m.Size select m).Any())
			{
				throw new Exception($"Overlap in memory map");
			}

			MemoryMaps.Add(new MemoryMap(address, size, memory));
		}

		public byte Load(ushort address)
		{
			var map = GetMap(address);

			byte value = map.Memory.Load((ushort)(address - map.Address));
			Console.Error.WriteLine($"Loaded 0x{value:X} from 0x{address:X}");
			return value;
		}

		MemoryMap GetMap(ushort address)
		{
			MemoryMap map;
			try
			{
				map = (from m in MemoryMaps where m.Address <= address && address < m.Address + m.Size select m).Single();
			}
			catch (InvalidOperationException)
			{
				throw new InvalidMemoryAddressException();
			}

			return map;
		}

		public void Store(ushort address, byte value)
		{
			var map = GetMap(address);

			Console.Error.WriteLine($"Storing 0x{value:X} at 0x{address:X}");
			map.Memory.Store((ushort)(address - map.Address), value);
		}
	}
}
