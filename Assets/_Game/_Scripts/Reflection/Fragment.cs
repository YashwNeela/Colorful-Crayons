using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

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

        public virtual void OnSunlightTriggerEnter(){}

        public virtual void OnSunlightTriggerExit(){}


    }
}
