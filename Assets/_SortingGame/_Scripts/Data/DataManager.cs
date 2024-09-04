using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DataManager
{
    StudentGameData studentGameData;
    public StudentGameData StudentGameData => studentGameData;
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
}










