using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SocialPlatforms.Impl;

public class StudentGameProgressApi : MonoBehaviour
{
    public static StudentGameProgressApi Instance { get; private set; }
    public StudentGameData CurrentGameData { get; private set; }
    public StudentTestGameData CurrentGameTestData { get; private set; }

    public static Action OnStudentAPIInitialized;

    private bool isInitialized;

    public bool IsInitialized => isInitialized;


    public bool isGet;

    public int gameId = 1;

    public string GetAuth;

    private void Awake()
    {


        if (Instance == null)
        {

            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {

        if (!PlayerPrefs.HasKey(TMKOCPlaySchoolConstants.LastAttendanceKey))
        {
            // Set the key to yesterday's date
            PlayerPrefs.SetString(TMKOCPlaySchoolConstants.LastAttendanceKey, DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd"));
        }

        //if (isGet) 
        //{

        //GetStudentIsPlayingLevel("TestName", gameId);
        //}
        //else
        //{

        //    AddStudentIsPlayingLevel("TestName", gameId);
        //}
        //StartCoroutine(AddStudentAttendance("TestName2" , true));

#if PLAYSCHOOL_MAIN
        GetAuth = PlayerPrefs.GetString(TMKOCPlaySchoolConstants.AuthorizationToken);
        OnStudentAPIInitialized?.Invoke();
        isInitialized = true;
#else
        GetAuth = TMKOCPlaySchoolConstants.AuthorizationToken;
        OnStudentAPIInitialized?.Invoke();
        isInitialized = true;

#endif
    }
    public void SetGameData(StudentGameData data)
    {
        CurrentGameData = data;

    }

    public void SetGameTestData(StudentTestGameData data)
    {
        CurrentGameTestData = data;

        //Debug.Log(CurrentGameData.data.stars);
        //Debug.Log(CurrentGameData.data.totalLevel);

        //Debug.Log("asljidjc " + CalculateStars(CurrentGameData.data.completedLevel, CurrentGameData.data.totalLevel));

    }

    public void GetStudentByGameId(string StudentName, int GameId, Action status)
    {
        StartCoroutine(GetStudentGameByIdApi(StudentName, GameId, status));

    }

    public void AddStudentByGameId(string StudentName, int stars, int completedLevel, int totalLevel, int attempts, long timeSpentInSeconds, int score, int GameId, Action callback
        )
    {
        //TestName
        StartCoroutine(AddStudentGameByIdApi(StudentName, stars, completedLevel, totalLevel, attempts, timeSpentInSeconds, score, GameId, callback));

    }
    public void GetStudentByTestsId(string StudentName, int TestId, Action callback)
    {
        //TestName
        StartCoroutine(GetStudentTestById(StudentName, TestId, callback));

    }

    public void AddStudentByTestsId(string StudentName, int stars, int medal, int scores, int attempts, int totalQuestions, int streak, long timeSpentInSeconds, int TestId, Action callback)
    {
        //TestName
        StartCoroutine(AddStudentTestById(StudentName, stars, medal, scores, attempts, totalQuestions, streak, timeSpentInSeconds, TestId, callback));

    }
    private float startTime;
    public long EndGame(float startime)
    {
        float endTime = Time.time;
        float timeSpentInSeconds = endTime - startime;

        return (long)timeSpentInSeconds;
    }

    public int CalculateStars(int completedLevel, int totalLevel)
    {
        float percentage = (float)completedLevel / totalLevel * 100;
        Debug.Log("perceontage " + percentage);
        if (percentage <= 20) return 1;
        else if (percentage <= 40) return 2;
        else if (percentage <= 60) return 3;
        else if (percentage <= 80) return 4;
        else if (percentage == 100) return 5;

        return 4;
    }
    IEnumerator AddStudentGameByIdApi(string StudentName, int stars, int completedLevel, int totalLevel, int attempts, long timeSpentInSeconds, int score, int GameId, Action callback)
    {
        //todo off the api
        WWWForm form = new WWWForm();
        form.AddField("StudentName", StudentName);
        form.AddField("Stars", stars.ToString());
        form.AddField("CompletedLevel", completedLevel.ToString());
        form.AddField("TotalLevel", totalLevel.ToString());
        form.AddField("Attempts", attempts.ToString());
        form.AddField("TimeSpentInSeconds", timeSpentInSeconds.ToString());
        form.AddField("Score", score.ToString());
        form.AddField("GameId", GameId.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(Constants.AddDataStudentGameById, form))
        {
            print(GetAuth);
            request.SetRequestHeader("Authorization", "Bearer " + GetAuth);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                var jsonResponse = request.downloadHandler.text;
                Debug.Log(jsonResponse);
                callback?.Invoke();

                CheckAndMarkAttendance(StudentName);
            }
        }
    }

    IEnumerator GetStudentGameByIdApi(string StudentName, int id, Action status)
    {
        // yield return new WaitForSeconds(1);
        WWWForm form = new WWWForm();
        form.AddField("StudentName", StudentName);

        Debug.Log("Student Na, " + StudentName);
        form.AddField("GameId", id.ToString());
        print(GetAuth);
        using (UnityWebRequest request = UnityWebRequest.Post(Constants.GetDataStudentGameById, form))
        {
            request.SetRequestHeader("Authorization", "Bearer " + GetAuth);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                var jsonResponse = request.downloadHandler.text;
                Debug.Log(jsonResponse + " get");
                StudentGameData studentGameData = JsonUtility.FromJson<StudentGameData>(jsonResponse);


                SetGameData(studentGameData);
                status?.Invoke();


                Debug.Log(jsonResponse);

            }
        }
    }

    private void CheckAndMarkAttendance(string studentName)
    {
        DateTime today = DateTime.Now;
        TimeSpan timeSpan = today.Date - DateTime.Parse(PlayerPrefs.GetString(TMKOCPlaySchoolConstants.LastAttendanceKey, DateTime.Now.ToString())).Date;

        Debug.Log(" time span " + timeSpan.Days);
        Debug.Log(" time span today  " + today + "  " + PlayerPrefs.GetString(TMKOCPlaySchoolConstants.LastAttendanceKey));
        if (timeSpan.Days >= 1)
        {
            Debug.Log("Marking attendance for today");
            PlayerPrefs.SetString(TMKOCPlaySchoolConstants.LastAttendanceKey, DateTime.Now.ToString());
            StartCoroutine(AddStudentAttendance(studentName, true));
        }
        else
        {
            Debug.Log("Attendance already marked for today");
        }

    }


    IEnumerator AddStudentTestById(string StudentName, int stars, int medal, int scores, int attempts, int totalQuestions, int streak, long timeSpentInSeconds, int TestId, Action callback)
    {

        WWWForm form = new WWWForm();
        form.AddField("StudentName", StudentName);
        form.AddField("Stars", stars.ToString());
        form.AddField("Medal", medal.ToString());
        form.AddField("Scores", scores.ToString());
        form.AddField("Attempts", attempts.ToString());
        form.AddField("TotalQuestions", totalQuestions.ToString());
        form.AddField("Streak", streak.ToString());
        form.AddField("TimeSpentInSeconds", timeSpentInSeconds.ToString());
        form.AddField("TestId", TestId.ToString());

        using (UnityWebRequest request = UnityWebRequest.Post(Constants.AddDataStudentTestById, form))
        {
            request.SetRequestHeader("Authorization", "Bearer " + GetAuth);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                var jsonResponse = request.downloadHandler.text;


                callback?.Invoke();
                Debug.Log("jsin parce " + jsonResponse);


            }
        }
    }

