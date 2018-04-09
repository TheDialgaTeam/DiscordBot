using System.IO;

namespace TheDialgaTeam.Modules.System.IO
{
    public static class PathExtensionMethods
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