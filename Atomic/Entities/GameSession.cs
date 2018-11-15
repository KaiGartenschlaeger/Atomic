namespace Atomic.Entities
{
    public class GameSession
    {
        public void Reset()
        {
            Score = 0;
            Atoms = 0;
            Molecules = 0;
        }

        public int Score { get; set; }
        public int Atoms { get; set; }
        public int Molecules { get; set; }
    }
}