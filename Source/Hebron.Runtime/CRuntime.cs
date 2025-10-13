using System;
using System.Runtime.InteropServices;

namespace Hebron.Runtime
{
	public static unsafe class CRuntime
	{
		public static void* malloc(ulong size)
		{
			return malloc((long)size);
		}

		public static void* malloc(long size)
		{
			var ptr = Marshal.AllocHGlobal((int)size);

			MikkTSpaceSharpMemoryStats.Allocated();

			return ptr.ToPointer();
		}

		public static void free(void* a)
		{
			if (a == null)
				return;

			var ptr = new IntPtr(a);
			Marshal.FreeHGlobal(ptr);
			MikkTSpaceSharpMemoryStats.Freed();
		}

		public static void memcpy(void* a, void* b, long size)
		{
			var ap = (byte*)a;
			var bp = (byte*)b;
			for (long i = 0; i < size; ++i)
				*ap++ = *bp++;
		}

		public static void memcpy(void* a, void* b, ulong size)
		{
			memcpy(a, b, (long)size);
		}

		public static void memset(void* ptr, int value, long size)
		{
			var bptr = (byte*)ptr;
			var bval = (byte)value;
			for (long i = 0; i < size; ++i)
				*bptr++ = bval;
		}

		public static void memset(void* ptr, int value, ulong size)
		{
			memset(ptr, value, (long)size);
		}

		public static double cos(double r) => Math.Cos(r);

		public static double acos(double r) => Math.Acos(r);
		public static float fabsf(float f) => Math.Abs(f);

		public static float sqrtf(float f) => (float)Math.Sqrt(f);
	}
}