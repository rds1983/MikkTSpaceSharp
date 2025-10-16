using DigitalRiseModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MikkTSpaceSharp.Tests
{
	[TestClass]
	public sealed class GenerateTangentsTests
	{
		public const float DefaultEpsilon = 1e-4f;

		[TestMethod]
		[DataRow("AlphaDeadTree.glb")]
		[DataRow("dude.glb")]
		[DataRow("banner.glb")]
		[DataRow("Hotel01.glb")]
		[DataRow("FlightHelmet.glb")]
		[DataRow("BarramundiFish.glb")]

		public void GenerateTangets(string fileName, float epsilon = DefaultEpsilon)
		{
			var manager = Utility.CreateAssetManager();

			var model = manager.LoadModel(TestsEnvironment.GraphicsDevice, fileName, ModelLoadFlags.IgnoreMaterials | ModelLoadFlags.ReadableBuffers);

			foreach(var mesh in model.Meshes)
			{
				foreach(var part in mesh.MeshParts)
				{
					var vd = part.GetVertexData();


					// Calculate tangents through MikkTSpaceSharp
					var a = vd.Clone();
					a.CalculateTangents();

					// Calculate tangents through native wrapper
					var b = vd.Clone();
					b.CalculateTangentsNative();

					// Compare
					for(var i = 0; i < vd.Vertices.Length; ++i)
					{
						Utility.AssertAreEqual(a.Vertices[i].Tangent, b.Vertices[i].Tangent, epsilon);
						Utility.AssertAreEqual(a.Vertices[i].BiTangent, b.Vertices[i].BiTangent, epsilon);
					}

					// Now do basic calculations
					a = vd.Clone();
					a.CalculateTangentsBasic();

					b = vd.Clone();
					b.CalculateTangentsNativeBasic();

					for (var i = 0; i < vd.Vertices.Length; ++i)
					{
						Utility.AssertAreEqual(a.Vertices[i].TangentBasic, b.Vertices[i].TangentBasic, epsilon);
					}
				}
			}
		}
	}
}
