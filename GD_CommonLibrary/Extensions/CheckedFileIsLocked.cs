using System.Runtime.InteropServices;

namespace GD_CommonLibrary.Extensions
{
    public static partial class CommonExtensions
    {
        /// <summary>
        /// 檢查檔案是否被上鎖
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static bool IsFileLocked(string file)
        {
            try
            {
                using (File.Open(file, FileMode.Open, FileAccess.Write, FileShare.None))
                {
                    return false;
                }
            }
            catch (IOException exception)
            {
                var errorCode = Marshal.GetHRForException(exception) & 65535;
                //2 找不到檔案
                //32/33 被上鎖
                return errorCode == 32 || errorCode == 33;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
