namespace Atomic
{
    internal static class AppConstants
    {
        public const string AppDataDirectoryName = "Atomics";

        public const int SidebarWidth = 340;

        public const int GridTileSize = 64;
        public const int GridWidth = 8;
        public const int GridHeight = 10;

        public const int ScreenPadding = 45;

        public const int PreviewBoxWidth = (int)(GridTileSize * 2.5f);
        public const int PreviewBoxHeight = GridTileSize * 2;
        public const int PreviewBoxPadding = 15;

        public const int GridX = ScreenPadding;
        public const int GridY = ScreenPadding;
        public const int GridRight = GridX + GridWidth * GridTileSize + ScreenPadding;

        public const int WindowWidth = ScreenPadding + GridWidth * GridTileSize + ScreenPadding + SidebarWidth + ScreenPadding;
        public const int WindowHeight = GridHeight * GridTileSize + ScreenPadding * 2;

        public const int MenuPadding = 8;

        public const string LastSaveGameFilename = "LastGame";
        public const string HighscoreFilename = "Highscores";

        public const int TopHighscorePlaces = 10;
    }
}