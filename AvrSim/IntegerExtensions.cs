using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	public static class IntegerExtensions
	{
		public static int ToSigned(this uint unsigned, int bitSize)
		{
			var signBit = bitSize - 1;

			if ((unsigned & (1 << signBit)) != 0)
			{
				// number is negative
				var extended = uint.MaxValue & unsigned;
				var twosComplement = (int) -((~extended + 1) & (uint.MaxValue >> (32 - bitSize)));

				return twosComplement;
			}

			return (int)unsigned;
		}

		public static short ToSigned(this ushort unsigned, int bitSize)
		{
			var signBit = bitSize - 1;

			if ((unsigned & (1 << signBit)) != 0)
			{
				// number is negative
				var extended = ushort.MaxValue & unsigned;
				var twosComplement = (short)-((~extended + 1) & (ushort.MaxValue >> (16 - bitSize)));

				return twosComplement;
			}

			return (short)unsigned;
		}
	}
}
