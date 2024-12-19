using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace TMKOC.Sorting.ToySorting
{
    [CreateAssetMenu(fileName = "TeddyToyTexture", menuName = "ScriptableObject/ToySorting/TeddyToyTexture")]
    public class TeddyToyTextureSO : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<ToyType, Sprite> m_TeddyToyTextureDict;

        public Dictionary<ToyType, Sprite> TeddyToyTextureDict => m_TeddyToyTextureDict;
    }
}
