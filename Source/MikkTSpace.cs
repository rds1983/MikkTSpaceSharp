using System.Runtime.InteropServices;

namespace MikkTSpaceSharp
{
	public static partial class MikkTSpace
	{
		public struct SEdgeInternal
		{
			public int i0, i1, f;
		}


		[StructLayout(LayoutKind.Explicit)]
		public unsafe struct SEdge
		{
			[FieldOffset(0)]
			public SEdgeInternal i;

			[FieldOffset(0)]
			public fixed int array[3];
		}
	}
}
