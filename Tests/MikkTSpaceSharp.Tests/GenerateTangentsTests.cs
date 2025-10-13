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
				}
			}
		}
	}
}
