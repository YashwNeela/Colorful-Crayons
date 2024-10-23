using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.ShapeSorting
{
    public class ShapeSnapPoint : SnapPoint
    {
        [SerializeField] private ShapeType m_ShapeType;

        public ShapeType ShapeType => m_ShapeType;
    }
}
