using Atomic.Services.SaveGames;
using Atomic.Services.Sounds;
using Atomic.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using PureFreak.TileMore.Screens;

namespace Atomic.Screens
{
    public partial class StartMenuScreen : Screen
    {
        #region Fields

        private readonly ISaveGameService _saveService;
        private readonly ISoundsManager _soundsManager;

        private Texture2D _background;
        private TextMenu _menu;

        #endregion

        #region Constructor

        public StartMenuScreen(
            ISaveGameService saveService,
            ISoundsManager soundsManager)
        {
            _saveService = saveService;
            _soundsManager = soundsManager;
        }

        #endregion

        #region Menu events

        private void Menu_ItemHovered(TextMenuItem item)
        {
            _soundsManager.PlaySound(SoundName.Blip5);
        }

        private void ItemNew_Clicked()
        {
            GetScreen<GameScreen>().NewGame();
            Manager.SwitchTo<GameScreen>();
        }

        private void ItemContinueLast_Clicked()
        {
            GetScreen<GameScreen>().ContinueLastGame();
            Manager.SwitchTo<GameScreen>();
        }

        private void ItemSettings_Clicked()
        {
            Manager.SwitchTo<SettingsScreen>();
        }

        private void ItemEnd_Clicked()
        {
            Game.Exit();
        }

        #endregion

        #region Screen methods

        protected override void OnEnter()
        {
            var item = _menu.GetItem("continue_last_game");
            item.IsEnabled = _saveService.HasSaveGame(AppConstants.LastSaveGameFileName);
        }

        protected override void OnStart()
        {
            _background = Content.Load<Texture2D>("Background");

            _menu = new TextMenu(AppContents.DefaultFont);
            _menu.ItemHovered += Menu_ItemHovered;
            _menu.Pos = new Vector2(100, 150);
            _menu.Padding = AppConstants.MenuPadding;
            _menu.Color = AppColors.MenuItems;
            _menu.ColorDisabled = AppColors.MenuItemsDisabled;
            _menu.ColorHover = AppColors.MenuItemsHover;

            var itemNew = _menu.CreateItem("New game");
            itemNew.Clicked += ItemNew_Clicked;

            var itemContinueLast = _menu.CreateItem("Continue last game");
            itemContinueLast.Clicked += ItemContinueLast_Clicked;

            var itemHighscore = _menu.CreateItem("Highscore list");

            var itemSettings = _menu.CreateItem("Settings");
            itemSettings.Clicked += ItemSettings_Clicked;

            var itemEnd = _menu.CreateItem("Exit");
            itemEnd.Margin = new Padding(0, 25, 0, 0);
            itemEnd.Clicked += ItemEnd_Clicked;
        }

        protected override void OnUpdate(GameTime time, int updateCounter)
        {
            _menu.Update(time, Mouse);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            GraphicsDevice.Clear(AppColors.WindowBackground);

            batch.Begin();

            // background
            batch.Draw(_background, new Vector2(GraphicsDevice.Viewport.Width - _background.Width - 25, 25), Color.White);
            // menu
            _menu.Draw(batch);

            batch.End();
        }

        #endregion

        #region Properties

        [Store]
        public AppContents AppContents { get; set; }

        #endregion
    }
}