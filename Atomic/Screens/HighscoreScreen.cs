using Atomic.Services.Highscore;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PureFreak.TileMore;
using PureFreak.TileMore.Screens;

namespace Atomic.Screens
{
    public class HighscoreScreen : Screen
    {
        #region Fields

        private readonly IHighscoreService _highscore;

        #endregion

        #region Constructor

        public HighscoreScreen(IHighscoreService highscore)
        {
            _highscore = highscore;
        }

        #endregion

        #region Screen methods

        protected override void OnStart()
        {
            _highscore.Load();
        }

        protected override void OnEnd()
        {
            if (HasChanges) _highscore.Save();
        }

        protected override void OnInput(GameTime time, int updateCounter)
        {
            if (Keyboard.IsKeyPressed(Keys.Escape))
                Manager.SwitchTo<StartMenuScreen>();
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            GraphicsDevice.Clear(AppColors.WindowBackground);

            batch.Begin();

            if (_highscore.Items.Length > 0)
            {
                var posY = 100;
                for (int i = 0; i < MathI.Min(_highscore.Items.Length, AppConstants.TopHighscorePlaces); i++)
                {
                    var item = _highscore.Items[i];

                    var scoreText = item.Score.ToString("n0");
                    var scoreSize = AppContents.DefaultFont.MeasureString(scoreText);

                    batch.DrawBitmapFont(AppContents.DefaultFont,
                        new Vector2(100, posY),
                        (1 + i).ToString(),
                        AppColors.Descriptions);

                    batch.DrawBitmapFont(AppContents.DefaultFont,
                        new Vector2(160, posY),
                        item.User,
                        AppColors.Texts);

                    batch.DrawBitmapFont(AppContents.DefaultFont,
                        new Vector2(GraphicsDevice.Viewport.Width - scoreSize.X - 100, posY),
                        scoreText,
                        AppColors.Texts);

                    posY += AppContents.DefaultFont.Data.LineHeight;
                }
            }
            else
            {
                batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(100, 100), "Empty", AppColors.Texts);
            }

            batch.End();
        }

        #endregion

        #region Properties

        [Store]
        public AppContents AppContents { get; set; }

        public bool HasChanges { get; set; }

        #endregion
    }
}