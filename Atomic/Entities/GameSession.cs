namespace Atomic.Entities
{
    public class GameSession
    {
        #region Methods

        public void NewGame()
        {
            GameStarted = true;
            IsGameOver = false;

            Time = 0d;

            Score = 0;
            Atoms = 0;
            Molecules = 0;

            AddedAtoms = 0;
        }

        #endregion

        #region Properties

        public bool GameStarted { get; set; }
        public bool IsGameOver { get; set; }

        public double Time { get; set; }

        public int Score { get; set; }
        public int Atoms { get; set; }
        public int Molecules { get; set; }

        public int AddedAtoms { get; set; }

        public Atom CurrentAtom { get; set; }
        public Atom NextAtom { get; set; }

        #endregion
    }
}