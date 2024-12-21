using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting{
public class CloudUI : Singleton<CloudUI>
{
    private Animator m_Animator;

    protected override void Awake()
    {
        base.Awake();
        m_Animator = GetComponent<Animator>();
    }

    //Called from animation event

    public void OnCloudAnimationFinished()
    {
        SortingGameManager.Instance.LoadNextLevel(SortingLevelManager.Instance.CurrentLevelIndex + 1);
    }

    public void PlayColoudEnterAnimation()
    {
        m_Animator.SetTrigger("entry");
    }

    public void PlayColoudExitAnimation()
    {
        m_Animator.SetTrigger("exit");

    }
    
}
}
