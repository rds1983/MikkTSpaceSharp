[![NuGet](https://img.shields.io/nuget/v/MikkTSpaceSharp.svg)](https://www.nuget.org/packages/MikkTSpaceSharp/)
[![Build & Publish Beta](https://github.com/rds1983/MikkTSpaceSharp/actions/workflows/build-and-publish-beta.yml/badge.svg)](https://github.com/rds1983/MikkTSpaceSharp/actions/workflows/build-and-publish-beta.yml)
[![Chat](https://img.shields.io/discord/628186029488340992.svg)](https://discord.gg/ZeHxhCY)

### Overview
C# Port of https://github.com/mmikk/MikkTSpace

### Adding Reference
https://www.nuget.org/packages/MikkTSpaceSharp/

### Usage
This sample code generates Vector4 tangents(w stores orientation):
```c#
	public struct VertexElementData
	{
		public Vector3 Position;
		public Vector3 Normal;
		public Vector2 UV;
		public Vector4 TangentsBasic;
	}

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
			if (result == 0)
			{
				throw new Exception("Tangents generation failed");
			}
		}
```
