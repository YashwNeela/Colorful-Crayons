using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting.ToySorting;
using UnityEngine;

namespace TMKOC.Sorting.ToySorting
{


    public class TeddyToy : Toy
    {
        [SerializeField] private TeddyToyTextureSO m_TeddyToyTextureSO;

        protected override void Awake()
        {
            base.Awake();
            SetToyTexture();
        }

        public void SetToyTexture()
        {
            if (ToyType.HasFlag(ToyType.Teddy1Red) || ToyType.HasFlag(ToyType.Teddy2Red) || ToyType.HasFlag(ToyType.Teddy3Red))
            {
                m_Renderer.materials[0].mainTexture = m_TeddyToyTextureSO.TeddyToyTextureDict[ToyType.Teddy1Red].texture;
            }
            else if (ToyType.HasFlag(ToyType.Teddy1Blue) || ToyType.HasFlag(ToyType.Teddy2Blue) || ToyType.HasFlag(ToyType.Teddy3Blue))
            {
                m_Renderer.materials[0].mainTexture = m_TeddyToyTextureSO.TeddyToyTextureDict[ToyType.Teddy1Blue].texture;
            }
            else if (ToyType.HasFlag(ToyType.Teddy1Green) || ToyType.HasFlag(ToyType.Teddy2Green) || ToyType.HasFlag(ToyType.Teddy3Green))
            {
                m_Renderer.materials[0].mainTexture = m_TeddyToyTextureSO.TeddyToyTextureDict[ToyType.Teddy1Green].texture;
            }
            else if (ToyType.HasFlag(ToyType.Teddy1Pink) || ToyType.HasFlag(ToyType.Teddy2Pink) || ToyType.HasFlag(ToyType.Teddy3Pink))
            {
                m_Renderer.materials[0].mainTexture = m_TeddyToyTextureSO.TeddyToyTextureDict[ToyType.Teddy1Pink].texture;
            }
            else if (ToyType.HasFlag(ToyType.Teddy1Yellow) || ToyType.HasFlag(ToyType.Teddy2Yellow) || ToyType.HasFlag(ToyType.Teddy3Yellow))
            {
                m_Renderer.materials[0].mainTexture = m_TeddyToyTextureSO.TeddyToyTextureDict[ToyType.Teddy1Yellow].texture;
            }
        }
    }
}
