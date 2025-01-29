using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Reflection{

    public enum CameraTriggerType
    {
        None,
        NextLevel, PreviousLevel
    }
public class CameraTrigger : MonoBehaviour
{
   public CameraTriggerType m_CameraTriggerType;

    /// <summary>
    /// Sent when another object enters a trigger collider attached to this
    /// object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if(other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            LoadLevel();
        }
    }

    protected virtual void LoadLevel()
    {
        switch(m_CameraTriggerType)
        {
            case CameraTriggerType.None:
            break;
            
            case CameraTriggerType.NextLevel:
         //   LevelManager.Instance.LoadNextLevel();
         GameManager.Instance.GameOver();

         GameManager.Instance.GameWin();
             GameManager.Instance.LoadNextLevel(LevelManager.Instance.CurrentLevelIndex + 1);
            m_CameraTriggerType = CameraTriggerType.PreviousLevel;
            break;

            case CameraTriggerType.PreviousLevel:
            GameManager.Instance.LoadNextLevel(LevelManager.Instance.CurrentLevelIndex -1 );

         //   LevelManager.Instance.LoadPreviousLevel();
            m_CameraTriggerType = CameraTriggerType.NextLevel;
            break;

            default:
            break;
        }
    }
   
}
}