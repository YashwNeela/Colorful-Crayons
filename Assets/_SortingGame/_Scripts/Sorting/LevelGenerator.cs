using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace TMKOC.Sorting
{

    public class LevelGenerator : MonoBehaviour
    {
        [SerializeField] protected SortingLevelManager levelManager;
        public List<Transform> collectibleSpawnPoints;

        public List<Collectible> collectibles;

        [Tooltip("Prefab will be spawned and will make changes on the fly")]

        public List<Transform> collectorSpawnPoints;
        public GameObject collectorPrefab;

        [SerializeField] protected List<GameObject> levels = new List<GameObject>();

        

        [Button]
        public virtual void ClearLevel(int levelNumber = 1)
        {
            GameObject level = transform.Find("Level0" + levelNumber).gameObject;

           if(level == null)
           {
                Debug.LogError("No level with " + levelManager + "found");
                return;
           }

            levels.Remove(level);
           DestroyImmediate(level);
        }

        [Button]
        public virtual void ShowLevel(int levelNumber)
        {
           
        }


    }
}
