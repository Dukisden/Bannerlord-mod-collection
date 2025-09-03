using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace DukisCollection
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

        public static bool IsFamily(Hero hero)
        {
            bool check1 = hero.Siblings.Contains(Hero.MainHero);
            bool check2 = hero.Father == Hero.MainHero || Hero.MainHero.Father == hero;
            bool check3 = hero.Mother == Hero.MainHero || Hero.MainHero.Mother == hero;

            return (check1 || check2 || check3);
        }
    }
}
