using Atomic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PureFreak.TileMore;
using PureFreak.TileMore.Input;
using PureFreak.TileMore.Screens;

namespace Atomic.Screens
{
    public class GameScreen : Screen
    {
        #region Grid helper

        private bool IsMouseOverGrid()
        {
            return
                Mouse.Position.X >= AppConstants.GridX &&
                Mouse.Position.Y >= AppConstants.GridY &&
                Mouse.Position.X <= AppConstants.GridX + Grid.PixelWidth &&
                Mouse.Position.Y <= AppConstants.GridY + Grid.PixelHeight;
        }

        private Point GetMouseGridPos()
        {
            if (IsMouseOverGrid())
            {
                return new Point(
                    (Mouse.Position.X - AppConstants.GridX) / Grid.TileSize,
                    (Mouse.Position.Y - AppConstants.GridY) / Grid.TileSize);
            }

            return new Point(-1, -1);
        }

        #endregion

        #region Public methods

        public void NewGame()
        {
            Session.Reset();
            Grid.Clear();

            CurrentAtom = Grid.CreateAtom();
            NextAtom = Grid.CreateAtom();

            // test case 1
            //Grid.SetAtom(2, 2, 2);
            //Grid.SetAtom(3, 2, 3);
            //Grid.SetAtom(4, 2, 2);
            //Grid.SetAtom(4, 3, 3);
            //Grid.SetAtom(4, 4, 2);
            //Grid.SetAtom(3, 4, 2);
            //Grid.SetAtom(3, 3, 4);
            //CurrentAtom = Grid.CreateAtom(1);
        }

        #endregion

        #region Screen methods

        protected override void OnStart()
        {
            Session = new GameSession();

            Grid = new AtomsGrid(AppContents, Session, AppConstants.GridTileSize, AppConstants.GridWidth, AppConstants.GridHeight);
            GridRenderer = new GridRenderer(Grid, Session);
        }

        protected override void OnUpdate(GameTime time, int updateCounter)
        {
            if (CurrentAtom != null)
                CurrentAtom.Update(time);
            if (NextAtom != null)
                NextAtom.Update(time);

            GridRenderer.Update(time);
        }

        protected override void OnInput(GameTime time, int updateCounter)
        {
            if (CurrentAtom != null &&
                IsMouseOverGrid() &&
                Mouse.IsButtonPressed(MouseButton.Left))
            {
                var gridPos = GetMouseGridPos();
                if (Grid.SetAtom(gridPos.X, gridPos.Y, CurrentAtom))
                {
                    CurrentAtom = NextAtom;
                    NextAtom = Grid.CreateAtom();
                }
            }

            if (Keyboard.IsKeyReleased(Keys.Escape))
                Manager.Activate<GameMenuScreen>();

#if CHEATS_ENABLED
            HandleCheats();
#endif
        }

