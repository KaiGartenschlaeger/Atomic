using Atomic.Services.Settings;
using Atomic.Services.Sounds;
using Atomic.UI;
using Atomic.UI.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PureFreak.TileMore;
using PureFreak.TileMore.Screens;

namespace Atomic.Screens
{
    public class SettingsScreen : Screen
    {
        #region Fields

        private readonly ISoundsManager _sounds;
        private readonly ISettingsService _settings;

        private IUIManager _ui;

        #endregion

        #region Constructor

        public SettingsScreen(ISoundsManager sounds, ISettingsService settings)
        {
            _sounds = sounds;
            _settings = settings;
        }

        #endregion

        #region Screen methods

        protected override void OnStart()
        {
            InitSettings();
            InitUI();
        }

        private void InitSettings()
        {
            _settings.Load();

            _sounds.Volume = _settings.Settings.Audio.EffectsVolume;
        }

        private void InitUI()
        {
            var skin = Store.Get<UISkin>("UISkin");
            _ui = new UIManager(skin);
            //_ui.DebugRects = true;

            // effects volume
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

            // cancel button
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

        protected override void OnEnd()
        {
            _settings.Save();
        }

        private void ChangeVolume(int offset)
        {
            int newValue = MathI.Clamp(_sounds.Volume + offset, 0, 100);

            if (_sounds.Volume != newValue)
            {
                _sounds.Volume = (byte)newValue;
                _sounds.PlaySound(SoundName.Blip5);

                _settings.Settings.Audio.EffectsVolume = _sounds.Volume;

                RefreshVolumeLabel();
            }
        }

        private void RefreshVolumeLabel()
        {
            var lblVolume = (Label)_ui.FindElement("lblVolume");
            lblVolume.Text = _sounds.Volume.ToString();
        }

        protected override void OnEnter()
        {
            RefreshVolumeLabel();
        }

        protected override void OnUpdate(GameTime time, int updateCounter)
        {
            if (Keyboard.IsKeyPressed(Keys.Escape))
                Manager.SwitchTo<StartMenuScreen>();

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