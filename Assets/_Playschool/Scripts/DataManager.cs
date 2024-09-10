using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DataManager
{
    StudentGameData studentGameData;
    public StudentGameData StudentGameData => studentGameData;

    StudentTestGameData studentTestGameData;
    public StudentTestGameData StudentTestData=> studentTestGameData; 
    // public int GameId;
    // public int totalLevels;
    long previousSessionTime;
    public long PreviousSessionTime => previousSessionTime;
    float startGameTime;
    public float StartGameTime => startGameTime;
    public bool isTesting;
    public DataManager() { }
    public DataManager(int GameId, float startGameTime, int maxLevel, bool isTesting)
    {
        studentGameData = new StudentGameData();
        studentGameData.data = new StudentGameData.Data();
        studentGameData.data.id = GameId;
        this.startGameTime = startGameTime;
        studentGameData.data.totalLevel = maxLevel;
        this.isTesting = false;
        Debug.Log("Max Level is" + studentGameData.data.totalLevel);
    }

    #region GameData
    public void FetchData(Action successCallback = null)
    {
        if (isTesting)
            return;
#if PLAYSCHOOL_MAIN
        StudentGameProgressApi.Instance.GetStudentByGameId(PlayerPrefs.GetString(TMKOCPlaySchoolConstants.currentStudentPlaying), studentGameData.data.id,
        () =>
        {
            Debug.Log("Data fetched from backend");
            // StudentGameData.Data tempData = StudentGameProgressApi.Instance.CurrentGameData.data;
            previousSessionTime = StudentGameProgressApi.Instance.CurrentGameData.data.timeSpentInSeconds;
            studentGameData.data.attempts = StudentGameProgressApi.Instance.CurrentGameData.data.attempts;
            studentGameData.data.completedLevel = StudentGameProgressApi.Instance.CurrentGameData.data.completedLevel;
            successCallback?.Invoke();
        });
#else
        StudentGameProgressApi.Instance.GetStudentByGameId(TMKOCPlaySchoolConstants.currentStudentName, studentGameData.data.id,
             () =>
             {
                 Debug.Log("Data fetched from backend");
                 // StudentGameData.Data tempData = StudentGameProgressApi.Instance.CurrentGameData.data;
                 previousSessionTime = StudentGameProgressApi.Instance.CurrentGameData.data.timeSpentInSeconds;
                 studentGameData.data.attempts = StudentGameProgressApi.Instance.CurrentGameData.data.attempts;
                 studentGameData.data.completedLevel = StudentGameProgressApi.Instance.CurrentGameData.data.completedLevel;
                 successCallback?.Invoke();
             });
#endif
        startGameTime = Time.time;
    }
    public void SendData(Action successCallback = null)
    {
        if (isTesting)
            return;
        int star = StudentGameProgressApi.Instance.CalculateStars(studentGameData.data.completedLevel, studentGameData.data.totalLevel);
        if (studentGameData.data.attempts >= 1)
            star = 5;
        long currentSesstionTime = StudentGameProgressApi.Instance.EndGame(startGameTime);
        currentSesstionTime += previousSessionTime;
        // Debug.Log("Max Level is" + studentGameData.totalLevel);
#if PLAYSCHOOL_MAIN
        StudentGameProgressApi.Instance.AddStudentByGameId(PlayerPrefs.GetString(TMKOCPlaySchoolConstants.currentStudentPlaying),
                   star, studentGameData.data.completedLevel, studentGameData.data.totalLevel, studentGameData.data.attempts, currentSesstionTime, 10, studentGameData.data.id,
                    () =>
                    {
                        Debug.Log("Data sent Successfully");
                        successCallback?.Invoke();
                    });
#else
        StudentGameProgressApi.Instance.AddStudentByGameId(TMKOCPlaySchoolConstants.currentStudentName,
                   star, studentGameData.data.completedLevel, studentGameData.data.totalLevel, studentGameData.data.attempts, currentSesstionTime, 10, studentGameData.data.id,
                    () =>
                   {
                       Debug.Log("Data sent Successfully");
                       successCallback?.Invoke();
                   });
#endif
    }
    public void OnLevelCompleted()
    {
        studentGameData.data.completedLevel++;
    }
    public void OnDecrementLevel()
    {
        studentGameData.data.completedLevel--;
    }
    /// <summary>
    /// Call when user has finished all the levels
    /// </summary>
    public void SetCompletedLevel(int level)
    {
        studentGameData.data.completedLevel = level;
    }
    public void OnGameCompleted()
    {
        studentGameData.data.attempts++;
        if (studentGameData.data.attempts >= 1)
        {
            studentGameData.data.completedLevel = 0;
        }
        SendData();
    }
    public void SetMaxLevels(int maxLevels)
    {
        studentGameData.data.totalLevel = maxLevels;
    }

    public void GoBackToPlaySchool()
    {
        
    }

    #endregion

    #region  TestData
    public void FetchTestData(Action successCallback)
    {
        if(isTesting)
            return;
#if PLAYSCHOOL_MAIN
    StudentGameProgressApi.Instance.GetStudentByTestsId(PlayerPrefs.GetString(TMKOCPlaySchoolConstants.currentStudentPlaying), studentTestGameData.data.id,
    ()=>
    {
        Debug.Log("Test Data Fected from backed");
        studentTestGameData = StudentGameProgressApi.Instance.CurrentGameTestData;
        successCallback?.Invoke();

    });

#else
    StudentGameProgressApi.Instance.GetStudentByTestsId(TMKOCPlaySchoolConstants.currentStudentName, studentTestGameData.data.id,
             () =>
             {
                 Debug.Log("Test Data Fetched from backend");
                 // StudentGameData.Data tempData = StudentGameProgressApi.Instance.CurrentGameData.data;
                studentTestGameData = StudentGameProgressApi.Instance.CurrentGameTestData;
                 successCallback?.Invoke();
             });

#endif
    }

    public void SendTestData(Action successCallback)
    {   

        if(isTesting)
            return;
#if PLAYSCHOOL_MAIN
    StudentGameProgressApi.Instance.AddStudentByTestsId(PlayerPrefs.GetString(TMKOCPlaySchoolConstants.currentStudentPlaying,studentTestGameData.data.stars,studentTestGameData.data.earnedMedal, studentTestGameData.data.scores,
    studentTestGameData.data.attempts,studentTestGameData.data.totalQuestions, studentTestGameData.data.streak,
    studentTestGameData.data.timeSpentInSeconds,studentTestGameData.data.testId,
    ()=>{
        Debug.Log("Test Data Send successfully");
        successCallback?.Invoke();
    });
#else
    StudentGameProgressApi.Instance.AddStudentByTestsId(TMKOCPlaySchoolConstants.currentStudentName,studentTestGameData.data.stars,studentTestGameData.data.earnedMedal, studentTestGameData.data.scores,
    studentTestGameData.data.attempts,studentTestGameData.data.totalQuestions, studentTestGameData.data.streak,
    studentTestGameData.data.timeSpentInSeconds,studentTestGameData.data.testId,
    ()=>{
        Debug.Log("Test Data Send successfully");
        successCallback?.Invoke();
    });
#endif

    }

    public void SetTestData(int stars = -1, int medal = -1, int score = -1, int attempts = -1,
    int totalQuestions = -1,int streak = -1, int timeSpentInSeconds = -1, int testId = -1)
    {
        if(stars == -1)
            studentTestGameData.data.stars = 0;
        else
            studentTestGameData.data.stars = stars;

        if(medal == -1)
            studentTestGameData.data.earnedMedal = 0;
        else
            studentTestGameData.data.earnedMedal = medal;

        if(score == -1)
            studentTestGameData.data.scores = 0;
        else
            studentTestGameData.data.scores = score;

        if(attempts == -1)
            studentTestGameData.data.attempts = 0;
        else
            studentTestGameData.data.attempts = attempts;

        if(totalQuestions == -1)
            studentTestGameData.data.totalQuestions = 0;
        else
            studentTestGameData.data.totalQuestions = totalQuestions;

         if(streak == -1)
            studentTestGameData.data.streak = 0;
        else
            studentTestGameData.data.streak = streak;

        if(timeSpentInSeconds == -1)
            studentTestGameData.data.timeSpentInSeconds = 0;
        else
            studentTestGameData.data.timeSpentInSeconds = timeSpentInSeconds;

        if(timeSpentInSeconds == -1)
            studentTestGameData.data.testId = 0;
        else
            studentTestGameData.data.testId = testId;
    }

    
    #endregion
}










