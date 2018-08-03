using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultiRemoteController.Utilities
{
    class DataValidator
    {
        private static Regex mRegex;

        public static bool IsIP(string ip)
        {
            mRegex = new Regex(@"(?=(\b|\D))(((\d{1,2})|(1\d{1,2})|(2[0-4]\d)|(25[0-5]))\.){3}((\d{1,2})|(1\d{1,2})|(2[0-4]\d)|(25[0-5]))(?=(\b|\D))");
            Match match = mRegex.Match(ip);
            return match.Success;
        }

        public static bool IsValidPort(int port)
        {
            if(port >= 0 && port <= 65535)
            {
                if(port <= 1024)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool IsFilePath(string path)
        {
            mRegex = new Regex(@"[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\]+)+\\?)|(\\)?)\s*$");
            Match match = mRegex.Match(path);
            return match.Success;
        }
    }
}
