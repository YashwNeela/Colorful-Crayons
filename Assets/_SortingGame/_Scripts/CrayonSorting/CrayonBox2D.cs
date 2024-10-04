using System;
using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
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

    public override void SnapCollectibleToCollector(Collectible collectible, Action PlacedCorrectly)
    {
       foreach (var snapPoint in snapPoints)
            {
                if (!snapPoint.IsOccupied)
                {
                    // collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform; // Change parent first
                    collectible.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
                    collectible.transform.localRotation = Quaternion.identity; // Reset rotation relative to the new parent
                    snapPoint.IsOccupied = true;
                    collectible.SetSnapPoint(snapPoint);

                    if(m_CrayonColor.HasFlag((collectible as Crayon).CrayonColor)){
                        OnItemCollected(snapPoint);
                        PlacedCorrectly?.Invoke();
                    }
                    break;
                }
            }
    }

    public override void OnCollectibleExited(Collectible collectible)
    {
        collectible.RemoveFromSnapPoint();
        if(m_CrayonColor.HasFlag((collectible as Crayon).CrayonColor))
        {
            if(collectedItems > 0)
                OnItemRemoved();
        }
    }

}
