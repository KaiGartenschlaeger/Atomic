namespace Atomic.Services.SaveGames
{
    public interface ISaveGameService
    {
        bool HasSaveGame(string filename);

        void SaveGame(string filename, SaveGameData data);

        SaveGameData LoadGame(string filename);

        void DeleteSaveGame(string filename);
    }
}