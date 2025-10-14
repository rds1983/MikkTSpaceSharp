using Microsoft.Xna.Framework.Graphics;

namespace MikkTSpaceSharp.Tests
{
	public class TestsEnvironment
	{
		private static TestGame _game;

		public static GraphicsDevice GraphicsDevice => _game.GraphicsDevice;

		public static void SetUp()
		{
			_game = new TestGame();
		}
	}
}
