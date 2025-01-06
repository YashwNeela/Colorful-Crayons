using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMKOC.PlantLifeCycle;
using UnityEngine;

namespace TMKOC.Reflection
{
    public class FragmentCollector : MonoBehaviour
    {
        protected FragmentType m_FragmentType;

        public void OnSunlightEnter()
        {
            transform.DOScale(transform.localScale * 1.1f, 0.5f);

        }

        public void OnSunlightExit()
        {
            transform.DOScale(transform.localScale / 1.1f, 0.5f);

        }
    }
}
