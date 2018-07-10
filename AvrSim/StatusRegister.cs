using System;

namespace AvrSim
{
	public struct StatusRegister
	{
		public bool C { get; }
		public bool Z { get; }
		public bool N { get; }
		public bool V { get; }
		public bool S { get; }
		public bool H { get; }
		public bool T { get; }
		public bool I { get; }

		public StatusRegister(bool c, bool z, bool n, bool v, bool s, bool h, bool t, bool i)
		{
			C = c;
			Z = z;
			N = n;
			V = v;
			S = s;
			H = h;
			T = t;
			I = i;
		}

		public override string ToString()
		{
			char[] str = new char[8];
			str[0] = C ? 'C' : ' ';
			str[1] = Z ? 'Z' : ' ';
			str[2] = N ? 'N' : ' ';
			str[3] = V ? 'V' : ' ';
			str[4] = S ? 'S' : ' ';
			str[5] = H ? 'H' : ' ';
			str[6] = T ? 'T' : ' ';
			str[7] = I ? 'I' : ' ';

			return new string(str);
		}

		public StatusRegister WithCarry(bool c) => new StatusRegister(c, Z, N, V, S, H, T, I);
		public StatusRegister WithZero(bool z) => new StatusRegister(C, z, N, V, S, H, T, I);
		public StatusRegister WithNegative(bool n) => new StatusRegister(C, Z, n, V, S, H, T, I);
		public StatusRegister WithTwosComplementOverflow(bool v) => new StatusRegister(C, Z, N, v, S, H, T, I);
		public StatusRegister WithSigned(bool s) => new StatusRegister(C, Z, N, V, s, H, T, I);
		public StatusRegister WithHalfCarry(bool h) => new StatusRegister(C, Z, N, V, S, h, T, I);
		public StatusRegister WithTransfer(bool t) => new StatusRegister(C, Z, N, V, S, H, t, I);
		public StatusRegister WithInterrupt(bool i) => new StatusRegister(C, Z, N, V, S, H, T, i);
	}
}
