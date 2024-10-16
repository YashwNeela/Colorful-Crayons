using System;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{
    [Flags]
    public enum FruitType
    {
        None,
        Apple = 1 << 0,
        Banana = 1 << 2,
        Cabbage = 1 << 3,
        Cherry = 1 << 4,
        GreenApple = 1 << 5,
        Pear = 1 << 6,
        Pineapple = 1 << 7,
        Mango = 1 << 8,
        Watermelon = 1 << 9,
        Carrot = 1 << 10,
        Eggplant = 1 << 11,
        Onion = 1 << 12,
        Pumpkin = 1 << 13,
        Tomato = 1 << 14,
    }


    public class FruitCollector : Collector
    {
        [SerializeField] private FruitType m_FruitType;

        public FruitType FruitType => m_FruitType;

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

                    if (m_FruitType.HasFlag((collectible as Fruit2D).FruitType))
                    {
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
            if (m_FruitType.HasFlag((collectible as Fruit2D).FruitType))
            {
                if (collectedItems > 0)
                    OnItemRemoved();
            }
        }
    }
}
