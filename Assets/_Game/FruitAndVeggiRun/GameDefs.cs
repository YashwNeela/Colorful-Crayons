using UnityEngine;

/// <summary>Static definitions for the produce the player collects.</summary>
public static class GameDefs
{
    public static readonly string[] Names = { "tomato", "carrot", "potato", "apple", "banana" };

    public static Color ColorOf(string n)
    {
        switch (n)
        {
            case "tomato": return new Color(0.90f, 0.20f, 0.16f);
            case "carrot": return new Color(0.95f, 0.52f, 0.11f);
            case "potato": return new Color(0.72f, 0.55f, 0.33f);
            case "apple":  return new Color(0.90f, 0.19f, 0.35f);
            case "banana": return new Color(0.97f, 0.83f, 0.18f);
        }
        return Color.white;
    }

    public static string Display(string n)
    {
        if (string.IsNullOrEmpty(n)) return "";
        return n;
    }
}
