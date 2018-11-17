namespace Atomic.Entities
{
    public class GameSession
    {
        public void Reset(GameDifficulty difficulty)
        {
            Time = 0d;
            Difficulty = difficulty;

            Score = 0;
            Atoms = 0;
            Molecules = 0;
        }

        public double Time { get; set; }
        public GameDifficulty Difficulty { get; set; }

        public int Score { get; set; }
        public int Atoms { get; set; }
        public int Molecules { get; set; }
    }
}