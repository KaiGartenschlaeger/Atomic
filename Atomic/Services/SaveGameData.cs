using System;

namespace Atomic.Services
{
    public class SaveGameData
    {
        public TimeSpan ElapsedTime { get; set; }

        public int Score { get; set; }
        public int Atoms { get; set; }
        public int Molecules { get; set; }

        public int CurrentAtom { get; set; }
        public int NextAtom { get; set; }

        public SaveGameGridData[,] GridData { get; set; }
    }
}