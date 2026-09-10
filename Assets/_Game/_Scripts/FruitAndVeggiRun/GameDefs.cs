using UnityEngine;

namespace TMKOC.FruitAndVeggiRun
{

    /// <summary>Static definitions for the produce the player collects.</summary>
    public static class GameDefs
    {
                public static readonly string[] Names =
        {
            "apple", "banana", "mango", "orange", "watermelon", "grapes", "lemon"
        };

        public static Color ColorOf(string n)
        {
            switch (n)
            {
                case "apple":      return new Color(0.79f, 0.14f, 0.13f);
                case "banana":     return new Color(0.96f, 0.80f, 0.03f);
                case "mango":      return new Color(0.93f, 0.68f, 0.07f);
                case "orange":     return new Color(0.97f, 0.51f, 0.01f);
                                case "watermelon": return new Color(0.88f, 0.24f, 0.32f);
                case "grapes":     return new Color(0.53f, 0.28f, 0.67f);
                case "lemon":      return new Color(0.95f, 0.87f, 0.20f);
            }
            return Color.white;
        }

        public static string Display(string n)
        {
            if (string.IsNullOrEmpty(n)) return "";
            return n;
        }
    }
}
