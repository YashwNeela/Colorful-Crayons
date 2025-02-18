using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection
{
    public enum TriggerType
    {
        None, LevelIntro, GameEnd
    }
public class Trigger : MonoBehaviour
{
    
    public TriggerType triggerType;

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<ReflectionTags>().m_Tag == ReflectionTagsEnum.Player)
        {
            switch(triggerType)
            {
                case TriggerType.None:
                break;

                case TriggerType.LevelIntro:
                if(TutorialManager.Instance.IsTutorialCompleted(TutorialIds.mirrorTutorial))
                    (LevelManager.Instance.GetCurrentLevel() as ReflectionLevel).TriggerLevelIntro();
                break;

                case TriggerType.GameEnd:
                GameManager.Instance.GameOver();
                GameManager.Instance.GameWin();
                GameManager.Instance.StartOutroCutScene();
              //  GameManager.Instance.GameCompleted();
                
                break;
            }
        }
    }
}
}
