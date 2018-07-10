using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using AvrSim;

namespace AvrSim
{

	public class Core
	{
		public const byte R_X = 26;
		public const byte R_XL = 26;
		public const byte R_XH = 27;
		public const byte R_Y = 28;
		public const byte R_YL = 28;
		public const byte R_YH = 29;
		public const byte R_Z = 30;
		public const byte R_ZL = 30;
		public const byte R_ZH = 31;

		//public List<RegisterFile> RegisterFileStates { get; set; }

		public RegisterFile RegisterFile { get; set; }// => RegisterFileStates.Last();

		public Stack Stack { get; }

		public Flash ProgramMemory { get; set; }

		public MemoryBus MemoryBus { get; set; }

		public uint ProgramCounter { get; set; } = 0;

		OpcodeDecoder OpcodeDecoder { get; } = new OpcodeDecoder();

		delegate RegisterFile InstructionDelegate(Instruction instruction, RegisterFile registerFile);
		Dictionary<string, InstructionDelegate> InstructionHandlers { get; } = new Dictionary<string, InstructionDelegate>();

		public void Run()
		{
			RegisterFile = new RegisterFile(new byte[32], new StatusRegister(), 0, (ushort) (Stack.TopOfStack - 1));

			while (true)
			{
				var currentProgramCounter = RegisterFile.ProgramCounter;
				ushort instructionWord = ProgramMemory.GetInstruction(RegisterFile.ProgramCounter);

				if (!OpcodeDecoder.TryDecode(instructionWord, out var instruction))
				{
					RegisterFile = RegisterFile.WithProgramCounter(p => p + 1);
					ushort nextInstructionWord = ProgramMemory.GetInstruction(RegisterFile.ProgramCounter);

					instruction = OpcodeDecoder.DecodeWide(instructionWord, nextInstructionWord);
				}

				instruction.Address = (ushort)currentProgramCounter;
				Console.Error.WriteLine($"{instruction.Address * 2:X4}:    {"",-12}    {instruction.Opcode,-5}   ");
				ExecuteInstruction(instruction);
				Console.Error.WriteLine($"   {RegisterFile}");

				RegisterFile = RegisterFile.WithProgramCounter(p => p + 1);
			}
		}

		void ExecuteInstruction(Instruction instruction)
		{
			RegisterFile = InstructionHandlers[instruction.Opcode](instruction, RegisterFile);
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
			var methods = from m in Assembly.GetExecutingAssembly().GetTypes().SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
			//var methods = from m in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
						  let attr = m.GetCustomAttributes(typeof(InstructionHandlerAttribute), false)
						  where attr.Length == 1
						  select new { MethodInfo = m, InstructionHandlerAttribute = (InstructionHandlerAttribute)attr.First() };

			List<Handler> Instructions = new List<Handler>();

			foreach (var method in methods)
			{
				if (method.MethodInfo.ReturnType != typeof(RegisterFile))
				{
					throw new Exception($"Method should return a {typeof(RegisterFile)}");
				}

				Instructions.Add(new Handler
				{
					MethodInfo = method.MethodInfo
				});
				OpcodeDecoder.AddDecoder(method.MethodInfo.Name, method.InstructionHandlerAttribute.Pattern);
				InstructionHandlers.Add(method.MethodInfo.Name, (instruction, registerFile) =>
				{
					var handler = Instructions.Single(h => h.MethodInfo.Name == method.MethodInfo.Name);

					var parameters = handler.MethodInfo.GetParameters().Select(p => MapParameter(p, instruction, registerFile)).ToArray();

					try
					{
						if (handler.MethodInfo.IsStatic) 
						{
							return (RegisterFile)handler.MethodInfo.Invoke(null, parameters);
						} else {
							return (RegisterFile)handler.MethodInfo.Invoke(this, parameters);
						}
					}
					catch (TargetInvocationException exception)
					{
						ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
						throw;
					}

				});
			}
		}

		object MapParameter(ParameterInfo parameterInfo, Instruction instruction, RegisterFile registerFile)
		{
			if (parameterInfo.ParameterType == typeof(Instruction))
			{
				return instruction;
			}

			if (parameterInfo.ParameterType == typeof(RegisterFile))
			{
				return registerFile;
			}

			if (parameterInfo.ParameterType == typeof(MemoryBus))
			{
				return MemoryBus;
			}

			if (parameterInfo.ParameterType == typeof(Flash))
			{
				return ProgramMemory;
			}

			if (parameterInfo.ParameterType == typeof(Stack))
			{
				return Stack;
			}

			if (parameterInfo.Name.Length == 1)
			{
				char name = parameterInfo.Name[0];

				if (!instruction.Values.ContainsKey(name))
				{
					throw new ArgumentException($"Instruction does not contain parameter {name}.");
				}

				if (parameterInfo.ParameterType == typeof(ushort))
				{
					return (ushort)instruction[name];
				}

				if (parameterInfo.ParameterType == typeof(uint))
				{
					return (uint)instruction[name];
				}

				if (parameterInfo.ParameterType == typeof(byte))
				{
					return (byte)instruction[name];
				}
			}

			throw new ArgumentException($"Unable to map parameter {parameterInfo.Name}");
		}
	}
}
