using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	public class InstructionValue
	{
		uint storage;
		byte numberOfBits;

		public InstructionValue()
		{
			storage = 0;
			numberOfBits = 0;
		}

		public void InsertLeastSignificantBit(bool bit)
		{
			numberOfBits++;
			storage = ((storage << 1) | (uint)(bit ? 1 : 0));
		}

		public int ToSigned()
		{
			return storage.ToSigned(numberOfBits);
		}

		public static implicit operator uint(InstructionValue instructionValue)
		{
			return instructionValue.storage;
		}

		public static explicit operator int(InstructionValue instructionValue)
		{
			return instructionValue.ToSigned();
		}

		public static explicit operator short(InstructionValue instructionValue)
		{
			return (short)instructionValue.ToSigned();
		}

		public static explicit operator sbyte(InstructionValue instructionValue)
		{
			return (sbyte)instructionValue.ToSigned();
		}

		public static explicit operator ushort(InstructionValue instructionValue)
		{
			return (ushort) instructionValue.storage;
		}

		public static explicit operator byte(InstructionValue instructionValue)
		{
			return (byte)instructionValue.storage;
		}
	}

	public class OpcodeDecoder
	{
		public List<Decoder> Decoders { get; }

		public OpcodeDecoder()
		{
			Decoders = new List<Decoder>();
		}

		public void AddDecoder(string opcode, string pattern)
		{
			Decoders.Add(new Decoder
			{
				Opcode = opcode,
				Pattern = pattern
			});
		}

		bool BitIsSet(uint word, int bit)
		{
			return (word & (1 << bit)) != 0;
		}

		bool TryMatch(string pattern, uint instruction, int instructionWidth, out Dictionary<char, InstructionValue> values)
		{
			var position = instructionWidth - 1;

			values = new Dictionary<char, InstructionValue>();

			foreach (var chr in pattern)
			{
				switch (chr)
				{
					case '_':
						break;
					case '1':
						if (!BitIsSet(instruction, position--))
						{
							return false;
						}
						break;
					case '0':
						if (BitIsSet(instruction, position--))
						{
							return false;
						}
						break;
					case '?':
						position--;
						break;
					case char c when (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'):
						var bitValue = BitIsSet(instruction, position--);

						if (!values.ContainsKey(c))
						{
							values[c] = new InstructionValue();
						}

						values[c].InsertLeastSignificantBit(bitValue);
						break;
					default:
						throw new Exception($"Unknown character in pattern {chr}");
				}
			}

			return true;
		}

		public Instruction Decode(ushort instructionWord)
		{
			foreach (var decoder in Decoders)
			{
				if (TryMatch(decoder.Pattern, instructionWord, 16, out var values))
				{
					return new Instruction
					{
						Opcode = decoder.Opcode,
						Bytes = BitConverter.GetBytes(instructionWord).Reverse().ToArray(),
						Values = values
					};
				}
			}

			throw new InvalidInstructionException();
		}

		public bool TryDecode(ushort instructionWord, out Instruction instruction)
		{
			try
			{
				instruction = Decode(instructionWord);
				return true;
			}
			catch (InvalidInstructionException)
			{
				instruction = null;
				return false;
			}
		}

		public Instruction DecodeWide(ushort instructionWord1, ushort instructionWord2)
		{
			var wideInstruction = (uint)(instructionWord1 << 16 | instructionWord2);
			foreach (var decoder in Decoders)
			{
				if (TryMatch(decoder.Pattern, wideInstruction, 32, out var values))
				{
					return new Instruction
					{
						Opcode = decoder.Opcode,
						Bytes = BitConverter.GetBytes(wideInstruction).Reverse().ToArray(),
						Values = values
					};
				}
			}

			throw new InvalidInstructionException();
		}


		public bool TryDecodeWide(ushort instructionWord1, ushort instructionWord2, out Instruction instruction)
		{
			try
			{
				instruction = DecodeWide(instructionWord1, instructionWord2);
				return true;
			}
			catch (InvalidInstructionException)
			{
				instruction = null;
				return false;
			}
		}
	}
}
