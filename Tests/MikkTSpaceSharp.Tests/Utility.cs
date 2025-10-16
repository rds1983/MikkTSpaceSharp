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
		public Vector4 TangentBasic;
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

		public static SVec2 ToSVec2(this Vector2 v) => new SVec2(v.X, v.Y);
		public static SVec3 ToSVec3(this Vector3 v) => new SVec3(v.X, v.Y, v.Z);
		public static Vector3 ToVector3(this SVec3 v) => new Vector3(v.x, v.y, v.z);

		public static void CalculateTangents(this VertexData vd)
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
			if (!result)
			{
				throw new Exception("Tangents generation failed");
			}
		}

		public static void CalculateTangentsBasic(this VertexData vd)
		{
			var ctx = new SMikkTSpaceContext
			{
				m_getNumFaces = () => vd.Indices.Length / 3,
				m_getNumVerticesOfFace = face => 3,
				m_getPosition = (face, vertex) => vd.GetVertexElementData(face, vertex).Position.ToSVec3(),
				m_getNormal = (face, vertex) => vd.GetVertexElementData(face, vertex).Normal.ToSVec3(),
				m_getTexCoord = (face, vertex) => vd.GetVertexElementData(face, vertex).UV.ToSVec2(),
				m_setTSpaceBasic = (SVec3 tangent, float orient, int face, int vertex) =>
				{
					var idx = vd.Indices[face * 3 + vertex];
					vd.Vertices[idx].TangentBasic = new Vector4(tangent.x, tangent.y, tangent.z, orient);
				}
			};

			var result = genTangSpaceDefault(ctx);
			if (!result)
			{
				throw new Exception("Tangents generation failed");
			}
		}

		public static Vec2 ToVec2(this Vector2 v) => new Vec2(v.X, v.Y);
		public static Vec3 ToVec3(this Vector3 v) => new Vec3(v.X, v.Y, v.Z);
		public static Vec4 ToVec4(this Vector4 v) => new Vec4(v.X, v.Y, v.Z, v.W);
		public static VData ToVData(this VertexElementData vd)
		{
			return new VData
			{
				Position = vd.Position.ToVec3(),
				Normal = vd.Normal.ToVec3(),
				UV = vd.UV.ToVec2(),
				Tangent = vd.Tangent.ToVec3(),
				BiTangent = vd.BiTangent.ToVec3(),
				TangentBasic = vd.TangentBasic.ToVec4()
			};
		}

		public static Vector3 ToVector3(this Vec3 v) => new Vector3(v.X, v.Y, v.Z);
		public static Vector4 ToVector4(this Vec4 v) => new Vector4(v.X, v.Y, v.Z, v.W);

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

		public static void CalculateTangentsNativeBasic(this VertexData vd)
		{
			var vdatas = new VData[vd.Vertices.Length];
			for (var i = 0; i < vdatas.Length; ++i)
			{
				vdatas[i] = vd.Vertices[i].ToVData();
			}

			Native.CalculateTangentsBasic(vdatas, vd.Indices);

			for (var i = 0; i < vdatas.Length; ++i)
			{
				vd.Vertices[i].TangentBasic = vdatas[i].TangentBasic.ToVector4();
			}
		}

		public static void AssertAreEqual(Vector3 a, Vector3 b, float epsilon)
		{
			Assert.AreEqual(a.X, b.X, epsilon);
			Assert.AreEqual(a.Y, b.Y, epsilon);
			Assert.AreEqual(a.Z, b.Z, epsilon);
		}

		public static void AssertAreEqual(Vector4 a, Vector4 b, float epsilon)
		{
			Assert.AreEqual(a.X, b.X, epsilon);
			Assert.AreEqual(a.Y, b.Y, epsilon);
			Assert.AreEqual(a.Z, b.Z, epsilon);
			Assert.AreEqual(a.W, b.W, epsilon);
		}
	}
}
