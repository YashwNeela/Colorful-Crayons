using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace TMKOC.Sorting.ShapeSorting
{
    [Flags]
    public enum ShapeType
    {
        None = 0,
        Square = 1 << 0,
        Rectangle = 1 << 1,
        Circle = 1 << 2,
        Oval = 1 << 3,

        Triangle = 1 << 4,

    }
    public class ShapeCollector : Collector
    {
        
    }
}
