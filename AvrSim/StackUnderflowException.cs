using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	public class StackUnderflowException : Exception
	{
		public StackUnderflowException()
		{
		}

		public StackUnderflowException(string message) : base(message)
		{
		}

		public StackUnderflowException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected StackUnderflowException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
