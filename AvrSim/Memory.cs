using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	public interface IMemory
	{
		ushort Size { get; }
		byte Load(ushort address);
		void Store(ushort address, byte value);
	}

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
			var map = (from m in MemoryMaps where m.Address <= address && address < m.Address + m.Size select m).Single();

			byte value = map.Memory.Load((ushort)(address - map.Address));
			Console.WriteLine($"Loaded 0x{value:X} from 0x{address:X}");
			return value;
		}

		public void Store(ushort address, byte value)
		{
			var map = (from m in MemoryMaps where m.Address <= address && address < m.Address + m.Size select m).Single();

			Console.WriteLine($"Storing 0x{value:X} at 0x{address:X}");
			map.Memory.Store((ushort)(address - map.Address), value);
		}
	}

	public class Stack
	{
		ushort stackPointer;
		public ushort StackPointer
		{
			get => stackPointer;
			private set
			{
				if (value > topOfStack)
				{
					throw new StackUnderflowException();
				}
				stackPointer = value;
			}
		}

		readonly ushort topOfStack;

		public Stack(MemoryBus memory, ushort topOfStack)
		{
			Memory = memory;
			stackPointer = topOfStack;
			this.topOfStack = topOfStack;
		}

		public MemoryBus Memory { get; }

		public byte Pop()
		{
			return Memory.Load(StackPointer++);
		}

		public void Push(byte value)
		{
			Memory.Store(StackPointer--, value);
		}

		public void PushWide(ushort value)
		{
			Push((byte)(value >> 8));
			Push((byte)(value & 0xFF));
		}

		public ushort PopWide()
		{
			ushort value = Pop();
			value |= (ushort)(Pop() << 8);

			return value;
		}
	}
}
