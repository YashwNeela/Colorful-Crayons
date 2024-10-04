using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting.ColorfulCrayons;
using UnityEngine;

public class Crayon2D : Crayon
{
    [SerializeField] private SpriteRenderer m_CrayonColorSprite;

    protected override void SetCrayonColor(CrayonColor crayonColor)
    {
        if (crayonColor.HasFlag(CrayonColor.CrayonRed))
        {
            m_CrayonColorSprite.color = Color.red;

        }
        if (crayonColor.HasFlag(CrayonColor.CrayonYellow))
        {
            m_CrayonColorSprite.color = Color.yellow;


        }
        if (crayonColor.HasFlag(CrayonColor.CrayonGreen))
        {
            m_CrayonColorSprite.color = Color.green;

        }
        if (crayonColor.HasFlag(CrayonColor.CrayonBlue))
        {
            m_CrayonColorSprite.color = Color.blue;

        }

    }

    protected virtual void OnTriggerEnter2D(Collider2D other) 
    {
        HandleCollectorOnTriggerEnter(other);   
    }

    /// <summary>
    /// Sent each frame where another object is within a trigger collider
    /// attached to this object (2D physics only).
    /// </summary>
    /// <param name="other">The other Collider2D involved in this collision.</param>
    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        HandleCollectorOnTriggerStay(other);
    }

    protected virtual void OnTriggerExit2D(Collider2D other) 
    {
        HandleCollectorOnTriggerExit(other);
    }
}
