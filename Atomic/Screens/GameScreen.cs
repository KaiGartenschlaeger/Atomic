﻿using Atomic.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PureFreak.TileMore.Input;
using PureFreak.TileMore.Screens;

namespace Atomic.Screens
{
    public class GameScreen : Screen
    {
        #region Screen methods

        protected override void OnStart()
        {
            Session = new GameSession();
            Grid = new AtomsGrid(AppContents, Session, AppConstants.GridTileSize, AppConstants.GridWidth, AppConstants.GridHeight);
            GridRenderer = new GridRenderer(Grid, Session);

            CurrentAtom = Grid.CreateAtom();
            NextAtom = Grid.CreateAtom();
        }

        protected override void OnUpdate(GameTime time, int updateCounter)
        {
            if (CurrentAtom != null)
                CurrentAtom.Update(time);
            if (NextAtom != null)
                NextAtom.Update(time);

            if (CurrentAtom != null &&
                Mouse.IsButtonPressed(MouseButton.Left) &&
                Mouse.Position.X >= AppConstants.GridX &&
                Mouse.Position.Y >= AppConstants.GridY &&
                Mouse.Position.X <= AppConstants.GridX + Grid.PixelWidth &&
                Mouse.Position.Y <= AppConstants.GridY + Grid.PixelHeight)
            {
                var tileX = (Mouse.Position.X - AppConstants.GridX) / Grid.TileSize;
                var tileY = (Mouse.Position.Y - AppConstants.GridY) / Grid.TileSize;

                if (Grid.SetAtom(tileX, tileY, CurrentAtom))
                {
                    CurrentAtom = NextAtom;
                    NextAtom = Grid.CreateAtom();
                }
            }

#if CHEATS_ENABLED

            var electrons = -1;

            if (Keyboard.IsKeyReleased(Keys.D1)) electrons = 1;
            else if (Keyboard.IsKeyReleased(Keys.D2)) electrons = 2;
            else if (Keyboard.IsKeyReleased(Keys.D3)) electrons = 3;
            else if (Keyboard.IsKeyReleased(Keys.D4)) electrons = 4;

            if (CurrentAtom != null && electrons != -1)
                CurrentAtom.Electrons = electrons;

            if (Keyboard.IsKeyReleased(Keys.R))
                Grid.Clear();

#endif

            GridRenderer.Update(time);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            GraphicsDevice.Clear(AppColors.WindowBackground);

            Batch.Begin();

            // grid
            GridRenderer.Draw(Batch, new Vector2(AppConstants.GridX, AppConstants.GridY));

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
                CurrentAtom.Draw(Batch, new Vector2(AppConstants.GridRight + AppConstants.PreviewBoxWidth / 2, y + AppConstants.PreviewBoxHeight / 2));
            if (NextAtom != null)
                NextAtom.Draw(Batch, new Vector2(AppConstants.GridRight + AppConstants.PreviewBoxWidth + AppConstants.PreviewBoxPadding + AppConstants.PreviewBoxWidth / 2, y + AppConstants.PreviewBoxHeight / 2));

            // atom grid preview
            if (CurrentAtom != null &&
                Mouse.Position.X >= AppConstants.GridX &&
                Mouse.Position.Y >= AppConstants.GridY &&
                Mouse.Position.X <= AppConstants.GridX + Grid.PixelWidth &&
                Mouse.Position.Y <= AppConstants.GridY + Grid.PixelHeight)
            {
                var tileX = (Mouse.Position.X - AppConstants.GridX) / Grid.TileSize;
                var tileY = (Mouse.Position.Y - AppConstants.GridY) / Grid.TileSize;

                if (Grid.IsValidPos(tileX, tileY) && !Grid.HasAtom(tileX, tileY))
                {
                    CurrentAtom.Draw(Batch, new Vector2(
                        AppConstants.GridX + tileX * Grid.TileSize + Grid.TileSize / 2,
                        AppConstants.GridY + tileY * Grid.TileSize + Grid.TileSize / 2),
                        Color.LightGray);
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