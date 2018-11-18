using Atomic.Services.SaveGames;
using Atomic.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using PureFreak.TileMore.Screens;
using System;

namespace Atomic.Screens
{
    public class GameMenuScreen : Screen
    {
        #region Fields

        private TextMenu _menu;
        private readonly ISaveGameService _saveService;

        #endregion

        #region Constructor

        public GameMenuScreen(ISaveGameService saveService)
        {
            if (saveService == null)
                throw new ArgumentNullException(nameof(saveService));

            _saveService = saveService;
        }

        #endregion

        #region Menu events

        private void ItemContinue_Clicked()
        {
            Manager.Deactivate<GameMenuScreen>();
        }

        private void ItemSave_Clicked()
        {
            var gs = GetScreen<GameScreen>();
            var session = gs.Session;
            var grid = gs.Grid;

            var data = new SaveGameData();
            data.ElapsedTime = TimeSpan.FromSeconds(session.Time);

            data.Score = session.Score;
            data.Atoms = session.Atoms;
            data.Molecules = session.Molecules;

            data.CurrentAtom = session.CurrentAtom.Electrons;
            data.NextAtom = session.NextAtom.Electrons;

            data.GridData = new SaveGameGridData[grid.Width, grid.Height];
            for (int gridX = 0; gridX < grid.Width; gridX++)
            {
                for (int gridY = 0; gridY < grid.Height; gridY++)
                {
                    var atom = grid.GetAtom(gridX, gridY);
                    if (atom != null)
                    {
                        data.GridData[gridX, gridY] = new SaveGameGridData
                        {
                            Electrons = atom.Electrons,
                            ConnectedLeft = atom.LeftConnection != null,
                            ConnectedTop = atom.TopConnection != null,
                            ConnectedRight = atom.RightConnection != null,
                            ConnectedBottom = atom.BottomConnection != null
                        };
                    }
                }
            }

            _saveService.SaveGame(AppConstants.LastSaveGameFileName, data);

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
            _menu.Pos = new Vector2(100, 150);
            _menu.Padding = AppConstants.MenuPadding;
            _menu.Color = AppColors.MenuItems;
            _menu.ColorDisabled = AppColors.MenuItemsDisabled;
            _menu.ColorHover = AppColors.MenuItemsHover;

            var itemContinue = _menu.CreateItem("Continue", "ContinueGame");
            itemContinue.Clicked += ItemContinue_Clicked;

            var itemSave = _menu.CreateItem("Save", "Save");
            itemSave.Clicked += ItemSave_Clicked;

            var itemCancel = _menu.CreateItem("Cancel", "Cancel");
            itemCancel.Clicked += ItemCancel_Clicked;

            var itemEnd = _menu.CreateItem("Exit");
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
            batch.FillRect(new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), 0, new Color(0, 0, 0, 220));
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