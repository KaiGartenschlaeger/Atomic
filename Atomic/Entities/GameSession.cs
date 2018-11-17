namespace Atomic.Entities
{
    public class GameSession
    {
        #region Constructor

        public void Reset()
        {
            Time = 0d;

            Score = 0;
            Atoms = 0;
            Molecules = 0;
        }

        #endregion

        #region Properties

        public double Time { get; set; }

        public int Score { get; set; }
        public int Atoms { get; set; }
        public int Molecules { get; set; }

        public Atom CurrentAtom { get; set; }
        public Atom NextAtom { get; set; }

        #endregion
    }
}