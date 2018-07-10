using System;
namespace AvrSim.Instructions
{
	public static class Test
	{
		[InstructionHandler("0010_00rd_dddd_rrrr")]
		public static RegisterFile And(RegisterFile registerFile, byte r, byte d)
		{
			var R = (byte)(registerFile[d] & registerFile[r]);

			var statusRegister = registerFile.StatusRegister
											 .WithTwosComplementOverflow(false)
											 .WithNegative(r.BitIsSet(7))
											 .WithZero(R == 0);
			statusRegister = statusRegister.WithSigned(statusRegister.N & statusRegister.V);

			return registerFile.WithRegister(d, R).WithStatusRegister(statusRegister);
		}

		[InstructionHandler("0111_KKKK_dddd_KKKK")]
		public static RegisterFile Andi(RegisterFile registerFile, byte K, byte d)
		{
			throw new NotImplementedException();
		}

		[InstructionHandler("0000_01rd_dddd_rrrr")]
		public static RegisterFile Cpc(RegisterFile registerFile, byte d, byte r)
		{
			return Cp(registerFile, d, r, true);
		}

		static RegisterFile Cp(RegisterFile registerFile, byte d, byte r, bool withCarry)
		{
			var Rd = registerFile[d];
			var Rr = registerFile[r];
			var R = (byte)(Rd - Rr - (withCarry && registerFile.StatusRegister.C ? 1 : 0));

			var statusRegister = registerFile.StatusRegister
											 .WithHalfCarry((Rd.BitIsSet(3) & Rr.BitIsSet(3)) | (Rr.BitIsSet(3) & R.BitIsCleared(3)) | (R.BitIsCleared(3) & Rd.BitIsSet(3)))
											 .WithTwosComplementOverflow((Rd.BitIsSet(7) & Rr.BitIsSet(7) & R.BitIsCleared(7)) | (Rd.BitIsCleared(7) & Rr.BitIsCleared(7) & R.BitIsSet(7)))
											 .WithNegative(R.BitIsSet(7))
											 .WithZero(R == 0 & (registerFile.StatusRegister.Z | !withCarry))
											 .WithCarry((Rd.BitIsSet(7) & Rr.BitIsSet(7)) | (Rr.BitIsSet(7) & R.BitIsCleared(7)) | (R.BitIsCleared(7) & Rd.BitIsSet(7)));
			statusRegister = statusRegister.WithSigned(statusRegister.N ^ statusRegister.V);

			return registerFile.WithStatusRegister(statusRegister);
		}

		[InstructionHandler("0011_KKKK_dddd_KKKK")]
		public static RegisterFile Cpi(RegisterFile registerFile, byte K, byte d)
		{
			return Cpi(registerFile, K, d, false);
		}

		static RegisterFile Cpi(RegisterFile registerFile, byte K, byte d, bool withCarry)
		{
			d += 16;

			var Rd = registerFile[d];
			var R = (byte)(Rd - K - (withCarry && registerFile.StatusRegister.C ? 1 : 0));

			var statusRegister = registerFile.StatusRegister
											 .WithHalfCarry((Rd.BitIsSet(3) & K.BitIsSet(3)) | (K.BitIsSet(3) & R.BitIsCleared(3)) | (R.BitIsCleared(3) & Rd.BitIsSet(3)))
											 .WithTwosComplementOverflow((Rd.BitIsSet(7) & K.BitIsSet(7) & R.BitIsCleared(7)) | (Rd.BitIsCleared(7) & K.BitIsCleared(7) & R.BitIsSet(7)))
											 .WithNegative(R.BitIsSet(7))
											 .WithZero(R == 0 & (registerFile.StatusRegister.Z | !withCarry))
											 .WithCarry((Rd.BitIsSet(7) & K.BitIsSet(7)) | (K.BitIsSet(7) & R.BitIsCleared(7)) | (R.BitIsCleared(7) & Rd.BitIsSet(7)));
			statusRegister = statusRegister.WithSigned(statusRegister.N ^ statusRegister.V);

			return registerFile.WithStatusRegister(statusRegister);
		}


		[InstructionHandler("0001_00rd_dddd_rrrr")]
		public static RegisterFile Cpse(RegisterFile registerFile, byte r, byte d)
		{
			if (registerFile[r] == registerFile[d])
			{
				return registerFile.WithProgramCounter(p => p + 1);
			}

			return registerFile;
		}
	}
}
