using Atomic.Services.Settings.Entities;

namespace Atomic.Services.Settings
{
    public interface ISettingsService
    {
        #region Methods

        void Load();
        void Save();

        #endregion

        #region Properties

        SettingsElement Settings { get; }

        #endregion
    }
}