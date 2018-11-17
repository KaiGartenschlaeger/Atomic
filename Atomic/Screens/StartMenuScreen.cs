using Atomic.Services;
using Atomic.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using PureFreak.TileMore.Screens;
using System;

namespace Atomic.Screens
{
    public partial class StartMenuScreen : Screen
    {
        #region Fields

        private Texture2D _background;
        private TextMenu _menu;
        private readonly SaveGameService _saveService;

        #endregion

        #region Constructor

        public StartMenuScreen(SaveGameService saveService)
        {
            if (saveService == null)
                throw new ArgumentNullException(nameof(saveService));

            _saveService = saveService;
        }

        #endregion

        #region Menu events

        private void ItemNew_Clicked()
        {
            var gs = GetScreen<GameScreen>();
            gs.StartNewGame();

            Manager.SwitchTo<GameScreen>();
        }

        private void ItemContinueLast_Clicked()
        {
            var gs = GetScreen<GameScreen>();
            if (gs.ContinueLastGame())
            {
                Manager.SwitchTo<GameScreen>();
            }
        }

        private void ItemEnd_Clicked()
        {
            Game.Exit();
        }

        #endregion

        #region Screen methods

        protected override void OnEnter()
        {
            var item = _menu.GetItem("ContinueLastGame");
            item.IsEnabled = _saveService.HasLastGame();
        }

        protected override void OnStart()
        {
            _background = Content.Load<Texture2D>("Background");

            _menu = new TextMenu(AppContents.DefaultFont);
            _menu.Pos = new Vector2(100, 150);
            _menu.Padding = AppConstants.MenuPadding;
            _menu.Color = AppColors.MenuItems;
            _menu.ColorDisabled = AppColors.MenuItemsDisabled;
            _menu.ColorHover = AppColors.MenuItemsHover;

            var itemNew = _menu.CreateItem("New game", "NewGame");
            itemNew.Clicked += ItemNew_Clicked;

            var itemContinueLast = _menu.CreateItem("Continue last game", "ContinueLastGame");
            itemContinueLast.Clicked += ItemContinueLast_Clicked;

            var itemHighscore = _menu.CreateItem("Highscore list");

            var itemEnd = _menu.CreateItem("Exit", "Exit");
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