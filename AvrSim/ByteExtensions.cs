using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	internal static class ByteExtensions
	{
		public static bool BitIsSet(this byte value, int bit)
		{
			return (value & (1 << bit)) != 0;
		}

		public static bool BitIsCleared(this byte value, int bit)
		{
			return !value.BitIsSet(bit);
		}

		public static byte WithBitSet(this byte value, int bit)
		{
			return (byte) (value | (1 << bit));
		}

		public static byte WithBitCleared(this byte value, int bit)
		{
			return (byte)(value & ~(1 << bit));
		}

		public static bool BitIsSet(this ushort value, int bit)
		{
			return (value & (1 << bit)) != 0;
		}

		public static bool BitIsCleared(this ushort value, int bit)
		{
			return !value.BitIsSet(bit);
		}

		public static ushort WithBitSet(this ushort value, int bit)
		{
			return (ushort)(value | (1 << bit));
		}

		public static ushort WithBitCleared(this ushort value, int bit)
		{
			return (ushort)(value & ~(1 << bit));
		}
	}
}
