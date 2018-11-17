using Microsoft.Xna.Framework;
using PureFreak.TileMore.Helper;

namespace Atomic
{
    internal static class AppColors
    {
        public static readonly Color WindowBackground = new Color(40, 40, 40);

        public static readonly Color GridBorder = new Color(80, 80, 80);
        public static readonly Color GridCellBorder = new Color(60, 60, 60);

        public static readonly Color Descriptions = new Color(140, 140, 140);
        public static readonly Color Texts = new Color(220, 220, 220);

        public static readonly Color PreviewBorder = new Color(120, 120, 120);

        public static readonly Color AtomValidPos = new Color(20, 20, 20, 80);
        public static readonly Color AtomInvalidPos = new Color(80, 20, 20, 80);

        public static readonly Color Atom0 = ColorHelper.FromHexString("aedc77");
        public static readonly Color Atom1 = ColorHelper.FromHexString("dcdaab");
        public static readonly Color Atom2 = ColorHelper.FromHexString("dadc5c");
        public static readonly Color Atom3 = ColorHelper.FromHexString("dc9c13");
        public static readonly Color Atom4 = ColorHelper.FromHexString("dc2525");

        public static readonly Color MenuItems = new Color(140, 140, 140);
        public static readonly Color MenuItemsDisabled = new Color(80, 80, 80);
        public static readonly Color MenuItemsHover = new Color(220, 220, 220);
    }
}