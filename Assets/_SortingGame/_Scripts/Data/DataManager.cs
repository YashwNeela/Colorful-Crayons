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

    public DataManager(){}

    public DataManager(int GameId, float startGameTime, int maxLevel, bool isTesting)
    {
        studentGameData  = new StudentGameData();
        studentGameData.data = new StudentGameData.Data();

        studentGameData.data.id = GameId;
        this.startGameTime = startGameTime;
        studentGameData.data.totalLevel = maxLevel;

        this.isTesting = false;
        Debug.Log("Max Level is" + studentGameData.data.totalLevel);

    }


    public void FetchData(Action successCallback = null)
    {
        if(isTesting)
            return;
        StudentGameProgressApi.Instance.GetStudentByGameId(TMKOCPlaySchoolConstants.currentStudentName, studentGameData.data.id,
        () =>
        {
            Debug.Log("Data fetched from backend");
           // StudentGameData.Data tempData = StudentGameProgressApi.Instance.CurrentGameData.data;
            previousSessionTime = studentGameData.data.timeSpentInSeconds;
            studentGameData.data.attempts = StudentGameProgressApi.Instance.CurrentGameData.data.attempts;
            studentGameData.data.completedLevel = StudentGameProgressApi.Instance.CurrentGameData.data.completedLevel;
            successCallback?.Invoke();
        });

        startGameTime = Time.time;
    }

    public void SendData(Action successCallback = null)
    {
        if(isTesting)
            return;
        int star = StudentGameProgressApi.Instance.CalculateStars(studentGameData.data.completedLevel, studentGameData.data.totalLevel);
        long currentSesstionTime = StudentGameProgressApi.Instance.EndGame(startGameTime);
        currentSesstionTime += studentGameData.data.timeSpentInSeconds;

       // Debug.Log("Max Level is" + studentGameData.totalLevel);
        
        StudentGameProgressApi.Instance.AddStudentByGameId(TMKOCPlaySchoolConstants.currentStudentName,
                   star, studentGameData.data.completedLevel, studentGameData.data.totalLevel, studentGameData.data.attempts, currentSesstionTime, 10,54,
                    () =>
                   {
                       Debug.Log("Data sent Successfully");
                       successCallback?.Invoke();
                       
                   });
    }
    public void OnLevelCompleted()
    {
        studentGameData.data.completedLevel++;
    }

    /// <summary>
    /// Call when user has finished all the levels
    /// </summary>
    public void OnGameCompleted()
    {
        
        studentGameData.data.attempts++;
        SendData();
    }

    public void SetMaxLevels(int maxLevels)
    {
        studentGameData.data.totalLevel = maxLevels;
    }
}
