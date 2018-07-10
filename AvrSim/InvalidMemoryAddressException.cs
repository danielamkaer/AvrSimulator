using System;
using System.Runtime.Serialization;

namespace AvrSim
{
	public class InvalidMemoryAddressException : Exception
	{
		public InvalidMemoryAddressException()
		{
		}

		public InvalidMemoryAddressException(string message) : base(message)
		{
		}

		public InvalidMemoryAddressException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidMemoryAddressException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
