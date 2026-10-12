using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Hebron.Runtime
{
	public static unsafe class CRuntime
	{
        [ThreadStatic]
        private static AllocationScope _scope;

        internal static IDisposable BeginAllocationScope() => new AllocationScope();

        private sealed class AllocationScope : IDisposable
        {
            internal readonly AllocationScope Previous;
            internal readonly List<IntPtr> Pointers = new List<IntPtr>();

            internal AllocationScope() { Previous = _scope; _scope = this; }

            public void Dispose()
            {
                _scope = Previous;
                foreach (var pointer in Pointers)
                {
                    Marshal.FreeHGlobal(pointer);
                    MikkTSpaceSharpMemoryStats.Freed();
                }
                Pointers.Clear();
            }
        }

		public static void* malloc(ulong size)
		{
			return malloc(checked((long)size));
		}

		public static void* malloc(long size)
		{
			if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
            var ptr = Marshal.AllocHGlobal(checked((IntPtr)size));
            try { _scope?.Pointers.Add(ptr); }
            catch { Marshal.FreeHGlobal(ptr); throw; }

			MikkTSpaceSharpMemoryStats.Allocated();

			return ptr.ToPointer();
		}

		public static void free(void* a)
		{
			if (a == null)
				return;

			var ptr = new IntPtr(a);
			for (var scope = _scope; scope != null; scope = scope.Previous)
                if (scope.Pointers.Remove(ptr)) break;
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