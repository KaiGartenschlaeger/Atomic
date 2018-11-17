﻿namespace Atomic.Services
{
    public interface ISaveGameService
    {
        bool HasSaveGame(string filename);

        void SaveGame(string filename, SaveGameData data);

        SaveGameData LoadGame(string filename);
    }
}