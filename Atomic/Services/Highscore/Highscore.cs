using System;

namespace Atomic.Services.Highscore
{
    public class Highscore
    {
        #region Properties

        public string User { get; set; }

        public TimeSpan Time { get; set; }

        public int Score { get; set; }

        public int Atoms { get; set; }

        public int Molecules { get; set; }

        #endregion
    }
}