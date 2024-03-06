namespace KickblipsTwo.IO
{
    using System.Collections.Generic;

    public static class SongInfoManager
    {
        internal static List<SongInfo> SongList { get; private set; } = new List<SongInfo>();
    }
}