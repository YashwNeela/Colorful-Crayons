using UnityEngine;
using TMKOC.Sorting;
using System;
namespace TMKOC
{
    public class AudioMapper : Singleton<AudioMapper>
    {
        public string GameStart = "GameIntro", GameComplete = "GameComplete";

        public string[] LevelIntro, LevelComplete, LevelFail;

        public string[] TutorialAudio, GemCollection;


        public string GetRandomGemCollection() => GemCollection[UnityEngine.Random.Range(0, GemCollection.Length - 1)];

        public string GetLevelIntro(int level)
        {
            return LevelIntro[level];
        }

        public string GetRandomLevelFail() => LevelFail[UnityEngine.Random.Range(0, LevelFail.Length - 1)];

        public string GetTutorialAudio(int level) => TutorialAudio[level];

        public string GetTutorialAudio(string audioName)
        {
            return Array.Find(TutorialAudio, x => x == audioName);
        }
    }

}