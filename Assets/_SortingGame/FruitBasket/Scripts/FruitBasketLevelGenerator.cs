using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting{
    [System.Serializable]
    public class FruitBasketLevelGeneratorData
    {
        public BasketType basketType;
       
    }
public class FruitBasketLevelGenerator : LevelGenerator
{
        public FruitBasketLevelGeneratorData leveData;

        public ParticleSystem starExplosionParticleEffect;
        [Button]
        public virtual void GenerateLevel(int collectorSpawnIndex = 0)
        {
            if(levels == null)
                levels = new List<GameObject>();
            if(collectorSpawnIndex >= collectorSpawnPoints.Count)
            {
                Debug.LogError("Collector spawn Index out of bound");
                return;
            }

            #region  Level gameobject
            GameObject tempLevel = transform.Find("Level" + levels.Count + 1).gameObject;
            GameObject levelGo;
            if(tempLevel == null){
            levelGo = new GameObject("Level" + levels.Count + 1);
            levelGo.transform.parent = transform;
            levelGo.transform.position = Vector3.zero;
            levelGo.AddComponent<Level>();
            levels.Add(levelGo);
            }else
            {
                levelGo = tempLevel;
            }
            #endregion

            #region Collector
            GameObject collector = Instantiate(collectorPrefab);
            collector.transform.parent = levelGo.transform;

            
            collector.transform.position = collectorSpawnPoints[collectorSpawnIndex].position;

            collector.name = "Basket" + leveData.basketType.ToString();
            #endregion

            #region Collectibles
            FruitBasket fruitBasket = collector.GetComponentInChildren<FruitBasket>();
            fruitBasket.m_FruitCollectParticleEffect = starExplosionParticleEffect;
            fruitBasket.SetBasketType(leveData.basketType);

            GameObject fruitParent = new GameObject("Fruits");
            fruitParent.transform.parent = levelGo.transform;
            fruitParent.transform.position= Vector3.zero;

            int numberOfFruitsToSpawn = 0;
            List<Collectible> validFruits = new List<Collectible>();

            for(int i = 0; i< fruitBasket.SnapPointData.Count;i++)
            {
                numberOfFruitsToSpawn += fruitBasket.SnapPointData[i].snapPoints.Count;
            }

            //Get all valid fruits
            for(int i = 0;i< collectibles.Count;i++)
            {
                Fruit f = collectibles[i] as Fruit;
                if(f.BasketType.HasFlag(leveData.basketType))
                {
                    validFruits.Add(collectibles[i]);
                }
            }

            //Spwan random fruits
            for(int i = 0;i<numberOfFruitsToSpawn;i++)
            {
                Fruit fruitToSpawn = validFruits[Random.Range(0,validFruits.Count)] as Fruit;
                GameObject f = Instantiate(fruitToSpawn.gameObject);
                f.transform.parent = fruitParent.transform;

                f.transform.position = collectibleSpawnPoints[Random.Range(0,collectibleSpawnPoints.Count)].transform.position;
            }
            
            #endregion
        }

        
}
}
