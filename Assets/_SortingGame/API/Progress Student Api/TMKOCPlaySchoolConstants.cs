using UnityEngine;

public static class TMKOCPlaySchoolConstants
{
    #region Scene Names
    public static string TMKOCPlayMainMenu = "PlayschoolMainScene";
    public static string TMKOCPlayBeachScene = "TMKOCPlayBeachScene";
    #endregion

    #region Game Names
    public static string Coloring = "Coloring";
    public static string MusicalInstruments = "MusicalInstruments";
    public static string Tracing = "Tracing";
    public static string JigsawPuzzles = "JigsawPuzzles";
    public static string FlashCards = "FlashCards";
    public static string BalloonPopping = "BalloonPopping";
    public static string VisualPerceptionGames = "VisualPerceptionGames";
    public static string WordGames = "WordGames";
    public static string CountingGames = "CountingGames";
    public static string Painting = "Painting";
    public static string FindTheLetter = "FindTheLetter";

    public static string MatchTheArrow = "MatchTheArrow";
    public static string CopyThePattern = "CopyThePattern";
    public static string MatchTheShadow = "MatchTheShadow";
    public static string MatchTheColour = "MatchTheColour";


    public static string WorldPersonalities = "WorldPersonalities";
    public static string PeekABoo = "PeekABoo";


    public static string SpotDifference = "SpotDifference";
    public static string FindHiddenObject = "FindHiddenObject";

    
    #endregion

    #region TokenString
    public static string AuthorizationToken = "authorizationToken";
    #endregion

    #region Lang Names
    public static string LangName = "LangName";
    public static string Hindi = "Hindi";
    public static string English = "English";
    #endregion


    #region Get Data
   // public static string GetAuthToken = PlayerPrefs.GetString(AuthorizationToken);
    public static int totalStudents = PlayerPrefs.GetInt("totalStudents");


    public static string currentStudentPlaying = "currentStudent";


    public static string saveDateTime = "saveDateTime";


    public static string LastAttendanceKey = "LastAttendanceDate";

    public static string currentStudentName = "TestName";
    #endregion

}