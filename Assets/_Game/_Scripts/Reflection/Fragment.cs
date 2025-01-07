using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;
using System;

namespace TMKOC.PlantLifeCycle
{
    public enum FragmentType
    {
        Diamond, Square, Circle, Trianlge, Star
    }
    public class Fragment : MonoBehaviour
    {
        public FragmentType m_FragmentType;

        public Vector3 m_OriginalScale;

        protected bool m_IsCollected;

        public bool IsCollected => m_IsCollected;

        protected bool m_IsScaling;

        Coroutine ref_CO;

        protected virtual void Awake()
        {
            m_OriginalScale = transform.lossyScale;
            m_IsScaling = false;
        }

        protected virtual void OnPlayerTriggerEnter() { }

        protected virtual void OnPlayerExitTrigger() { }

        public virtual void OnSunlightTriggerEnter()
        {

            transform.DOComplete();


            transform.DOScale(1 * 1.1f, 0.5f).OnComplete(() => m_IsScaling = false);


            m_IsCollected = true;


        }

        public virtual void OnSunlightTriggerExit()
        {
            transform.DOComplete();


            m_IsCollected = false;



        }

        IEnumerator Co_Scale(Action callback)
        {
            Debug.Log("Scaling");

            yield return new WaitUntil(() => !m_IsScaling);
            Debug.Log("Scaling callback");
            callback?.Invoke();

        }



    }
}
