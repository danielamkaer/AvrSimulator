using System;
using AvrSim;
using AvrSim.Instructions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AvrSimTests
{
	[TestClass]
	public class ArithmetricTests
	{
		public RegisterFile InitialRegisterFile => new RegisterFile(new byte[32], new StatusRegister(), 0, 0);

		[DataTestMethod]
		[DataRow((byte)3, (byte)5)]
		[DataRow((byte)100, (byte)150)]
		[DataRow((byte)128, (byte)128)]
		public void AddRegisters(byte a, byte b)
		{
			byte c = (byte)(a + b);

			var registerFile = InitialRegisterFile.WithRegister(0, a).WithRegister(1, b);

			registerFile = Arithmetic.Add(registerFile, 0, 1, new object[] { false });

			Assert.AreEqual(a + b > 255, registerFile.StatusRegister.C, "Carry");
			Assert.AreEqual(c == 0, registerFile.StatusRegister.Z, "Zero");
			Assert.AreEqual((a & 0xf) + (b & 0xf) > 0xf, registerFile.StatusRegister.H, "Half carry");
			Assert.AreEqual((a >= 128 && b >= 128 && c < 128) || (a < 128 && b < 128 && c >= 128), registerFile.StatusRegister.V, "Two's complement overflow");
			Assert.AreEqual(registerFile.StatusRegister.N ^ registerFile.StatusRegister.V, registerFile.StatusRegister.S, "Sign");
			Assert.AreEqual(c >= 128, registerFile.StatusRegister.N, "Negative");
			Assert.AreEqual(c, registerFile[0], "Result");
		}

		[DataTestMethod]
		[DataRow((byte)3, (byte)5)]
		[DataRow((byte)100, (byte)150)]
		[DataRow((byte)128, (byte)128)]
		public void AddRegistersWithCarry(byte a, byte b)
		{
			var registerFile = InitialRegisterFile.WithRegister(0, a).WithRegister(1, b).WithStatusRegister(sr => sr.WithCarry(true));

			byte c = (byte)(a + b + 1);

			registerFile = Arithmetic.Add(registerFile, 0, 1, new object[] { true });

			Assert.AreEqual(a + b + 1 > 255, registerFile.StatusRegister.C, "Carry");
			Assert.AreEqual(c == 0, registerFile.StatusRegister.Z, "Zero");
			Assert.AreEqual((a & 0xf) + (b & 0xf) > 0xf, registerFile.StatusRegister.H, "Half carry");
			Assert.AreEqual((a >= 128 && b >= 128 && c < 128) || (a < 128 && b < 128 && c >= 128), registerFile.StatusRegister.V, "Two's complement overflow");
			Assert.AreEqual(registerFile.StatusRegister.N ^ registerFile.StatusRegister.V, registerFile.StatusRegister.S, "Sign");
			Assert.AreEqual(c >= 128, registerFile.StatusRegister.N, "Negative");
			Assert.AreEqual(c, registerFile[0], "Result");
		}
	}
}
