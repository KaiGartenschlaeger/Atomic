using Atomic.Services.Sounds;
using Atomic.UI;
using Atomic.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PureFreak.TileMore;
using PureFreak.TileMore.Screens;

namespace Atomic.Screens
{
    public class SettingsScreen : Screen
    {
        #region Fields

        private readonly ISoundsManager _soundsManager;
        private IUIManager _ui;

        #endregion

        #region Constructor

        public SettingsScreen(ISoundsManager soundsManager)
        {
            _soundsManager = soundsManager;
        }

        #endregion

        #region Screen methods

        protected override void OnStart()
        {
            var skin = Store.Get<UISkin>("UISkin");
            _ui = new UIManager(skin);
            //_ui.DebugRects = true;

            _ui.Create<Label>(l =>
            {
                l.Text = "Volume:";
                l.Pos = new Vector2(100, 100);
                l.ColorHovered = AppColors.MenuItems;
            });

            _ui.Create<Label>(l =>
            {
                l.Name = "lblDecreaseVolume";
                l.Pos = new Vector2(240, 100);
                l.AutoSize = false;
                l.Width = 20;
                l.Height = skin.DefaultFont.Data.LineHeight;
                l.Text = "-";
                l.TextAlignment = TextAlignment.Right;

                l.Clicked += () =>
                {
                    _soundsManager.Volume -= 5;
                    RefreshVolumeLabel();
                };
            });
            _ui.Create<Label>(l =>
            {
                l.Name = "lblVolume";
                l.Pos = new Vector2(260, 100);
                l.AutoSize = false;
                l.Width = 80;
                l.Height = skin.DefaultFont.Data.LineHeight;
                l.TextAlignment = TextAlignment.Center;

                l.Clicked += () =>
                {
                    _soundsManager.Volume = 50;
                    RefreshVolumeLabel();
                };
            });
            _ui.Create<Label>(l =>
            {
                l.Name = "lblIncreaseVolume";
                l.Pos = new Vector2(340, 100);
                l.AutoSize = false;
                l.Width = 20;
                l.Height = skin.DefaultFont.Data.LineHeight;
                l.Text = "+";
                l.TextAlignment = TextAlignment.Left;

                l.Clicked += () =>
                {
                    _soundsManager.Volume += 5;
                    RefreshVolumeLabel();
                };
            });

            _ui.Create<Label>(l =>
            {
                l.Name = "lblCancel";
                l.AutoSize = false;
                l.Text = "Close";
                l.Width = (int)skin.DefaultFont.MeasureString(l.Text).X;
                l.Height = skin.DefaultFont.Data.LineHeight;
                l.Pos = new Vector2(GraphicsDevice.Viewport.Width / 2 - l.Width / 2, GraphicsDevice.Viewport.Height - 100);
                l.TextAlignment = TextAlignment.Center;

                l.Clicked += () =>
                {
                    Manager.SwitchTo<StartMenuScreen>();
                };
            });
        }

        private void RefreshVolumeLabel()
        {
            var lblVolume = (Label)_ui.FindElement("lblVolume");
            lblVolume.Text = _soundsManager.Volume.ToString();
        }

        protected override void OnEnter()
        {
            RefreshVolumeLabel();
        }

        protected override void OnUpdate(GameTime time, int updateCounter)
        {
            _ui.Update(time, Mouse, Keyboard);
        }

        protected override void OnDraw(SpriteBatch batch)
        {
            GraphicsDevice.Clear(AppColors.WindowBackground);

            _ui.Draw(batch);
        }

        #endregion
    }
}