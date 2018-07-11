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
				Console.Error.WriteLine($"{instruction.Address * 2:X4}:    {BitConverter.ToString(instruction.Bytes).Replace('-',' '),-12}    {instruction.Opcode,-5}   ");
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
			public string Name { get; set; }
			public MethodInfo MethodInfo { get; set; }
		}

		Core()
		{
			var methods = Assembly.GetExecutingAssembly().GetTypes()
							  .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Public))
			                  .SelectMany(m => m.GetCustomAttributes(typeof(InstructionHandlerAttribute), false), (m,a) => new {MethodInfo = m, InstructionHandlerAttribute = (InstructionHandlerAttribute)a});

			List<Handler> Instructions = new List<Handler>();

			foreach (var method in methods)
			{
				var name = method.MethodInfo.Name + (method.InstructionHandlerAttribute.Arguments.Length == 0 ? string.Empty : ("_" + string.Join("-", method.InstructionHandlerAttribute.Arguments)));

				if (method.MethodInfo.ReturnType != typeof(RegisterFile))
				{
					throw new Exception($"Method should return a {typeof(RegisterFile)}");
				}

				Instructions.Add(new Handler
				{
					Name = name,
					MethodInfo = method.MethodInfo
				});
				OpcodeDecoder.AddDecoder(name, method.InstructionHandlerAttribute.Pattern);
				InstructionHandlers.Add(name, (instruction, registerFile) =>
				{
					//var handler = Instructions.Single(h => );
					var methodInfo = method.MethodInfo;

					var parameters = methodInfo.GetParameters().Select(p => MapParameter(p, instruction, registerFile, method.InstructionHandlerAttribute.Arguments)).ToArray();

					try
					{
						if (methodInfo.IsStatic) 
						{
							return (RegisterFile)methodInfo.Invoke(null, parameters);
						} else {
							return (RegisterFile)methodInfo.Invoke(this, parameters);
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

		object MapParameter(ParameterInfo parameterInfo, Instruction instruction, RegisterFile registerFile, string[] arguments)
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

			if (parameterInfo.ParameterType == typeof(string[]))
			{
				return arguments;
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

				if (parameterInfo.ParameterType == typeof(short))
				{
					return (short)instruction[name];
				}

				if (parameterInfo.ParameterType == typeof(int))
				{
					return (int)instruction[name];
				}

				if (parameterInfo.ParameterType == typeof(sbyte))
				{
					return (sbyte)instruction[name];
				}
			}

			throw new ArgumentException($"Unable to map parameter {parameterInfo.Name}");
		}
	}
}