    IEnumerator GetStudentTestById(string StudentName, int testId, Action callback)
    {

        WWWForm form = new WWWForm();
        form.AddField("StudentName", StudentName);
        form.AddField("TestId", testId.ToString());
        using (UnityWebRequest request = UnityWebRequest.Post(Constants.GetDataStudentTestById, form))
        {
            request.SetRequestHeader("Authorization", "Bearer " + GetAuth);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                var jsonResponse = request.downloadHandler.text;
                Debug.Log("jsin parce " + jsonResponse);
                StudentTestGameData studentTestGameData = JsonUtility.FromJson<StudentTestGameData>(jsonResponse);

                if (studentTestGameData.status)
                {
                    SetGameTestData(studentTestGameData);
                }else
                {
                    CurrentGameData = new StudentGameData();
                    CurrentGameData.data = new StudentGameData.Data();
                }

                callback?.Invoke();


            }
        }
    }




    public IEnumerator AddStudentAttendance(string StudentName, bool IsPresent)
    {
        yield return new WaitForSeconds(1);
        WWWForm form = new WWWForm();
        form.AddField("StudentName", StudentName);
        form.AddField("IsPresent", IsPresent.ToString());
        using (UnityWebRequest request = UnityWebRequest.Post(Constants.AddAttendanceUrl, form))
        {
            request.SetRequestHeader("Authorization", "Bearer " + GetAuth);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                var jsonResponse = request.downloadHandler.text;
                Debug.Log("jsin parce " + jsonResponse);

            }
        }

    }
}


[Serializable]
public class StudentGameData
{
    public bool status;
    public Data data;

    [Serializable]
    public class Data
    {
        public int id;
        public int stars;
        public int completedLevel;
        public int totalLevel;
        public int attempts;
        public long timeSpentInSeconds;
        public DateTime createdAt;
        public DateTime updatedAt;
        public bool isDeleted;



        public bool IsTimeSpentValid(int gameTime)
        {
            return timeSpentInSeconds >= gameTime;
        }
    }
}

[Serializable]
public class StudentTestGameData
{
    public bool status;
    public Data data;

    [Serializable]
    public class Data
    {
        public int id;
        public int stars;
        public int scores;
        public int attempts;
        public int totalQuestions;
        public int streak;
        public int timeSpentInSeconds;
        public int earnedMedal;
        public int studentId;
        public int testId;
        public DateTime createdAt;
        public DateTime updatedAt;
        public bool isDeleted;



        public bool IsTimeSpentValid(int gameTime)
        {
            return timeSpentInSeconds >= gameTime;
        }
    }
}