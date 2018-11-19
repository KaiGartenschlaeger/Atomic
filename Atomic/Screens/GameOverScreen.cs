using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PureFreak.TileMore.Screens;

namespace Atomic.Screens
{
    public class GameOverScreen : Screen
    {
        #region Constructor

        public GameOverScreen()
        {
        }

        #endregion

        #region Screen methods

        protected override void OnStart()
        {
        }

        protected override void OnInput(GameTime time, int updateCounter)
        {
            if (Keyboard.IsKeyReleased(Keys.Escape))
                Manager.SwitchTo<StartMenuScreen>();
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            GraphicsDevice.Clear(AppColors.WindowBackground);

            batch.Begin();

            var size = AppContents.DefaultFont.MeasureString("Game Over");
            var pos = new Vector2(
                GraphicsDevice.Viewport.Width / 2 - size.X / 2,
                GraphicsDevice.Viewport.Height / 2 - size.Y / 2);

            batch.DrawBitmapFont(AppContents.DefaultFont, pos, "Game Over");

            batch.End();
        }

        #endregion

        #region Properties

        [Store]
        public AppContents AppContents { get; set; }

        #endregion
    }
}