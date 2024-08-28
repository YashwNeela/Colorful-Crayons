using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMKOC.Sorting;

namespace TMKOC.Sorting.ColorfulCrayons{
public class CrayonSnapPoint : SnapPoint
{
    [SerializeField] private CrayonColor m_CrayonColor;

    public CrayonColor CrayonColor => m_CrayonColor;
}
}
