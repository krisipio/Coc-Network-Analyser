using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClashRoyale_NetworkAnalyser.Utils
{
    public static class Utilsa
    {
        public static string BytesToString(byte[] bytes)
        {
            var str = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
                str += bytes[i].ToString("x2").ToUpper() + " ";
            return str;
        }
    }
}
