using System.IO;

namespace TheDialgaTeam.DiscordBot.Extension.System.IO
{
    internal static class PathExtensionMethods
    {
        public static string ResolveFullPath(this string filePath)
        {
            return Path.GetFullPath(filePath.ResolveFilePath());
        }

        public static string ResolveFilePath(this string filePath)
        {
            return filePath.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        }
    }
}