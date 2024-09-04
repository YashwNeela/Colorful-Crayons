using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
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
        public virtual void GenerateLevel(int levelNumber,int collectorSpawnIndex = 0)
        {
            if(levels == null)
                levels = new List<GameObject>();
            if(collectorSpawnIndex >= collectorSpawnPoints.Count)
            {
                Debug.LogError("Collector spawn Index out of bound");
                return;
            }

            #region  Level gameobject
            
             Transform tempLevel = transform.Find("Level0" + levelNumber );
            GameObject levelGo;
            if(tempLevel == null){
            levelGo = new GameObject("Level0" + levelNumber);
            levelGo.transform.parent = transform;
            levelGo.transform.position = Vector3.zero;
            levelGo.AddComponent<Level>();
            levels.Add(levelGo);
            }else
            {
                levelGo = tempLevel.gameObject;
            }
            #endregion

            #region Collector
            GameObject collector = PrefabUtility.InstantiatePrefab(collectorPrefab) as GameObject;
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
                if(CanFruitBePutInBasket(f.BasketType,leveData.basketType))
                {
                    validFruits.Add(collectibles[i]);
                }
            }

            //Spwan random fruits
            for(int i = 0;i<numberOfFruitsToSpawn;i++)
            {
                Fruit fruitToSpawn = validFruits[Random.Range(0,validFruits.Count)] as Fruit;
                GameObject f = PrefabUtility.InstantiatePrefab(fruitToSpawn.gameObject) as GameObject;
                f.transform.parent = fruitParent.transform;

                f.transform.position = collectibleSpawnPoints[Random.Range(0,collectibleSpawnPoints.Count)].transform.position;
            }
            
            #endregion
        }

        private bool CanFruitBePutInBasket(BasketType basketType, BasketType fruitType)
        {
            // Check if the basket type has all the required flags of the fruit type
            return (basketType & fruitType) != 0;
        }

        
}
}
