#if UNITY_EDITOR
using System;

namespace XiheFramework.Core.Utility.Extension
{
    public static class EditorTime
    {
        public static string TimeStamp => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds().ToString();

        public static long Time => new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }

}
#endif