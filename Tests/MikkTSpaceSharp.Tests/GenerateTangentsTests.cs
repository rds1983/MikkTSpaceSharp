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

					var clone = vd.Clone();

					// Calculate tangents through MikkTSpaceSharp
					vd.CalculateTangets();

					// Calculate tangents through native wrapper
					clone.CalculateTangentsNative();

					// Compare
					for(var i = 0; i < vd.Vertices.Length; ++i)
					{
						Utility.AssertAreEqual(vd.Vertices[i].Tangent, clone.Vertices[i].Tangent);
						Utility.AssertAreEqual(vd.Vertices[i].BiTangent, clone.Vertices[i].BiTangent);
					}
				}
			}
		}
	}
}
