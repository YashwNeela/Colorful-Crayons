using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection{
public class LevelRestart : MonoBehaviour
{
    
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<ReflectionTags>(out ReflectionTags tag))
        {
           if(tag.m_Tag == ReflectionTagsEnum.Player){
            ReflectionGameManager.Instance.GameOver();
            ReflectionGameManager.Instance.GameLoose();
           }
        }
    }
}
}
