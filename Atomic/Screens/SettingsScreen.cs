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
        // todo: Save settings

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
                l.Text = "<";
                l.TextAlignment = TextAlignment.Right;

                l.Clicked += () => { ChangeVolume(-5); };
            });
            _ui.Create<Label>(l =>
            {
                l.Name = "lblVolume";
                l.Pos = new Vector2(260, 100);
                l.AutoSize = false;
                l.Width = (int)skin.DefaultFont.MeasureString("100 ").X;
                l.Height = skin.DefaultFont.Data.LineHeight;
                l.TextAlignment = TextAlignment.Center;

                l.MouseWheel += (delta) =>
                {
                    ChangeVolume(delta < 0 ? -5 : 5);
                };
            });
            _ui.Create<Label>(l =>
            {
                l.Name = "lblIncreaseVolume";
                l.Pos = new Vector2(260 + _ui.FindElement("lblVolume").Width, 100);
                l.AutoSize = false;
                l.Width = 20;
                l.Height = skin.DefaultFont.Data.LineHeight;
                l.Text = ">";
                l.TextAlignment = TextAlignment.Left;

                l.Clicked += () =>
                {
                    ChangeVolume(+5);
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

        private void ChangeVolume(int offset)
        {
            int newValue = MathI.Clamp(_soundsManager.Volume + offset, 0, 100);

            if (_soundsManager.Volume != newValue)
            {
                _soundsManager.Volume = (byte)newValue;
                _soundsManager.PlaySound(SoundName.Blip5);

                RefreshVolumeLabel();
            }
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