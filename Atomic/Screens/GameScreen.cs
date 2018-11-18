using Atomic.Entities;
using Atomic.Services.SaveGames;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PureFreak.TileMore;
using PureFreak.TileMore.Input;
using PureFreak.TileMore.Screens;
using System;
using System.Collections.Generic;

namespace Atomic.Screens
{
    public class GameScreen : Screen
    {
        #region Fields

        private readonly ISaveGameService _saveService;
        private readonly Stack<int> _nextAtoms;

        #endregion

        #region Constructor

        public GameScreen(ISaveGameService saveService)
        {
            if (saveService == null)
                throw new ArgumentNullException(nameof(saveService));

            _saveService = saveService;
            _nextAtoms = new Stack<int>();
        }

        #endregion

        #region Helper

        private Atom GetNextAtom()
        {
            if (_nextAtoms.Count == 0)
            {
                // create next chunk of atoms
                var buffer = new int[40];
                for (int i = 0; i < buffer.Length; i++)
                    buffer[i] = 1 + i % 4;

                RandomHelper.Shuffle(buffer);

                for (int i = 0; i < buffer.Length; i++)
                    _nextAtoms.Push(buffer[i]);
            }

            return Grid.CreateAtom(_nextAtoms.Pop());
        }

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

            _nextAtoms.Clear();
            Session.CurrentAtom = GetNextAtom();
            Session.NextAtom = GetNextAtom();
        }

        public void ContinueLastGame()
        {
            var data = _saveService.LoadGame(AppConstants.LastSaveGameFileName);

            Session.Reset();
            Session.Time = data.ElapsedTime.TotalSeconds;
            Session.Score = data.Score;
            Session.Atoms = data.Atoms;
            Session.Molecules = data.Molecules;

            _nextAtoms.Clear();
            Session.CurrentAtom = Grid.CreateAtom(data.CurrentAtom);
            Session.NextAtom = Grid.CreateAtom(data.NextAtom);

            Grid.FromSaveGame(data.GridData);
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
            if (Session.CurrentAtom != null)
                Session.CurrentAtom.Update(time);
            if (Session.NextAtom != null)
                Session.NextAtom.Update(time);

            GridRenderer.Update(time);
        }

        protected override void OnInput(GameTime time, int updateCounter)
        {
            Session.Time += time.ElapsedSeconds();

            if (Session.CurrentAtom != null &&
                IsMouseOverGrid() &&
                Mouse.IsButtonPressed(MouseButton.Left))
            {
                var gridPos = GetMouseGridPos();
                if (Grid.SetAtom(gridPos.X, gridPos.Y, Session.CurrentAtom))
                {
                    Session.CurrentAtom = Session.NextAtom;
                    Session.NextAtom = GetNextAtom();
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

            if (Session.CurrentAtom != null && electrons != -1)
                Session.CurrentAtom.Electrons = electrons;

            if (Keyboard.IsKeyReleased(Keys.R))
                Grid.Clear();

            if (IsMouseOverGrid() && Mouse.IsButtonPressed(MouseButton.Right))
            {
                var gridPos = GetMouseGridPos();
                Grid.RemoveAtom(gridPos.X, gridPos.Y);
            }
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

            // time
            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), "Time:", AppColors.Descriptions);
            Batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.WindowWidth - AppConstants.ScreenPadding - 140, y), TimeSpan.FromSeconds(Session.Time).ToString(@"hh\:mm\:ss"), AppColors.Texts);
            y += AppContents.DefaultFont.Data.LineHeight + 15;

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

            if (Session.CurrentAtom != null)
            {
                Session.CurrentAtom.Draw(Batch,
                    pos: new Vector2(
                        AppConstants.GridRight + AppConstants.PreviewBoxWidth / 2,
                        y + AppConstants.PreviewBoxHeight / 2), layerDepth: LayerDepth.Default);
            }

            if (Session.NextAtom != null)
            {
                Session.NextAtom.Draw(Batch,
                    pos: new Vector2(
                        AppConstants.GridRight + AppConstants.PreviewBoxWidth + AppConstants.PreviewBoxPadding + AppConstants.PreviewBoxWidth / 2,
                        y + AppConstants.PreviewBoxHeight / 2),
                    layerDepth: LayerDepth.Default);
            }

            // atom grid preview
            if (Session.CurrentAtom != null && IsMouseOverGrid() && IsOnTop)
            {
                var gridPos = GetMouseGridPos();

                if (Grid.IsValidPos(gridPos.X, gridPos.Y) && !Grid.HasAtom(gridPos.X, gridPos.Y))
                {
                    if (Grid.CanSet(gridPos.X, gridPos.Y, Session.CurrentAtom))
                    {
                        Session.CurrentAtom.Draw(Batch, new Vector2(
                            AppConstants.GridX + gridPos.X * Grid.TileSize + Grid.TileSize / 2,
                            AppConstants.GridY + gridPos.Y * Grid.TileSize + Grid.TileSize / 2),
                            LayerDepth.Default,
                            AppColors.AtomValidPos);
                    }
                    else
                    {
                        Session.CurrentAtom.Draw(Batch, new Vector2(
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

        public AtomsGrid Grid { get; private set; }
        public GridRenderer GridRenderer { get; private set; }

        public GameSession Session { get; private set; }

        #endregion
    }
}