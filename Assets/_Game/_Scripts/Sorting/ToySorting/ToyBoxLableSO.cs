using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TMKOC.Sorting.ToySorting
{

    [CreateAssetMenu(fileName = "ToyBoxLable", menuName = "ScriptableObject/ToySorting/ToyBoxLable")]
    public class ToyBoxLableSO : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<ToyType, Sprite> m_ToyBoxLableTextures;

        [HideInInspector]
        public Dictionary<ToyType, Sprite> ToyBoxLableTextures => m_ToyBoxLableTextures;
    }
}
