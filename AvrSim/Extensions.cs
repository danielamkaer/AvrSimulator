using System.Collections.ObjectModel;
using System.Linq;
using AvrSim;

namespace AvrSim
{
	public static class Extensions
	{
		public static T[] ToArray<T>(this ReadOnlyCollection<T> collection)
		{
			return (Enumerable.Range(0, collection.Count).Select(i => collection[i])).ToArray();
		}
	}
}
