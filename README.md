[![NuGet](https://img.shields.io/nuget/v/MikkTSpaceSharp.svg)](https://www.nuget.org/packages/MikkTSpaceSharp/)
[![Build & Publish Beta](https://github.com/rds1983/MikkTSpaceSharp/actions/workflows/build-and-publish-beta.yml/badge.svg)](https://github.com/rds1983/MikkTSpaceSharp/actions/workflows/build-and-publish-beta.yml)
[![Chat](https://img.shields.io/discord/628186029488340992.svg)](https://discord.gg/ZeHxhCY)

### Overview
C# Port of https://github.com/mmikk/MikkTSpace

### Adding Reference
https://www.nuget.org/packages/MikkTSpaceSharp/

### Usage
This sample code generates Vector4 tangents(W stores orientation):
```c#
using static MikkTSpaceSharp.MikkTSpace;

public struct VertexElementData
{
	public Vector3 Position;
	public Vector3 Normal;
	public Vector2 UV;
}

public static class TangentsCalc
{
	public static SVec2 ToSVec2(this Vector2 v) => new SVec2(v.X, v.Y);
	public static SVec3 ToSVec3(this Vector3 v) => new SVec3(v.X, v.Y, v.Z);

	public static Vector4[] Calculate(VertexElementData[] vertices, uint[] indices)
	{
		var result = new Vector4[vertices.Length];
		Func<int, int, uint> indexCalc = (face, vertex) => indices[face * 3 + vertex];
		var ctx = new SMikkTSpaceContext
		{
			m_getNumFaces = () => indices.Length / 3,
			m_getNumVerticesOfFace = face => 3,
			m_getPosition = (face, vertex) => vertices[indexCalc(face, vertex)].Position.ToSVec3(),
			m_getNormal = (face, vertex) => vertices[indexCalc(face, vertex)].Normal.ToSVec3(),
			m_getTexCoord = (face, vertex) => vertices[indexCalc(face, vertex)].UV.ToSVec2(),
			m_setTSpaceBasic = (SVec3 tangent, float orient, int face, int vertex) =>
			{
				var idx = indexCalc(face, vertex);
				result[idx] = new Vector4(tangent.x, tangent.y, tangent.z, orient);
			}
		};

		var r = genTangSpaceDefault(ctx);
		if (r == 0)
		{
			throw new Exception("Tangents generation failed");
		}

		return result;
	}
}
```
