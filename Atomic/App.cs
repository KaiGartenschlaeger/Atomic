using Atomic.Screens;
using Atomic.Services;
using Microsoft.Xna.Framework;
using PureFreak.TileMore;
using PureFreak.TileMore.Screens;

namespace Atomic
{
    public class App : TileMoreGame
    {
        private readonly ScreenManager _screenManager;

        public App()
            : base(AppConstants.WindowWidth, AppConstants.WindowHeight)
        {
            _screenManager = new ScreenManager(this);
        }

        protected override void Initialize()
        {
            Components.Add(_screenManager);

            _screenManager.Dependencies.AddSingleton<ISaveGameService, XmlSaveGameService>();

            _screenManager.Register<StartMenuScreen>();
            _screenManager.Register<GameScreen>();
            _screenManager.Register<GameMenuScreen>();

            _screenManager.SwitchTo<StartMenuScreen>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var contents = new AppContents();
            contents.LoadContent(Content);

            _screenManager.Store.Add("AppContents", contents);

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}