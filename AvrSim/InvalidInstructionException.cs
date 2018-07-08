using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	public class InvalidInstructionException : Exception
	{
		public InvalidInstructionException()
		{
		}

		public InvalidInstructionException(string message) : base(message)
		{
		}

		public InvalidInstructionException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidInstructionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
