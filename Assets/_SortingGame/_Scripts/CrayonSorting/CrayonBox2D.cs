using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting.ColorfulCrayons;
using UnityEngine;

public class CrayonBox2D : CrayonBox
{
    [SerializeField] private SpriteRenderer m_BoxSprite, m_CoverSprite;
    protected override void SetBoxColorsBasedOnEnum(CrayonColor crayonColor)
    {
        if (crayonColor.HasFlag(CrayonColor.CrayonRed))
            {
                m_BoxSprite.color = Color.red;
                m_CoverSprite.color = Color.red;
                
            }
            if (crayonColor.HasFlag(CrayonColor.CrayonYellow))
            {
                m_BoxSprite.color = Color.yellow;
                m_CoverSprite.color = Color.yellow;

               
            }
            if (crayonColor.HasFlag(CrayonColor.CrayonGreen))
            {
               m_BoxSprite.color = Color.green;
                m_CoverSprite.color = Color.green;
                
            }
            if (crayonColor.HasFlag(CrayonColor.CrayonBlue))
            {
                m_BoxSprite.color = Color.blue;
                m_CoverSprite.color = Color.blue;
              
            }
    }
}