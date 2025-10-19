[![NuGet](https://img.shields.io/nuget/v/MikkTSpaceSharp.svg)](https://www.nuget.org/packages/MikkTSpaceSharp/)
[![Build & Publish Beta](https://github.com/rds1983/MikkTSpaceSharp/actions/workflows/build-and-publish-beta.yml/badge.svg)](https://github.com/rds1983/MikkTSpaceSharp/actions/workflows/build-and-publish-beta.yml)
[![Chat](https://img.shields.io/discord/628186029488340992.svg)](https://discord.gg/ZeHxhCY)

### Overview
C# Port of https://github.com/mmikk/MikkTSpace

Which is a common standard for tangent space used in baking tools to produce normal maps.

More information can be found at http://www.mikktspace.com/.

It is important to note, that MikkTSpaceSharp is **port**(not **wrapper**). Original C code had been ported to C#. Therefore MikkTSpaceSharp doesnt require any native binaries.

The porting hasn't been done by hand, but using [Hebron](https://github.com/rds1983/Hebron), which is the C to C# code converter utility.

### Adding Reference
https://www.nuget.org/packages/MikkTSpaceSharp/

### Usage
This sample code accepts basic geometry represented by triangles and generates Vector4 tangents(W stores orientation):
```c#
using MikkTSpaceSharp;
using System;
using System.Numerics;
using static MikkTSpaceSharp.MikkTSpace;

namespace GltfUtility
{
	public static class TangentsCalc
	{
		public static SVec2 ToSVec2(this Vector2 v) => new SVec2(v.X, v.Y);
		public static SVec3 ToSVec3(this Vector3 v) => new SVec3(v.X, v.Y, v.Z);

		public static Vector4[] Calculate(Vector3[] positions, Vector3[] normals, Vector2[] uvs, uint[] indices)
		{
			if (positions == null)
			{
				throw new ArgumentNullException(nameof(positions));
			}

			if (normals == null)
			{
				throw new ArgumentNullException(nameof(normals));
			}

			if (uvs == null)
			{
				throw new ArgumentNullException(nameof(uvs));
			}

			if (positions.Length != normals.Length)
			{
				throw new ArgumentException($"Inconsistent sizes: positions.Length = {positions.Length}, normals.Length = {normals.Length}");
			}

			if (positions.Length != uvs.Length)
			{
				throw new ArgumentException($"Inconsistent sizes: positions.Length = {positions.Length}, uvs.Length = {uvs.Length}");
			}

			var result = new Vector4[positions.Length];
			Func<int, int, uint> indexCalc = (face, vertex) => indices[face * 3 + vertex];
			var ctx = new SMikkTSpaceContext
			{
				m_getNumFaces = () => indices.Length / 3,
				m_getNumVerticesOfFace = face => 3,
				m_getPosition = (face, vertex) => positions[indexCalc(face, vertex)].ToSVec3(),
				m_getNormal = (face, vertex) => normals[indexCalc(face, vertex)].ToSVec3(),
				m_getTexCoord = (face, vertex) => uvs[indexCalc(face, vertex)].ToSVec2(),
				m_setTSpaceBasic = (SVec3 tangent, float orient, int face, int vertex) =>
				{
					var idx = indexCalc(face, vertex);
					result[idx] = new Vector4(tangent.x, tangent.y, tangent.z, orient);
				}
			};

			var r = genTangSpaceDefault(ctx);
			if (!r)
			{
				throw new Exception("Tangents generation failed");
			}

			return result;
		}
	}
}

```
