using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MikkTSpaceSharp.Tests
{
	[TestClass]
	public class StartUp
	{
		[AssemblyInitialize]
		public static void SetUp(TestContext testContext)
		{
			TestsEnvironment.SetUp();
		}

	}
}
