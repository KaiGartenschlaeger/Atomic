using Atomic.Screens;
using Atomic.Services.Highscore;
using Atomic.Services.SaveGames;
using Atomic.Services.Settings;
using Atomic.Services.Sounds;
using Atomic.UI;
using Atomic.UI.Elements;
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

            _screenManager.Dependencies.AddSingleton<ISoundsManager, SoundsManager>();
            _screenManager.Dependencies.AddSingleton<ISettingsService, XmlSettingsService>();
            _screenManager.Dependencies.AddSingleton<ISaveGameService, XmlSaveGameService>();
            _screenManager.Dependencies.AddSingleton<IHighscoreService, XmlHighscoreService>();

            _screenManager.Register<StartMenuScreen>();
            _screenManager.Register<SettingsScreen>();
            _screenManager.Register<HighscoreScreen>();
            _screenManager.Register<GameScreen>();
            _screenManager.Register<GameMenuScreen>();
            _screenManager.Register<GameOverScreen>();

            _screenManager.SwitchTo<StartMenuScreen>();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            var contents = new AppContents();
            contents.LoadContent(Content);

            _screenManager.Store.Add("AppContents", contents);

            var skin = new UISkin(contents.DefaultFont);
            skin.SetValue<Label, Color>(l => l.Color, AppColors.MenuItems);
            skin.SetValue<Label, Color>(l => l.ColorHovered, AppColors.MenuItemsHover);

            _screenManager.Store.Add("UISkin", skin);

            _screenManager.Dependencies.GetService<ISoundsManager>()
                .LoadContent(Content);

            base.LoadContent();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}