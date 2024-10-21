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
        AirConditioner = 1<<5, Football = 1<<6, Biscut = 1<<7, CheeseSlice = 1<<8, Clock = 1<<9,
        Dice = 1<<10,EarRings = 1<<11, Egg = 1<<12, Hanger = 1<<13, Hexagon = 1<<14, Leaf = 1<<15,
        Nut = 1<<16, OvalMirror = 1<<17, PhotoFrame1 = 1<<18, PhotonFram2 = 1<<19, Pizza = 1<<20,
        PizzaSlice = 1<<21, Radio = 1<<22, RubyBall = 1<<23, TrafficCone = 1<<24, TV = 1<<25, Watermelon = 1<<26,
        Wheel1 = 1<<27, Wheel2 = 1<<28

    }
    public class ShapeCollector : Collector
    {
        
    }
}
