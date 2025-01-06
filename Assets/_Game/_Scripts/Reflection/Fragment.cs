using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using DG.Tweening;

namespace TMKOC.PlantLifeCycle
{
    public enum FragmentType
    {
        Diamond, Square, Circle, Trianlge,Star
    }
    public class Fragment : MonoBehaviour
    {
        public FragmentType m_FragmentType;

        protected virtual void OnPlayerTriggerEnter(){}

        protected virtual void OnPlayerExitTrigger(){}

        public virtual void OnSunlightTriggerEnter()
        {
            transform.DOScale(transform.localScale * 1.1f, 0.5f);
        }

        public virtual void OnSunlightTriggerExit()
        {
            transform.DOScale(transform.localScale / 1.1f, 0.5f);

        }


    }
}
