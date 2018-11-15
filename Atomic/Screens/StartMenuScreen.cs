using Atomic.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore.Screens;

namespace Atomic.Screens
{
    public partial class StartMenuScreen : Screen
    {
        #region Fields

        private TextMenu _menu;

        #endregion

        #region Menu events

        private void ItemNew_Clicked()
        {
            var gameScreen = GetScreen<GameScreen>();
            gameScreen.NewGame();

            Manager.SwitchTo<GameScreen>();
        }

        private void ItemEnd_Clicked()
        {
            Game.Exit();
        }

        #endregion

        #region Screen methods

        protected override void OnStart()
        {
            _menu = new TextMenu(AppContents.DefaultFont);
            _menu.Pos = new Vector2(AppConstants.MenuX, AppConstants.MenuY);
            _menu.Padding = AppConstants.MenuPadding;
            _menu.Color = AppColors.MenuItems;
            _menu.HoverColor = AppColors.MenuItemsHover;

            var itemNew = _menu.CreateItem("Neues Spiel");
            itemNew.Clicked += ItemNew_Clicked;

            var itemEnd = _menu.CreateItem("Beenden");
            //itemEnd.Margin = new Padding(0, 25, 0, 0);
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