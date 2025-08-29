using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace DK_Collection.Utils
{
    internal class Utils
    {
        public static void Log(string message, bool isError = false)
        {
            var color = new Color(0.333f, 0.612f, 0.839f);
            if (isError)
            {
                color = new Color(0.9f, 0f, 0f);
            }
            InformationManager.DisplayMessage(new InformationMessage(message, color));
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
    }
}
