using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvrSim
{
	public class IndexedProperty<T, TIndex>
	{
		readonly Func<TIndex, T> get;
		readonly Action<TIndex, T> set;

		public IndexedProperty(Func<TIndex, T> get, Action<TIndex, T> set)
		{
			this.get = get;
			this.set = set;
		}
		public T this[TIndex index] {
			get => get(index);
			set => set(index, value);
		}
	}
}
