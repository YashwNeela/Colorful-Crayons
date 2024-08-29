using UnityEngine;

public static class ColorCodes
{
    public static Color red;
    public static Color yellow;
    public static Color green;
    public static Color blue;

    // Static constructor to initialize the colors
    static ColorCodes()
    {
        // Parse each color code; if parsing fails, set to a default color and log an error
        if (!ColorUtility.TryParseHtmlString("#FF0000", out red))
        {
            Debug.LogError("Failed to parse the color code #FF0000 for red.");
            red = Color.red; // Setting to Unity's default red color as fallback
        }

        if (!ColorUtility.TryParseHtmlString("#FFFF00", out yellow))
        {
            Debug.LogError("Failed to parse the color code #FFFF00 for yellow.");
            yellow = Color.yellow; // Setting to Unity's default yellow color as fallback
        }

        if (!ColorUtility.TryParseHtmlString("#00FF00", out green))
        {
            Debug.LogError("Failed to parse the color code #00FF00 for green.");
            green = Color.green; // Setting to Unity's default green color as fallback
        }

        if (!ColorUtility.TryParseHtmlString("#0038FF", out blue))
        {
            Debug.LogError("Failed to parse the color code #0038FF for blue.");
            blue = Color.blue; // Setting to Unity's default blue color as fallback
        }
    }
}
