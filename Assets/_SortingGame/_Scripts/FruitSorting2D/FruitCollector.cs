using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public static event Action<List<GridItemData>> OnLevelOver;

        public FruitType FruitType => m_FruitType;

        private List<GridItemData> gridItems = new();

        private List<Tuple<Collectible, bool>> collectibles = new();


        protected override void OnEnable()
        {
            base.OnEnable();
            Gamemanager.OnGameRestart += ClearCollectibles;
            Gamemanager.OnLevelCompleteCheck += ItemsInBasket;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Gamemanager.OnGameRestart -= ClearCollectibles;
            Gamemanager.OnLevelCompleteCheck -= ItemsInBasket;
        }

        private void ClearCollectibles()
        {
            collectibles.Clear();
        }

        private void ItemsInBasket()
        {
            gridItems.Clear();

            for (int i = 0; i < collectibles.Count; i++)
            {
                var item = collectibles[i];
                GridItemData newGridItem = new GridItemData(item.Item1.GetComponent<SpriteRenderer>().sprite,  item.Item2);
                gridItems.Add(newGridItem);
            }

            if(sastaFixCoroutine != null)
            {
                StopCoroutine(sastaFixCoroutine);
            }
            sastaFixCoroutine = StartCoroutine(SastaFix(0.01f));
        }

        private Coroutine sastaFixCoroutine;

        private IEnumerator SastaFix(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            OnLevelOver?.Invoke(gridItems);
        }

        public override void SnapCollectibleToCollector(Collectible collectible, Action PlacedCorrectly)
        {
            foreach (var snapPoint in snapPoints)
            {
                if (!snapPoint.IsOccupied)
                {
                    //// collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform; // Change parent first

                    collectible.transform.DOLocalJump(Vector3.zero, 10f, 1, .35f).OnComplete(() =>
                    {
                        snapPoint.IsOccupied = true;
                        collectible.SetSnapPoint(snapPoint);

                        if (m_FruitType.HasFlag((collectible as Fruit2D).FruitType))
                        {
                            OnItemCollected(snapPoint);
                            PlacedCorrectly?.Invoke();

                            // Add to collectibles
                            collectibles.Add(new(collectible, true));

                            // create a GridItemData
                            //GridItemData newGridItem = new GridItemData(collectible.GetComponent<SpriteRenderer>().sprite, true, collectible);
                            //gridItems.Add(newGridItem);

                            //Debug.Log("fruit name: " + collectible.name);
                        } else
                        {
                            collectibles.Add(new(collectible, false));

                            //GridItemData newGridItem = new GridItemData(collectible.GetComponent<SpriteRenderer>().sprite, true, collectible);
                            //gridItems.Add(newGridItem);
                            //Debug.Log("incorrect fruit name: " + collectible.name);
                        }
                    });


                    //collectible.transform.DOLocalMove(Vector3.zero, 0.75f).OnComplete(() =>
                    //{

                    //    snapPoint.IsOccupied = true;
                    //    collectible.SetSnapPoint(snapPoint);

                    //    if (m_FruitType.HasFlag((collectible as Fruit2D).FruitType))
                    //    {
                    //        OnItemCollected(snapPoint);
                    //        PlacedCorrectly?.Invoke();
                    //    }
                    //});

                    //collectible.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
                    break;
                }
            }
        }

        public override void OnCollectibleExited(Collectible collectible)
        {
            collectible.RemoveFromSnapPoint();
            if (m_FruitType.HasFlag((collectible as Fruit2D).FruitType))
            {
                collectibles.Remove(new(collectible, true));
                //Debug.Log("correct-fruit: " + collectible.name);
                if (collectedItems > 0)
                    OnItemRemoved();
            } else
            {
                collectibles.Remove(new(collectible, false));
                //Debug.Log("incorrect-fruit: " + collectible.name);
            }
        }
    }
}
