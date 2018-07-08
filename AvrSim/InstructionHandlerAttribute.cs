using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class InstructionHandlerAttribute : Attribute
	{
		public InstructionHandlerAttribute(string pattern)
		{
			Pattern = pattern;
		}

		public string Pattern { get; }
	}
}
