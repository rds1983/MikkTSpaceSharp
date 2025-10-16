using System;

namespace MikkTSpaceSharp
{
	public struct SVec3
	{
		public float x;
		public float y;
		public float z;

		public SVec3(float xx, float yy, float zz)
		{
			x = xx;
			y = yy;
			z = zz;
		}

		public override string ToString() => $"{x}, {y}, {z}";

		public float LengthSquared()
		{
			return x * x + y * y + z * z;
		}

		public float Length()
		{
			return (float)Math.Sqrt(LengthSquared());
		}

		public bool VNotZero()
		{
			return x.NotZero() || y.NotZero() || z.NotZero();
		}

		public void Normalize()
		{
			if (!VNotZero())
			{
				return;
			}

			var scale = 1.0f / Length();

			x *= scale;
			y *= scale;
			z *= scale;
		}

		public static bool operator ==(SVec3 v1, SVec3 v2)
		{
			return (v1.x == v2.x) && (v1.y == v2.y) && (v1.z == v2.z);
		}

		public static bool operator !=(SVec3 v1, SVec3 v2)
		{
			return !(v1 == v2);
		}

		public static SVec3 operator +(SVec3 v1, SVec3 v2)
		{
			return new SVec3(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
		}

		public static SVec3 operator -(SVec3 v1, SVec3 v2)
		{
			return new SVec3(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
		}

		public static SVec3 operator *(SVec3 v, float scale)
		{
			return new SVec3(v.x * scale, v.y * scale, v.z * scale);
		}

		public static SVec3 operator *(float scale, SVec3 v)
		{
			return new SVec3(v.x * scale, v.y * scale, v.z * scale);
		}

		public static float Dot(SVec3 v1, SVec3 v2)
		{
			return v1.x * v2.x + v1.y * v2.y + v1.z * v2.z;
		}

		public override bool Equals(object obj)
		{
			return obj is SVec3 vec && (this == vec);
		}

		public override int GetHashCode()
		{
			int hashCode = 373119288;
			hashCode = hashCode * -1521134295 + x.GetHashCode();
			hashCode = hashCode * -1521134295 + y.GetHashCode();
			hashCode = hashCode * -1521134295 + z.GetHashCode();
			return hashCode;
		}
	}
}
