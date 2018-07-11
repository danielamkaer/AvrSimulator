using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class InstructionHandlerAttribute : Attribute
	{
		public object[] Arguments { get; }

		public InstructionHandlerAttribute(string pattern, params object[] arguments)
		{
			Arguments = arguments;
			Pattern = pattern;
		}

		public string Pattern { get; }

		public string Name { get; set; }
	}
}
