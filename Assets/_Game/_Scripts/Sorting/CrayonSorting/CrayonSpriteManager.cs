using TMKOC.Sorting;
using UnityEngine;

namespace TMKOC.Sorting.ColorfulCrayons
{
    public class CrayonSpriteManager : Singleton<CrayonSpriteManager>
    {
        [SerializeField] private CrayonSpriteSO m_CrayonSpriteSO;

        public CrayonSpriteSO CrayonSpriteSO => m_CrayonSpriteSO;
    }
}
