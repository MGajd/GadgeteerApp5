using System;
using Microsoft.SPOT;

namespace GadgeteerApp5
{
    public static class HelpFunctions
    {
        private static string emptyString = "                ";
     
        public static string GetEmptyText(int number, bool fillLine)
        {
            int length = fillLine ? 16 - number : number;
            return emptyString.Substring(0, length);
        }
    }
}
