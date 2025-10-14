using DigitalRiseModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MikkTSpaceSharp.Tests
{
	[TestClass]
	public sealed class GenerateTangentsTests
	{
		[TestMethod]
		public void GenerateTangets()
		{
			var manager = Utility.CreateAssetManager();

			var model = manager.LoadModel(TestsEnvironment.GraphicsDevice, "AlphaDeadTree.glb");

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
						Utility.AssertAreEqual(a.Vertices[i].Tangent, b.Vertices[i].Tangent);
						Utility.AssertAreEqual(a.Vertices[i].BiTangent, b.Vertices[i].BiTangent);
					}

					// Now do basic calculations
					a = vd.Clone();
					a.CalculateTangentsBasic();

					b = vd.Clone();
					b.CalculateTangentsNativeBasic();

					for (var i = 0; i < vd.Vertices.Length; ++i)
					{
						Utility.AssertAreEqual(a.Vertices[i].TangentBasic, b.Vertices[i].TangentBasic);
					}
				}
			}
		}
	}
}
