using System;

namespace Atomic.Services.Highscore
{
    public interface IHighscoreService
    {
        #region Methods

        void Load();

        void Save();

        int Add(string user, TimeSpan time, int score, int atoms, int molecules);

        #endregion

        #region Properties

        Highscore[] Items { get; }

        #endregion
    }
}