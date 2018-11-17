using Atomic.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using PureFreak.TileMore.Screens;

namespace Atomic.Screens
{
    public class GameMenuScreen : Screen
    {
        #region Fields

        private TextMenu _menu;
        
        #endregion

        #region Menu events

        private void ItemContinue_Clicked()
        {
            Manager.Deactivate<GameMenuScreen>();
        }

        private void ItemCancel_Clicked()
        {
            Manager.SwitchTo<StartMenuScreen>();
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

            var itemContinue = _menu.CreateItem("Spiel fortsetzen");
            itemContinue.Clicked += ItemContinue_Clicked;

            var itemCancel = _menu.CreateItem("Spiel abbrechen");
            itemCancel.Clicked += ItemCancel_Clicked;

            var itemEnd = _menu.CreateItem("Beenden");
            itemEnd.Margin = new Padding(0, 25, 0, 0);
            itemEnd.Clicked += ItemEnd_Clicked;
        }

        protected override void OnInput(GameTime time, int updateCounter)
        {
            if (Keyboard.IsKeyReleased(Microsoft.Xna.Framework.Input.Keys.Escape))
                ItemContinue_Clicked();

            _menu.Update(time, Mouse);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            batch.Begin();

            // overlay background
            batch.FillRect(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), 0, new Color(0, 0, 0, 180));
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