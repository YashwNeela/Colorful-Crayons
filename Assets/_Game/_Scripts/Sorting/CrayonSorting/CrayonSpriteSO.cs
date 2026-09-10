using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TMKOC.Sorting.ColorfulCrayons
{
    [CreateAssetMenu(fileName = "CrayonSprite", menuName = "ScriptableObject/CrayonSorting/CrayonSprite")]
    public class CrayonSpriteSO : SerializedScriptableObject
    {
        [SerializeField]
        private Dictionary<CrayonColor, Sprite> m_CrayonSprites;

        [HideInInspector]
        public Dictionary<CrayonColor, Sprite> CrayonSprites => m_CrayonSprites;
    }
}
