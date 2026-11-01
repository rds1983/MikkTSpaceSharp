using System;

namespace MikkTSpaceSharp
{
	public static partial class MikkTSpace
	{
		public struct SEdge
		{
			public int i0, i1, f;

			public int this[int index]
			{
				get
				{
					switch(index)
					{
						case 0: return i0;
						case 1: return i1;
						case 2: return f;
						default: throw new IndexOutOfRangeException();
					}
				}

				set
				{
					switch (index)
					{
						case 0: i0 = value; break;
						case 1: i1 = value; break;
						case 2: f = value; break;
						default: throw new IndexOutOfRangeException();
					}
				}
			}
		}
	}
}
