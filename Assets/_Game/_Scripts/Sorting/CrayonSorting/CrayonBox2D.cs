using System;
using System.Collections;
using System.Collections.Generic;
using TMKOC.Sorting;
using TMKOC.Sorting.ColorfulCrayons;
using UnityEngine;

namespace TMKOC.Sorting.ColorfulCrayons
{
    public class CrayonBox2D : CrayonBox
    {
        [SerializeField] private SpriteRenderer[] m_BoxColorSprite;
        protected override void SetBoxColorsBasedOnEnum(CrayonColor crayonColor)
        {
            if (crayonColor.HasFlag(CrayonColor.CrayonRed) || crayonColor.HasFlag(CrayonColor.SketchpenRed))
            {
                for (int i = 0; i < m_BoxColorSprite.Length; i++)
                {
                    m_BoxColorSprite[i].color = Color.red;
                }


            }
            if (crayonColor.HasFlag(CrayonColor.CrayonYellow) || crayonColor.HasFlag(CrayonColor.SketchpenYellow))
            {
                for (int i = 0; i < m_BoxColorSprite.Length; i++)
                {
                    m_BoxColorSprite[i].color = Color.yellow;
                }



            }
            if (crayonColor.HasFlag(CrayonColor.CrayonGreen) || crayonColor.HasFlag(CrayonColor.SketchpenGreen))
            {
                for (int i = 0; i < m_BoxColorSprite.Length; i++)
                {
                    m_BoxColorSprite[i].color = Color.green;
                }


            }
            if (crayonColor.HasFlag(CrayonColor.CrayonBlue) || crayonColor.HasFlag(CrayonColor.SketchpenBlue))
            {
                for (int i = 0; i < m_BoxColorSprite.Length; i++)
                {
                    m_BoxColorSprite[i].color = Color.blue;
                }


            }
        }

        public override void OnCollectibleEntered(Collectible collectible)
        {
            base.OnCollectibleEntered(collectible);
            foreach (var snapPoint in snapPoints)
            {
                if (!snapPoint.IsOccupied)
                {
                   
                    if (m_CrayonColor.HasFlag((collectible as Crayon2D).CrayonColor))
                    {
                            OnItemCollected(snapPoint);

                    }

                    break;
                }
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

                    if (m_CrayonColor.HasFlag((collectible as Crayon2D).CrayonColor))
                    {
                        PlacedCorrectly?.Invoke();
                    }

                    break;
                }
            }
        }

        public override void OnCollectibleExited(Collectible collectible)
        {
            collectible.RemoveFromSnapPoint();
            if (m_CrayonColor.HasFlag((collectible as Crayon).CrayonColor))
            {
                if (collectedItems > 0)
                    OnItemRemoved();
            }
        }

    }
}
