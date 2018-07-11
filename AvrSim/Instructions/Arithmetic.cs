using System;
namespace AvrSim.Instructions
{
	public static class Arithmetic
	{
		#region Add
		[InstructionHandler("0000_11rd_dddd_rrrr", false, Name = "Add")]
		[InstructionHandler("0001_11rd_dddd_rrrr", true, Name = "Adc")]
		public static RegisterFile Add(RegisterFile registerFile, byte d, byte r, object[] arguments)
		{
			var carry = (bool)arguments[0];

			var Rd = registerFile[d];
			var Rr = registerFile[r];

			var R = (byte)(Rd + Rr + (byte)((carry && registerFile.StatusRegister.C) ? 1 : 0));

			var statusRegister = registerFile.StatusRegister
											 .WithHalfCarry((Rd.BitIsSet(3) & Rr.BitIsSet(3)) | (Rr.BitIsSet(3) & R.BitIsCleared(3)) | (R.BitIsCleared(3) & Rd.BitIsSet(3)))
											 .WithTwosComplementOverflow((Rd.BitIsSet(7) & Rr.BitIsSet(7) & R.BitIsCleared(7)) | (Rd.BitIsCleared(7) & Rr.BitIsCleared(7) & R.BitIsSet(7)))
											 .WithNegative(R.BitIsSet(7))
											 .WithZero(R == 0)
											 .WithCarry((Rd.BitIsSet(7) & Rr.BitIsSet(7)) | (Rr.BitIsSet(7) & R.BitIsCleared(7)) | (R.BitIsCleared(7) & Rd.BitIsSet(7)));
			statusRegister = statusRegister.WithSigned(statusRegister.N ^ statusRegister.V);

			return registerFile.WithRegister(d, R).WithStatusRegister(statusRegister);
		}

		[InstructionHandler("1001_0110_KKdd_KKKK", Name = "Adiw")]
		public static RegisterFile Adiw(RegisterFile registerFile, byte K, byte d)
		{
			d = (byte)(24 + 2 * d);

			var Rdh = registerFile[d + 1];
			var Rdl = registerFile[d];
			ushort R = (ushort)(registerFile.GetWide(d) + K);

			var statusRegister = registerFile.StatusRegister
											 .WithTwosComplementOverflow(Rdh.BitIsCleared(7) & R.BitIsSet(15))
											 .WithNegative(R.BitIsSet(15))
											 .WithZero(R == 0)
											 .WithCarry(R.BitIsCleared(15) & Rdh.BitIsSet(7));
			statusRegister = statusRegister.WithSigned(statusRegister.N & statusRegister.V);

			return registerFile.WithWide(d, R)
							   .WithStatusRegister(statusRegister);
		}
		#endregion

		#region Subtract
		[InstructionHandler("0101_KKKK_dddd_KKKK", false, Name = "Subi")]
		[InstructionHandler("0100_KKKK_dddd_KKKK", true, Name = "Sbci")]
		public static RegisterFile Subi(RegisterFile registerFile, byte K, byte d, object[] arguments)
		{
			var carry = (bool)arguments[0];

			d = (byte)(d + 16);

			var Rd = registerFile[d];
			var R = (byte)(Rd - K - (carry && registerFile.StatusRegister.C ? 1 : 0));

			var statusRegister = registerFile.StatusRegister
											 .WithHalfCarry((Rd.BitIsCleared(3) & K.BitIsSet(3)) | (K.BitIsSet(3) & R.BitIsSet(3)) | (R.BitIsSet(3) & Rd.BitIsCleared(3)))
											 .WithTwosComplementOverflow((Rd.BitIsSet(7) & K.BitIsCleared(7) & R.BitIsCleared(7)) | (Rd.BitIsCleared(7) & K.BitIsSet(7) & R.BitIsSet(7)))
											 .WithNegative(R.BitIsSet(7))
											 .WithZero(R == 0 & (registerFile.StatusRegister.Z | !carry))
											 .WithCarry((Rd.BitIsCleared(7) & K.BitIsSet(7)) | (K.BitIsSet(7) & R.BitIsSet(7)) | (R.BitIsSet(7) & Rd.BitIsCleared(7)));
			statusRegister = statusRegister.WithSigned(statusRegister.N ^ statusRegister.V);

			return registerFile.WithRegister(d, R).WithStatusRegister(statusRegister);
		}
		#endregion
	}
}
