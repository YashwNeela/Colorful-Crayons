using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC.Sorting.ShapeSorting
{
    public class ShapeSnapPoint : SnapPoint
    {
        [SerializeField] private ShapeType m_ShapeType;
        public ShapeType ShapeType => m_ShapeType;

        public override bool HasValidCollectible()
        {
            ShapeCollectible shapeCollectible = m_CurrentCollectible as ShapeCollectible;
            if(shapeCollectible != null){
            if(m_ShapeType.HasFlag(shapeCollectible.ShapeType))
                return true;
            else
                return false;
            }else
                return false;
        }
    }
}
