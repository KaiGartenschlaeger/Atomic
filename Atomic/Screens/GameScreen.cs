using Atomic.Entities;
using Atomic.Services.Highscore;
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

        private readonly ISaveGameService _saves;
        private readonly IHighscoreService _highscores;
        private readonly Stack<int> _nextAtoms;

        #endregion

        #region Constructor

        public GameScreen(ISaveGameService saveService, IHighscoreService highscores)
        {
            _saves = saveService;
            _highscores = highscores;

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
            Session.NewGame();

            Grid.Clear();

            _nextAtoms.Clear();
            Session.CurrentAtom = GetNextAtom();
            Session.NextAtom = GetNextAtom();
        }

        public void ContinueLastGame()
        {
            var data = _saves.LoadGame(AppConstants.LastSaveGameFilename);

            Session.NewGame();

            Session.Time = data.ElapsedTime.TotalSeconds;
            Session.Score = data.Score;
            Session.Atoms = data.Atoms;
            Session.Molecules = data.Molecules;

            _nextAtoms.Clear();
            Session.CurrentAtom = Grid.CreateAtom(data.CurrentAtom);
            Session.NextAtom = Grid.CreateAtom(data.NextAtom);

            Grid.FromSaveGame(data.GridData);
        }

        public void SaveGame()
        {
            var data = new SaveGameData();
            data.ElapsedTime = TimeSpan.FromSeconds(Session.Time);

            data.Score = Session.Score;
            data.Atoms = Session.Atoms;
            data.Molecules = Session.Molecules;

            data.AddedAtoms = Session.AddedAtoms;

            data.CurrentAtom = Session.CurrentAtom.Electrons;
            data.NextAtom = Session.NextAtom.Electrons;

            data.GridData = new SaveGameGridData[Grid.Width, Grid.Height];
            for (int gridX = 0; gridX < Grid.Width; gridX++)
            {
                for (int gridY = 0; gridY < Grid.Height; gridY++)
                {
                    var atom = Grid.GetAtom(gridX, gridY);
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

            _saves.SaveGame(AppConstants.LastSaveGameFilename, data);
        }

        /// <summary>
        /// Checks after each added atom for game over state
        /// </summary>
        private bool CheckGameOver()
        {
            for (int gridX = 0; gridX < Grid.Width; gridX++)
            {
                for (int gridY = 0; gridY < Grid.Height; gridY++)
                {
                    if (!Grid.HasAtom(gridX, gridY)) return false;
                }
            }

            return true;
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
            if (Session.IsGameOver)
            {
                HandleGameOver();
            }
            else
            {
                Session.Time += time.ElapsedSeconds();

                if (Session.CurrentAtom != null &&
                    IsMouseOverGrid() &&
                    Mouse.IsButtonPressed(MouseButton.Left))
                {
                    var gridPos = GetMouseGridPos();
                    if (Grid.SetAtom(gridPos.X, gridPos.Y, Session.CurrentAtom))
                    {
                        Session.AddedAtoms++;

                        Session.CurrentAtom = Session.NextAtom;
                        Session.NextAtom = GetNextAtom();

                        Session.IsGameOver = CheckGameOver();
                    }
                }

                if (Keyboard.IsKeyReleased(Keys.Escape))
                    Manager.Activate<GameMenuScreen>();

#if CHEATS_ENABLED
                HandleCheats();
#endif
            }
        }

        private void HandleGameOver()
        {
            // delete last game
            _saves.DeleteSaveGame(AppConstants.LastSaveGameFilename);

            // add to highscore list
            var highscorePlace = _highscores.Add(
                Environment.UserName,
                TimeSpan.FromSeconds(Session.Time),
                Session.Score,
                Session.Atoms,
                Session.Molecules);

            GetScreen<HighscoreScreen>().HasChanges = true;

            // switch screen
            if (highscorePlace != -1 && highscorePlace < AppConstants.TopHighscorePlaces)
                Manager.SwitchTo<HighscoreScreen>();
            else
                Manager.SwitchTo<GameOverScreen>();
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

            batch.Begin(SpriteSortMode.FrontToBack);

            // grid
            GridRenderer.Draw(batch, new Vector2(AppConstants.GridX, AppConstants.GridY));

            batch.End();


            batch.Begin();

            var y = AppConstants.GridY + 10;

            // time
            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), "Time:", AppColors.Descriptions);
            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.WindowWidth - AppConstants.ScreenPadding - 140, y), TimeSpan.FromSeconds(Session.Time).ToString(@"hh\:mm\:ss"), AppColors.Texts);
            y += AppContents.DefaultFont.Data.LineHeight + 15;

            // score
            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), $"Score:", AppColors.Descriptions);
            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.WindowWidth - AppConstants.ScreenPadding - 140, y), Session.Score.ToString("n0"), AppColors.Texts);
            y += AppContents.DefaultFont.Data.LineHeight + 15;

            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), $"Atoms:", AppColors.Descriptions);
            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.WindowWidth - AppConstants.ScreenPadding - 140, y), Session.Atoms.ToString("n0"), AppColors.Texts);
            y += AppContents.DefaultFont.Data.LineHeight + 5;

            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), $"Molecules:", AppColors.Descriptions);
            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.WindowWidth - AppConstants.ScreenPadding - 140, y), Session.Molecules.ToString("n0"), AppColors.Texts);
            y += AppContents.DefaultFont.Data.LineHeight + 5;

            // current/next atom
            y = AppConstants.WindowHeight - AppConstants.ScreenPadding - AppConstants.PreviewBoxHeight - AppContents.DefaultFont.Data.LineHeight - 8;

            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight, y), "Current", AppColors.Descriptions);
            batch.DrawBitmapFont(AppContents.DefaultFont, new Vector2(AppConstants.GridRight + AppConstants.PreviewBoxWidth + AppConstants.PreviewBoxPadding, y), "Next", AppColors.Descriptions);
            y += AppContents.DefaultFont.Data.LineHeight + 8;

            batch.DrawRect(new Rectangle(AppConstants.GridRight, y, AppConstants.PreviewBoxWidth, AppConstants.PreviewBoxHeight), 1, AppColors.PreviewBorder);
            batch.DrawRect(new Rectangle(AppConstants.GridRight + AppConstants.PreviewBoxWidth + AppConstants.PreviewBoxPadding, y, AppConstants.PreviewBoxWidth, AppConstants.PreviewBoxHeight), 1, AppColors.PreviewBorder);

            if (Session.CurrentAtom != null)
            {
                Session.CurrentAtom.Draw(batch,
                    pos: new Vector2(
                        AppConstants.GridRight + AppConstants.PreviewBoxWidth / 2,
                        y + AppConstants.PreviewBoxHeight / 2), layerDepth: LayerDepth.Default);
            }

            if (Session.NextAtom != null)
            {
                Session.NextAtom.Draw(batch,
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
                        Session.CurrentAtom.Draw(batch, new Vector2(
                            AppConstants.GridX + gridPos.X * Grid.TileSize + Grid.TileSize / 2,
                            AppConstants.GridY + gridPos.Y * Grid.TileSize + Grid.TileSize / 2),
                            LayerDepth.Default,
                            AppColors.AtomValidPos);
                    }
                    else
                    {
                        Session.CurrentAtom.Draw(batch, new Vector2(
                            AppConstants.GridX + gridPos.X * Grid.TileSize + Grid.TileSize / 2,
                            AppConstants.GridY + gridPos.Y * Grid.TileSize + Grid.TileSize / 2),
                            LayerDepth.Default,
                            AppColors.AtomInvalidPos);
                    }
                }
            }

            batch.End();
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