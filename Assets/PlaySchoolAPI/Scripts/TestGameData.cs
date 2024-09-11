using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class TestGameData : MonoBehaviour
{
    public static TestGameData Instance;

    public string addstudenttestUrl = "http://13.200.173.193/api/StudentTests/user/addstudenttest";
    public string addstudentgameUrl = "http://13.200.173.193/api/StudentGames/user/addstudentgame";
    public string addstudentachievementUrl = "http://13.200.173.193/user/addstudentachievement";

    [SerializeField] private string authToken;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        authToken = PlayerPrefs.GetString(TMKOCPlaySchoolConstants.AuthorizationToken);
    }

    public void OnEndStudentTest(string attempts, string score, string testID)
    {
        StartCoroutine(AddStudentTest(attempts, score, testID));
    }

    public void OnEndStudentGame(bool isPlayed, string gameId)
    {
        StartCoroutine(AddStudentGame(isPlayed, gameId));
    }

    public void OnEndAllAchievement(string achievementId)
    {
        StartCoroutine(AddStudentAchievement(achievementId));
    }

    IEnumerator AddStudentTest(string attempts, string score, string testID)
    {
        WWWForm form = new WWWForm();
        form.AddField("attempts", attempts);
        form.AddField("score", score);
        form.AddField("testID", testID);

        UnityWebRequest request = UnityWebRequest.Post(addstudenttestUrl, form);
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Student Test Created!");
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    IEnumerator AddStudentGame(bool isPlayed, string gameId)
    {
        WWWForm form = new WWWForm();
        form.AddField("isPlayed", isPlayed.ToString());
        form.AddField("GameId", gameId);

        UnityWebRequest request = UnityWebRequest.Post(addstudentgameUrl, form);
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Student Game Created!");
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }

    IEnumerator AddStudentAchievement(string achievementId)
    {
        WWWForm form = new WWWForm();
        form.AddField("achievementId", achievementId);

        UnityWebRequest request = UnityWebRequest.Post(addstudentachievementUrl, form);
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.LogWarning("Student Achievement Exist Already!");
        }
        else
        {
            Debug.LogError("Error: " + request.error);
        }
    }
}
