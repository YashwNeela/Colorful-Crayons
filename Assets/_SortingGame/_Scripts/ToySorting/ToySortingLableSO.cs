using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TMKOC.Sorting.ToySorting
{

    [CreateAssetMenu(fileName = "ToySortingLable", menuName = "ScriptableObject/ToySorting/ToySortingLable")]
    public class ToySortingLableSO : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<ToyType, Sprite> m_ToyBoxLableTextures;

        [HideInInspector]
        public Dictionary<ToyType, Sprite> ToyBoxLableTextures => m_ToyBoxLableTextures;
    }
}
