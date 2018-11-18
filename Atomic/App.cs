using Atomic.Screens;
using Atomic.Services.SaveGames;
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

            ISoundsManager soundsManager = new SoundsManager();
            soundsManager.Volume = 50;

            _screenManager.Dependencies.AddSingleton<ISoundsManager>(r =>
            {
                return soundsManager;
            });

            _screenManager.Dependencies.AddSingleton<ISaveGameService, XmlSaveGameService>();

            _screenManager.Register<StartMenuScreen>();
            _screenManager.Register<SettingsScreen>();
            _screenManager.Register<GameScreen>();
            _screenManager.Register<GameOverScreen>();
            _screenManager.Register<GameMenuScreen>();

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