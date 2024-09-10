using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

public class PlayschoolTestDataManager
{
    private int m_MaxTestQuestions;
    private bool isTesting;

    public int MaxTestQuestions => m_MaxTestQuestions;

    StudentTestGameData studentTestGameData;
    public StudentTestGameData StudentTestData => studentTestGameData;

    private int GameId;

    public PlayschoolTestDataManager()
    {

    }

    public PlayschoolTestDataManager(int GameId, int maxTextQuestions, bool isTesting)
    {
        this.isTesting = isTesting;
        m_MaxTestQuestions = maxTextQuestions;
        this.GameId = GameId;

    }

    ~PlayschoolTestDataManager()
    {

    }

    #region Stars

    public int GetStarsBasedOnAttempt(int attemptNumber, int questionRight)
    {
        float percentage = ((float)questionRight / m_MaxTestQuestions) * 100;

        switch (attemptNumber)
        {
            case 1:
                if (percentage >= 100)
                    return 5; // 100% correct answers
                else if (percentage >= 80)
                    return 4; // 80% to 99% correct answers
                else if (percentage >= 70)
                    return 3; // 70% to 79% correct answers
                else if (percentage >= 60)
                    return 0; // 60% to 69% correct answers
                else
                    return 0; // Less than 60% correct answers

            case 2:
                if (percentage >= 100)
                    return 4; // 100% correct answers
                else if (percentage >= 80)
                    return 3; // 80% to 99% correct answers
                else if (percentage >= 70)
                    return 2; // 70% to 79% correct answers
                else if (percentage >= 60)
                    return 0; // 60% to 69% correct answers
                else
                    return 0; // Less than 60% correct answers
            case 3:
                if (percentage >= 100)
                    return 3; // 100% correct answers
                else if (percentage >= 80)
                    return 2; // 80% to 99% correct answers
                else if (percentage >= 70)
                    return 1; // 70% to 79% correct answers
                else if (percentage >= 60)
                    return 0; // 60% to 69% correct answers
                else
                    return 0; // Less than 60% correct answers
        }

        return -1;
    }

    public int GetMedalsBasedOnAttempt(int attemptNumber, int questionRight)
    {
        float percentage = ((float)questionRight / m_MaxTestQuestions) * 100;

        switch (attemptNumber)
        {
            case 1:
                if (percentage >= 100)
                    return 1; // 100% correct answers
                else if (percentage >= 80)
                    return 2; // 80% to 99% correct answers
                else if (percentage >= 70)
                    return 3; // 70% to 79% correct answers
                else if (percentage >= 60)
                    return -1; // 60% to 69% correct answers
                else
                    return -1; // Less than 60% correct answers

            case 2:
                if (percentage >= 100)
                    return 2; // 100% correct answers
                else if (percentage >= 80)
                    return 3; // 80% to 99% correct answers
                else if (percentage >= 70)
                    return -1; // 70% to 79% correct answers
                else if (percentage >= 60)
                    return -1; // 60% to 69% correct answers
                else
                    return -1; // Less than 60% correct answers
            case 3:
                if (percentage >= 100)
                    return 3; // 100% correct answers
                else if (percentage >= 80)
                    return -1; // 80% to 99% correct answers
                else if (percentage >= 70)
                    return -1; // 70% to 79% correct answers
                else if (percentage >= 60)
                    return -1; // 60% to 69% correct answers
                else
                    return -1; // Less than 60% correct answers
        }

        return -1;
    }

    public float GetScore(int attemptNumber, int questionRight)
    {
        return ((float)questionRight / m_MaxTestQuestions) * 100;
    }

    #endregion

    #region  TestData
    public void FetchTestData(Action successCallback)
    {
        if (isTesting)
            return;
        string studentName = "";

#if PLAYSCHOOL_MAIN
        studentName = PlayerPrefs.GetString(TMKOCPlaySchoolConstants.currentStudentPlaying);
#else
        studentName = TMKOCPlaySchoolConstants.currentStudentName;

#endif
        StudentGameProgressApi.Instance.GetStudentByTestsId(studentName, GameId,
                 () =>
                 {
                     Debug.Log("Test Data Fetched from backend");
                     // StudentGameData.Data tempData = StudentGameProgressApi.Instance.CurrentGameData.data;
                     studentTestGameData = StudentGameProgressApi.Instance.CurrentGameTestData;
                     successCallback?.Invoke();
                 });


    }



    public void SendTestData(Action successCallback)
    {
        if (isTesting)
            return;
        string studentName = "";
#if PLAYSCHOOL_MAIN
        studentName = PlayerPrefs.GetString(TMKOCPlaySchoolConstants.currentStudentPlaying);
#else
        studentName = TMKOCPlaySchoolConstants.currentStudentName;

#endif
        StudentGameProgressApi.Instance.AddStudentByTestsId(studentName, studentTestGameData.data.stars, studentTestGameData.data.earnedMedal, studentTestGameData.data.scores,
        studentTestGameData.data.attempts, studentTestGameData.data.totalQuestions, studentTestGameData.data.streak,
        studentTestGameData.data.timeSpentInSeconds, studentTestGameData.data.testId,
        () =>
        {
            Debug.Log("Test Data Send successfully");
            successCallback?.Invoke();
        });
    }



    #endregion
}


