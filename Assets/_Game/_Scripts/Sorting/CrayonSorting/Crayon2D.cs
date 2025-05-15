using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using TMKOC.Sorting.ColorfulCrayons;
using UnityEngine;
using TMKOC;
public class Crayon2D : Crayon
{

    [SerializeField] private SpriteRenderer m_CrayonColorSprite;

    protected override void SetCrayonColor(CrayonColor crayonColor)
    {
        if (crayonColor.HasFlag(CrayonColor.CrayonRed) || crayonColor.HasFlag(CrayonColor.SketchpenRed))
        {
            m_CrayonColorSprite.color = Color.red;

        }
        if (crayonColor.HasFlag(CrayonColor.CrayonYellow) || crayonColor.HasFlag(CrayonColor.SketchpenYellow))
        {
            m_CrayonColorSprite.color = Color.yellow;


        }
        if (crayonColor.HasFlag(CrayonColor.CrayonGreen) || crayonColor.HasFlag(CrayonColor.SketchpenGreen))
        {
            m_CrayonColorSprite.color = Color.green;

        }
        if (crayonColor.HasFlag(CrayonColor.CrayonBlue) || crayonColor.HasFlag(CrayonColor.SketchpenBlue))
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

    protected override void HandleCollectorOnTriggerExit(Component collider)
    {
        base.BaseHandleCollectorOnTriggerExit(collider);


        m_ValidCollector = null;
        m_CurrentCollector = null;
    }

    protected override void OnPlacedCorrectly()
    {
        BaseOnPlacedCorrectly();
    }

    protected override void PlaceInCorrectly(Collector collector)
    {
        BaseOnPlaceInCorrectly(collector);
    }

    protected override void HandleDragEnd()
    {
        if (m_CurrentCollector != null && !m_IsPlacedCorrectly)
                m_CurrentCollector.OnCollectibleEntered(this);
                
        if (m_IsPlacedInsideCollector)
        {
            draggable.ResetToStartDraggingValues();
            return;
        }
        if (m_CurrentCollector != null)
        {
            if (m_ValidCollector != null)
            {

                if (m_ValidCollector.IsSlotAvailable())
                {
                    if ((m_CurrentCollector as CrayonBox2D).CrayonColor.HasFlag(m_CrayonColor))
                        m_CurrentCollector.SnapCollectibleToCollector(this, () => OnPlacedCorrectly());
                    else
                    {
                        m_CurrentCollector.SnapCollectibleToCollector(this, () => { });
                        PlaceInCorrectly(m_CurrentCollector);
                    }
                }
                else
                {
                    draggable.ResetToStartDraggingValues();

                    m_ValidCollector = null;
                    m_CurrentCollector = null;
                }

            }


        }
      
    }

    protected override void OnDragStartedStaticAction(Transform transform)
    {
        if (transform == this.transform)
            return;

        if (m_Collider is Collider)
            ((Collider)m_Collider).enabled = false;
        if (m_Collider is Collider2D)
            ((Collider2D)m_Collider).enabled = false;
    }

    protected override void OnDragEndStaticAction(Transform transform)
    {
        if (transform == this.transform)
            return;

        if (m_Collider is Collider)
            ((Collider)m_Collider).enabled = true;
        if (m_Collider is Collider2D)
            ((Collider2D)m_Collider).enabled = true;
    }

    protected override void OnGameStart()
    {
        if (m_HasCustomSnapPoint)
        {
            m_IsPlacedCorrectly = m_CustomIsPlacedCorrectly;
            m_IsPlacedInsideCollector = true;

            m_CurrentSnapPoint = m_CustomSnapPoint;
            transform.position = m_CurrentSnapPoint.transform.position;
            transform.rotation = m_CurrentSnapPoint.transform.rotation;
        }
        else
        {
            m_IsPlacedCorrectly = false;
            m_IsPlacedInsideCollector = false;
        }
    }

    protected override void Reset()
    {
        m_IsPlacedCorrectly = false;

        if(SortingGameManager.Instance.CurrentGameState != GameState.Restart)
            draggable.m_CanDrag = true;
        else
            draggable.m_CanDrag = false;

    }
}
