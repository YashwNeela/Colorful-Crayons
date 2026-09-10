using UnityEngine;

namespace TMKOC.StarLink
{

public class StarlinkAudioMapper : AudioMapper
{
        public string[] TapAudio,SuccesfullHit,Miss;

        public string GetRandomTapAudio()
        {
                return TapAudio[Random.Range(0,TapAudio.Length)];                
        }
        
        public string GetRandomSuccesfulHit()
        {
                return SuccesfullHit[Random.Range(0,SuccesfullHit.Length)];
        }        

        public string GetRandomMiss()
                {
                return Miss[Random.Range(0,Miss.Length)];
                        
                }
}
}