using AssetManagementBase;
using DigitalRiseModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework.Graphics;
using MikkTSpaceNativeWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Reflection;
using static MikkTSpaceSharp.MikkTSpace;

namespace MikkTSpaceSharp.Tests
{
	internal struct VertexElementData
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 UV;
		public Vector3 Tangent;
		public Vector3 BiTangent;
	}

	internal class VertexData
	{
		public VertexElementData[] Vertices;
		public uint[] Indices;

		public VertexData Clone()
		{
			var result = new VertexData
			{
				Vertices = new VertexElementData[Vertices.Length],
				Indices = new uint[Indices.Length]
			};

			for (var i = 0; i < Vertices.Length; ++i)
			{
				result.Vertices[i] = Vertices[i];
			}

			for (var i = 0; i < Indices.Length; ++i)
			{
				result.Indices[i] = Indices[i];
			}

			return result;
		}
	}


	internal static class Utility
	{
		/// <summary>
		/// The value for which all absolute numbers smaller than are considered equal to zero.
		/// </summary>
		public const float ZeroTolerance = 1e-6f;

		/// <summary>
		/// Compares two floating point numbers based on an epsilon zero tolerance.
		/// </summary>
		/// <param name="left">The first number to compare.</param>
		/// <param name="right">The second number to compare.</param>
		/// <param name="epsilon">The epsilon value to use for zero tolerance.</param>
		/// <returns><c>true</c> if <paramref name="left"/> is within epsilon of <paramref name="right"/>; otherwise, <c>false</c>.</returns>
		public static bool EpsilonEquals(this float left, float right, float epsilon = ZeroTolerance)
		{
			return Math.Abs(left - right) <= epsilon;
		}

		public static string ExecutingAssemblyDirectory
		{
			get
			{
				string codeBase = Assembly.GetExecutingAssembly().Location;
				UriBuilder uri = new UriBuilder(codeBase);
				string path = Uri.UnescapeDataString(uri.Path);
				return Path.GetDirectoryName(path);
			}
		}

		public static AssetManager CreateAssetManager()
		{
			return AssetManager.CreateFileAssetManager(Path.Combine(Utility.ExecutingAssemblyDirectory, "Assets/Models"));
		}

		public static int GetSize(this VertexElementFormat elementFormat)
		{
			switch (elementFormat)
			{
				case VertexElementFormat.Single:
					return 4;
				case VertexElementFormat.Vector2:
					return 8;
				case VertexElementFormat.Vector3:
					return 12;
				case VertexElementFormat.Vector4:
					return 16;
				case VertexElementFormat.Color:
					return 4;
				case VertexElementFormat.Byte4:
					return 4;
				case VertexElementFormat.Short2:
					return 4;
				case VertexElementFormat.Short4:
					return 8;
				case VertexElementFormat.NormalizedShort2:
					return 4;
				case VertexElementFormat.NormalizedShort4:
					return 8;
				case VertexElementFormat.HalfVector2:
					return 4;
				case VertexElementFormat.HalfVector4:
					return 8;
			}

			throw new Exception($"Unknown vertex element format {elementFormat}");
		}


		public static int CalculateStride(this IEnumerable<VertexElement> elements)
		{
			var result = 0;

			foreach (var channel in elements)
			{
				result += channel.VertexElementFormat.GetSize();
			}

			return result;
		}

		public static int CalculateElementsCount(this IEnumerable<VertexElement> elements)
		{
			var result = 0;

			foreach (var channel in elements)
			{
				result += channel.VertexElementFormat.GetElementsCount();
			}

			return result;
		}

		public static int GetElementsCount(this VertexElementFormat elementFormat)
		{
			switch (elementFormat)
			{
				case VertexElementFormat.Single:
					return 1;
				case VertexElementFormat.Vector2:
					return 2;
				case VertexElementFormat.Vector3:
					return 3;
				case VertexElementFormat.Vector4:
					return 4;
				case VertexElementFormat.Color:
					return 4;
				case VertexElementFormat.Byte4:
					return 4;
				case VertexElementFormat.Short2:
					return 2;
				case VertexElementFormat.Short4:
					return 4;
			}

			throw new Exception($"Unknown vertex element format {elementFormat}");
		}

		public static int GetSize(this IndexElementSize indexType)
		{
			switch (indexType)
			{
				case IndexElementSize.SixteenBits:
					return 2;
				case IndexElementSize.ThirtyTwoBits:
					return 4;
			}

			throw new Exception($"Unknown index buffer type {indexType}");
		}

		public static VertexElement EnsureElement(this VertexDeclaration vd, VertexElementUsage usage)
		{
			var ve = vd.GetVertexElements();
			for (var i = 0; i < ve.Length; ++i)
			{
				if (ve[i].VertexElementUsage == usage)
				{
					return ve[i];
				}
			}

			throw new Exception($"Could not find vertex element with usage {usage}");
		}

		public static VertexData GetVertexData(this DrMeshPart p)
		{
			var vd = p.VertexBuffer.VertexDeclaration;
			var stride = vd.VertexStride;

			var vertexBytes = new byte[p.NumVertices * stride];

			p.VertexBuffer.GetData(p.VertexOffset * stride, vertexBytes, 0, p.NumVertices * stride);

			var elPos = vd.EnsureElement(VertexElementUsage.Position);
			var elNormal = vd.EnsureElement(VertexElementUsage.Normal);
			var elUv = vd.EnsureElement(VertexElementUsage.TextureCoordinate);

			var result = new VertexData
			{
				Vertices = new VertexElementData[p.NumVertices]
			};

			for (var i = 0; i < p.NumVertices; ++i)
			{
				var ved = new VertexElementData();
				unsafe
				{
					fixed (byte* bptr = &vertexBytes[i * stride + elPos.Offset])
					{
						Vector3* v = (Vector3*)bptr;
						ved.Position = *v;
					}

					fixed (byte* bptr = &vertexBytes[i * stride + elNormal.Offset])
					{
						Vector3* v = (Vector3*)bptr;
						ved.Normal = *v;
					}

					fixed (byte* bptr = &vertexBytes[i * stride + elUv.Offset])
					{
						Vector2* v = (Vector2*)bptr;
						ved.UV = *v;
					}
				}

				result.Vertices[i] = ved;
			}

			result.Indices = new uint[p.PrimitiveCount * 3];
			if (p.IndexBuffer.IndexElementSize == IndexElementSize.SixteenBits)
			{
				var idata = new ushort[result.Indices.Length];
				p.IndexBuffer.GetData(idata);

				for (var i = 0; i < idata.Length; ++i)
				{
					result.Indices[i] = idata[i];
				}
			}
			else
			{
				p.IndexBuffer.GetData(result.Indices);
			}

			return result;
		}

		public static VertexElementData GetVertexElementData(this VertexData vd, int face, int vertex)
		{
			var idx = vd.Indices[face * 3 + vertex];

			return vd.Vertices[idx];
		}

		public static SVec3 ToSVec3(this Vector3 v) => new SVec3(v.X, v.Y, v.Z);
		public static SVec2 ToSVec2(this Vector2 v) => new SVec2(v.X, v.Y);

		public static Vector3 ToVector3(this SVec3 v) => new Vector3(v.x, v.y, v.z);

		public static void CalculateTangets(this VertexData vd)
		{
			var ctx = new SMikkTSpaceContext
			{
				m_getNumFaces = () => vd.Indices.Length / 3,
				m_getNumVerticesOfFace = face => 3,
				m_getPosition = (face, vertex) => vd.GetVertexElementData(face, vertex).Position.ToSVec3(),
				m_getNormal = (face, vertex) => vd.GetVertexElementData(face, vertex).Normal.ToSVec3(),
				m_getTexCoord = (face, vertex) => vd.GetVertexElementData(face, vertex).UV.ToSVec2(),

				m_setTSpace = (tangent, bitangent, magS, magT, orient, face, vertex) =>
					{
						var idx = vd.Indices[face * 3 + vertex];
						vd.Vertices[idx].Tangent = tangent.ToVector3();
						vd.Vertices[idx].BiTangent = bitangent.ToVector3();
					}
			};

			var result = genTangSpaceDefault(ctx);
			if (result == 0)
			{
				throw new Exception("Tangents generation failed");
			}
		}

		public static Vec3 ToVec3(this Vector3 v) => new Vec3 { X = v.X, Y = v.Y, Z = v.Z };
		public static Vec2 ToVec2(this Vector2 v) => new Vec2 { X = v.X, Y = v.Y };
		public static VData ToVData(this VertexElementData vd)
		{
			return new VData
			{
				Position = vd.Position.ToVec3(),
				Normal = vd.Normal.ToVec3(),
				UV = vd.UV.ToVec2(),
				Tangent = vd.Tangent.ToVec3(),
				BiTangent = vd.BiTangent.ToVec3(),
			};
		}

		public static Vector3 ToVector3(this Vec3 v) => new Vector3(v.X, v.Y, v.Z);

		public static void CalculateTangentsNative(this VertexData vd)
		{
			var vdatas = new VData[vd.Vertices.Length];
			for (var i = 0; i < vdatas.Length; ++i)
			{
				vdatas[i] = vd.Vertices[i].ToVData();
			}

			Native.CalculateTangents(vdatas, vd.Indices);

			for (var i = 0; i < vdatas.Length; ++i)
			{
				vd.Vertices[i].Tangent = vdatas[i].Tangent.ToVector3();
				vd.Vertices[i].BiTangent = vdatas[i].BiTangent.ToVector3();
			}
		}

		public static void AssertAreEqual(Vector3 a, Vector3 b)
		{
			Assert.AreEqual(a.X, b.X, ZeroTolerance);
			Assert.AreEqual(a.Y, b.Y, ZeroTolerance);
			Assert.AreEqual(a.Z, b.Z, ZeroTolerance);
		}
	}
}
