using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{

	public class Core
	{
		const byte R_X = 26;
		const byte R_XL = 26;
		const byte R_XH = 27;
		const byte R_Y = 28;
		const byte R_YL = 28;
		const byte R_YH = 29;
		const byte R_Z = 30;
		const byte R_ZL = 30;
		const byte R_ZH = 31;

		public ushort GetRegisterPair(byte register)
		{
			register = (byte)(register & ~(1));
			return (ushort)(Registers[register + 1] << 8 | Registers[register]);
		}

		public void SetRegisterPair(byte register, ushort value)
		{
			register = (byte)(register & ~(1));
			Registers[register + 1] = (byte)(value >> 8);
			Registers[register] = (byte)(value & 0xFF);
		}

		public ushort RegisterX
		{
			get => GetRegisterPair(R_X);
			set => SetRegisterPair(R_X, value);
		}

		public ushort RegisterY
		{
			get => GetRegisterPair(R_Y);
			set => SetRegisterPair(R_Y, value);
		}

		public ushort RegisterZ
		{
			get => GetRegisterPair(R_Z);
			set => SetRegisterPair(R_Z, value);
		}

		public StatusRegister StatusRegister { get; } = new StatusRegister();

		public byte[] Registers = new byte[32];

		public Stack Stack { get; }

		public Flash ProgramMemory { get; set; }

		public MemoryBus MemoryBus { get; set; }

		public uint ProgramCounter { get; set; } = 0;

		OpcodeDecoder OpcodeDecoder { get; } = new OpcodeDecoder();

		delegate void InstructionDelegate(Instruction instruction);
		Dictionary<string, InstructionDelegate> InstructionHandlers { get; } = new Dictionary<string, InstructionDelegate>();

		public void Run()
		{
			while (true)
			{
				var currentProgramCounter = ProgramCounter;
				ushort instructionWord = ProgramMemory.GetInstruction(ProgramCounter);

				if (!OpcodeDecoder.TryDecode(instructionWord, out var instruction))
				{
					ushort nextInstructionWord = ProgramMemory.GetInstruction(++ProgramCounter);

					instruction = OpcodeDecoder.DecodeWide(instructionWord, nextInstructionWord);
				}

				instruction.Address = (ushort)currentProgramCounter;
				Console.WriteLine($"{instruction.Address * 2:X4}:    {"",-12}    {instruction.Opcode.ToString(),-5}   ");
				ExecuteInstruction(instruction);

				ProgramCounter++;
			}
		}

		private void ExecuteInstruction(Instruction instruction)
		{
			InstructionHandlers[instruction.Opcode](instruction);
		}

		[InstructionHandler("1001_0101_1001_1000")]
		private static void Break(Instruction instruction)
		{
			Debugger.Break();
		}

		[InstructionHandler("0010_00rd_dddd_rrrr")]
		private void And(Instruction instruction)
		{
			var R = (byte)(Registers[instruction['d']] & Registers[instruction['r']]);
			Registers[instruction['d']] = R;

			StatusRegister.V = false;
			StatusRegister.N = R.BitIsSet(7);
			StatusRegister.Z = R == 0;
			StatusRegister.S = StatusRegister.N ^ StatusRegister.V;
		}

		[InstructionHandler("1001_0110_KKdd_KKKK")]
		private void Adiw(Instruction instruction)
		{
			var register = (byte)(24 + 2 * instruction['d']);

			var Rdh = Registers[register + 1];
			var Rdl = Registers[register];
			ushort R = (ushort)(GetRegisterPair(register) + instruction['K']);
			SetRegisterPair(register, R);

			StatusRegister.V = Rdh.BitIsCleared(7) & R.BitIsSet(15);
			StatusRegister.N = R.BitIsSet(15);
			StatusRegister.Z = R == 0;
			StatusRegister.C = R.BitIsCleared(15) & Rdh.BitIsSet(7);
			StatusRegister.S = StatusRegister.N ^ StatusRegister.V;
		}

		[InstructionHandler("1001_000d_dddd_0101")]
		private void Lpm_PostIncrement(Instruction instruction)
		{
			var word = (uint)(RegisterZ >> 1);
			var MSB = (RegisterZ & 1) != 0;

			var data = ProgramMemory.GetInstruction(word);

			byte value;
			if (MSB)
			{
				value = (byte)(data >> 8);
			}
			else
			{
				value = (byte)(data & 0xff);
			}

			Registers[instruction['d']] = value;
			RegisterZ++;
		}

		[InstructionHandler("1001_000d_dddd_0100")]
		private void Lpm(Instruction instruction)
		{
			var word = (uint)(RegisterZ >> 1);
			var MSB = (RegisterZ & 1) != 0;

			var data = ProgramMemory.GetInstruction(word);

			byte value;
			if (MSB)
			{
				value = (byte)(data >> 8);
			}
			else
			{
				value = (byte)(data & 0xff);
			}

			Registers[instruction['d']] = value;
		}

		[InstructionHandler("1001_0101_1100_1000")]
		private void Lpm_R0(Instruction instruction)
		{
			var word = (uint)(RegisterZ >> 1);
			var MSB = (RegisterZ & 1) != 0;

			var data = ProgramMemory.GetInstruction(word);

			byte value;
			if (MSB)
			{
				value = (byte)(data >> 8);
			}
			else
			{
				value = (byte)(data & 0xff);
			}

			Registers[0] = value;
		}

		[InstructionHandler("1111_01kk_kkkk_k001")]
		private void Brne(Instruction instruction)
		{
			if (!StatusRegister.Z)
			{
				ProgramCounter = (uint)(ProgramCounter + instruction['k'].ToSigned(7));
			}
		}

		[InstructionHandler("0000_01rd_dddd_rrrr")]
		private void Cpc(Instruction instruction)
		{
			var Rd = Registers[instruction['d']];
			var Rr = Registers[instruction['r']];
			var R = (byte)(Rd - Rr - (StatusRegister.C ? 1 : 0));

			StatusRegister.H = (Rd.BitIsSet(3) & Rr.BitIsSet(3)) | (Rr.BitIsSet(3) & R.BitIsCleared(3)) | (R.BitIsCleared(3) & Rd.BitIsSet(3));
			StatusRegister.V = (Rd.BitIsSet(7) & Rr.BitIsSet(7) & R.BitIsCleared(7)) | (Rd.BitIsCleared(7) & Rr.BitIsCleared(7) & R.BitIsSet(7));
			StatusRegister.N = R.BitIsSet(7);
			StatusRegister.Z = R == 0 & StatusRegister.Z;
			StatusRegister.C = (Rd.BitIsSet(7) & Rr.BitIsSet(7)) | (Rr.BitIsSet(7) & R.BitIsCleared(7)) | (R.BitIsCleared(7) & Rd.BitIsSet(7));
			StatusRegister.S = StatusRegister.N ^ StatusRegister.V;
		}

		[InstructionHandler("0011_KKKK_dddd_KKKK")]
		private void Cpi(Instruction instruction)
		{
			var Rd = Registers[16 + instruction['d']];
			var K = (byte)instruction['K'];
			var R = (byte)(Rd - K);

			StatusRegister.H = (Rd.BitIsSet(3) & K.BitIsSet(3)) | (K.BitIsSet(3) & R.BitIsCleared(3)) | (R.BitIsCleared(3) & Rd.BitIsSet(3));
			StatusRegister.V = (Rd.BitIsSet(7) & K.BitIsSet(7) & R.BitIsCleared(7)) | (Rd.BitIsCleared(7) & K.BitIsCleared(7) & R.BitIsSet(7));
			StatusRegister.N = R.BitIsSet(7);
			StatusRegister.Z = R == 0;
			StatusRegister.C = (Rd.BitIsSet(7) & K.BitIsSet(7)) | (K.BitIsSet(7) & R.BitIsCleared(7)) | (R.BitIsCleared(7) & Rd.BitIsSet(7));
			StatusRegister.S = StatusRegister.N ^ StatusRegister.V;
		}

		[InstructionHandler("1100_kkkk_kkkk_kkkk")]
		private void Rjmp(Instruction instruction)
		{
			ProgramCounter = (uint)(ProgramCounter + instruction['k'].ToSigned(12));
		}

		[InstructionHandler("1101_kkkk_kkkk_kkkk")]
		private void Rcall(Instruction instruction)
		{
			Stack.PushWide((ushort)ProgramCounter);
			ProgramCounter += instruction['k'];
		}

		[InstructionHandler("0001_11rd_dddd_rrrr")]
		private void Adc(Instruction instruction)
		{
			Add(instruction, true);
		}

		[InstructionHandler("0000_11rd_dddd_rrrr")]
		private void Add(Instruction instruction)
		{
			Add(instruction, false);
		}

		[InstructionHandler("10q0_qq0d_dddd_0qqq")]
		private void Ldd_Z(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load((ushort)(RegisterZ + instruction['q']));
		}

		[InstructionHandler("1001_000d_dddd_0001")]
		private void Ld_Z_PostIncrement(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load(RegisterY++);
		}

		[InstructionHandler("1001_000d_dddd_0010")]
		private void Ld_Z_PreDecrement(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load(--RegisterY);
		}

		[InstructionHandler("1000_000d_dddd_0000")]
		private void Ld_Z(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load(RegisterZ);
		}

		[InstructionHandler("10q0_qq0d_dddd_1qqq")]
		private void Ldd_Y(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load((ushort)(RegisterY + instruction['q']));
		}

		[InstructionHandler("1001_000d_dddd_1001")]
		private void Ld_Y_PostIncrement(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load(RegisterY++);
		}

		[InstructionHandler("1001_000d_dddd_1010")]
		private void Ld_Y_PreDecrement(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load(--RegisterY);
		}

		[InstructionHandler("1000_000d_dddd_1000")]
		private void Ld_Y(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load(RegisterY);
		}

		[InstructionHandler("1001_000d_dddd_1101")]
		private void Ld_X_PostIncrement(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load(RegisterX++);
		}

		[InstructionHandler("1001_000d_dddd_1110")]
		private void Ld_X_PreDecrement(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load(--RegisterX);
		}

		[InstructionHandler("1001_000d_dddd_1100")]
		private void Ld_X(Instruction instruction)
		{
			Registers[instruction['d']] = MemoryBus.Load(RegisterX);
		}

		[InstructionHandler("10q0_qq1r_rrrr_0qqq")]
		private void Std_Z(Instruction instruction)
		{
			MemoryBus.Store((ushort)(RegisterZ + instruction['q']), Registers[instruction['r']]);
		}

		[InstructionHandler("1001_001r_rrrr_0001")]
		private void St_Z_PostIncrement(Instruction instruction)
		{
			MemoryBus.Store(RegisterZ++, Registers[instruction['r']]);
		}

		[InstructionHandler("1001_001r_rrrr_0010")]
		private void St_Z_PreDecrement(Instruction instruction)
		{
			MemoryBus.Store(--RegisterZ, Registers[instruction['r']]);
		}

		[InstructionHandler("1000_001r_rrrr_0000")]
		private void St_Z(Instruction instruction)
		{
			MemoryBus.Store(RegisterZ, Registers[instruction['r']]);
		}

		[InstructionHandler("10q0_qq1r_rrrr_1qqq")]
		private void Std_Y(Instruction instruction)
		{
			MemoryBus.Store((ushort)(RegisterY + instruction['q']), Registers[instruction['r']]);
		}

		[InstructionHandler("1001_001r_rrrr_1001")]
		private void St_Y_PostIncrement(Instruction instruction)
		{
			MemoryBus.Store(RegisterY++, Registers[instruction['r']]);
		}

		[InstructionHandler("1001_001r_rrrr_1010")]
		private void St_Y_PreDecrement(Instruction instruction)
		{
			MemoryBus.Store(--RegisterY, Registers[instruction['r']]);
		}

		[InstructionHandler("1000_001r_rrrr_1000")]
		private void St_Y(Instruction instruction)
		{
			MemoryBus.Store(RegisterY, Registers[instruction['r']]);
		}

		[InstructionHandler("1001_001r_rrrr_1101")]
		private void St_X_PostIncrement(Instruction instruction)
		{
			MemoryBus.Store(RegisterX++, Registers[instruction['r']]);
		}

		[InstructionHandler("1001_001r_rrrr_1110")]
		private void St_X_PreDecrement(Instruction instruction)
		{
			MemoryBus.Store(--RegisterX, Registers[instruction['r']]);
		}

		[InstructionHandler("1001_001r_rrrr_1100")]
		private void St_X(Instruction instruction)
		{
			MemoryBus.Store(RegisterX, Registers[instruction['r']]);
		}

		[InstructionHandler("0010_11rd_dddd_rrrr")]
		private void Mov(Instruction instruction)
		{
			Registers[instruction['d']] = Registers[instruction['r']];
		}

		[InstructionHandler("1011_0AAd_dddd_AAAA")]
		private void In(Instruction instruction)
		{
			throw new NotImplementedException();
		}

		[InstructionHandler("1001_000d_dddd_1111")]
		private void Pop(Instruction instruction)
		{
			Registers[instruction['d']] = Stack.Pop();
		}

		[InstructionHandler("1001_001d_dddd_1111")]
		private void Push(Instruction instruction)
		{
			Stack.Push(Registers[instruction['d']]);
		}

		[InstructionHandler("1001_0101_0000_1000")]
		private void Ret(Instruction instruction)
		{
			ProgramCounter = Stack.PopWide();
		}

		[InstructionHandler("0000_0000_0000_0000")]
		private void Nop(Instruction instruction)
		{

		}

		[InstructionHandler("1001_001d_dddd_0000_kkkk_kkkk_kkkk_kkkk")]
		private void Sts(Instruction instruction)
		{
			MemoryBus.Store((ushort)instruction['k'], Registers[instruction['d']]);
		}

		[InstructionHandler("1110_KKKK_dddd_KKKK")]
		private void Ldi(Instruction instruction)
		{
			Registers[16 + instruction['d']] = (byte)instruction['K'];
		}

		private void Add(Instruction instruction, bool withCarry)
		{
			var Rd = Registers[instruction['d']];
			var Rr = Registers[instruction['r']];

			var R = (byte)(Rd + Rr + (byte)((withCarry && StatusRegister.C) ? 1 : 0));

			StatusRegister.H = (Rd.BitIsSet(3) & Rr.BitIsSet(3)) | (Rr.BitIsSet(3) & R.BitIsCleared(3)) | (R.BitIsCleared(3) & Rd.BitIsSet(3));
			StatusRegister.V = (Rd.BitIsSet(7) & Rr.BitIsSet(7) & R.BitIsCleared(7)) | (Rd.BitIsCleared(7) & Rr.BitIsCleared(7) & R.BitIsSet(7));
			StatusRegister.N = R.BitIsSet(7);
			StatusRegister.Z = R == 0;
			StatusRegister.C = (Rd.BitIsSet(7) & Rr.BitIsSet(7)) | (Rr.BitIsSet(7) & R.BitIsCleared(7)) | (R.BitIsCleared(7) & Rd.BitIsSet(7));
			StatusRegister.S = StatusRegister.N ^ StatusRegister.V;

			Registers[instruction['d']] = R;
		}

		public Core(Flash programMemory, Ram ram) : this()
		{
			MemoryBus = new MemoryBus();
			MemoryBus.AddMap(0, 32, new RegisterMemory(this));
			MemoryBus.AddMap(0x60, ram.Size, ram);
			Stack = new Stack(MemoryBus, (ushort)(0x60 + ram.Size - 1));
			ProgramMemory = programMemory;
		}

		public class Handler
		{
			public MethodInfo MethodInfo { get; set; }
		}

		Core()
		{
			var methods = from m in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
						  let attr = m.GetCustomAttributes(typeof(InstructionHandlerAttribute), false)
						  where attr.Length == 1
						  select new { MethodInfo = m, InstructionHandlerAttribute = (InstructionHandlerAttribute)attr.First() };

			List<Handler> Instructions = new List<Handler>();

			foreach (var method in methods)
			{
				Instructions.Add(new Handler
				{
					MethodInfo = method.MethodInfo
				});
				OpcodeDecoder.AddDecoder(method.MethodInfo.Name, method.InstructionHandlerAttribute.Pattern);
				InstructionHandlers.Add(method.MethodInfo.Name, (instruction) =>
				{
					Instructions.Single(h => h.MethodInfo.Name == method.MethodInfo.Name).MethodInfo.Invoke(this, new object[] { instruction });
				});
			}
		}

	}
}