        private void HandleCheats()
        {
            var electrons = -1;

            if (Keyboard.IsKeyReleased(Keys.D1)) electrons = 1;
            else if (Keyboard.IsKeyReleased(Keys.D2)) electrons = 2;
            else if (Keyboard.IsKeyReleased(Keys.D3)) electrons = 3;
            else if (Keyboard.IsKeyReleased(Keys.D4)) electrons = 4;

            if (CurrentAtom != null && electrons != -1)
                CurrentAtom.Electrons = electrons;

            if (Keyboard.IsKeyReleased(Keys.R))
                Grid.Clear();
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            GraphicsDevice.Clear(AppColors.WindowBackground);

            Batch.Begin(SpriteSortMode.FrontToBack);

            // grid
            GridRenderer.Draw(Batch, new Vector2(AppConstants.GridX, AppConstants.GridY));

            Batch.End();


            Batch.Begin();

            var y = AppConstants.GridY + 10;

            // score
            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), $"Score:", AppColors.Descriptions);
            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.WindowWidth - AppConstants.ScreenPadding - 140, y), Session.Score.ToString("n0"), AppColors.Texts);
            y += AppContents.DefaultFont.Data.LineHeight + 15;

            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), $"Atoms:", AppColors.Descriptions);
            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.WindowWidth - AppConstants.ScreenPadding - 140, y), Session.Atoms.ToString("n0"), AppColors.Texts);
            y += AppContents.DefaultFont.Data.LineHeight + 5;

            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), $"Molecules:", AppColors.Descriptions);
            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.WindowWidth - AppConstants.ScreenPadding - 140, y), Session.Molecules.ToString("n0"), AppColors.Texts);
            y += AppContents.DefaultFont.Data.LineHeight + 5;

            // current/next atom
            y = AppConstants.WindowHeight - AppConstants.ScreenPadding - AppConstants.PreviewBoxHeight - AppContents.DefaultFont.Data.LineHeight - 8;

            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), "Current", AppColors.Descriptions);
            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight + AppConstants.PreviewBoxWidth + AppConstants.PreviewBoxPadding, y), "Next", AppColors.Descriptions);
            y += AppContents.DefaultFont.Data.LineHeight + 8;

            Batch.DrawRect(new Rectangle(AppConstants.GridRight, y, AppConstants.PreviewBoxWidth, AppConstants.PreviewBoxHeight), 1, AppColors.PreviewBorder);
            Batch.DrawRect(new Rectangle(AppConstants.GridRight + AppConstants.PreviewBoxWidth + AppConstants.PreviewBoxPadding, y, AppConstants.PreviewBoxWidth, AppConstants.PreviewBoxHeight), 1, AppColors.PreviewBorder);

            if (CurrentAtom != null)
            {
                CurrentAtom.Draw(Batch,
                    pos: new Vector2(
                        AppConstants.GridRight + AppConstants.PreviewBoxWidth / 2,
                        y + AppConstants.PreviewBoxHeight / 2), layerDepth: LayerDepth.Default);
            }

            if (NextAtom != null)
            {
                NextAtom.Draw(Batch,
                    pos: new Vector2(
                        AppConstants.GridRight + AppConstants.PreviewBoxWidth + AppConstants.PreviewBoxPadding + AppConstants.PreviewBoxWidth / 2,
                        y + AppConstants.PreviewBoxHeight / 2),
                    layerDepth: LayerDepth.Default);
            }

            // atom grid preview
            if (CurrentAtom != null && IsMouseOverGrid() && IsOnTop)
            {
                var gridPos = GetMouseGridPos();

                if (Grid.IsValidPos(gridPos.X, gridPos.Y) && !Grid.HasAtom(gridPos.X, gridPos.Y))
                {
                    if (Grid.CanSet(gridPos.X, gridPos.Y, CurrentAtom))
                    {
                        CurrentAtom.Draw(Batch, new Vector2(
                            AppConstants.GridX + gridPos.X * Grid.TileSize + Grid.TileSize / 2,
                            AppConstants.GridY + gridPos.Y * Grid.TileSize + Grid.TileSize / 2),
                            LayerDepth.Default,
                            AppColors.AtomValidPos);
                    }
                    else
                    {
                        CurrentAtom.Draw(Batch, new Vector2(
                            AppConstants.GridX + gridPos.X * Grid.TileSize + Grid.TileSize / 2,
                            AppConstants.GridY + gridPos.Y * Grid.TileSize + Grid.TileSize / 2),
                            LayerDepth.Default,
                            AppColors.AtomInvalidPos);
                    }
                }
            }

            Batch.End();
        }

        #endregion

        #region Properties

        [Store]
        public AppContents AppContents { get; set; }

        public Atom CurrentAtom { get; private set; }
        public Atom NextAtom { get; private set; }

        public AtomsGrid Grid { get; private set; }

        public GridRenderer GridRenderer { get; private set; }

        public GameSession Session { get; private set; }

        #endregion
    }
}