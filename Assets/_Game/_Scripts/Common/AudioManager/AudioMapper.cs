using UnityEngine;
using TMKOC.Sorting;
namespace TMKOC
{
    public class AudioMapper : Singleton<AudioMapper>
    {
        public string GameStart = "GameIntro", GameComplete = "GameComplete";

        public string[] LevelIntro, LevelComplete, LevelFail;

    }

}