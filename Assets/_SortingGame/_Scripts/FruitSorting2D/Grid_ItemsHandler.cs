using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{
    public class Grid_ItemsHandler : MonoBehaviour
    {
        [SerializeField] private Grid_ItemController _gridItemPrefab;

        private List<Grid_ItemController> _currentItems = new();

        private void OnEnable()
        {
            Gamemanager.OnGameRestart += ResetList;
            FruitCollector.OnLevelOver += LoadCollectedItems;
        }
        private void OnDisable()
        {
            Gamemanager.OnGameRestart -= ResetList;
            FruitCollector.OnLevelOver -= LoadCollectedItems;
        }

        private void ResetList()
        {
            foreach (Grid_ItemController item in _currentItems)
            {
                Destroy(item.gameObject);
            }

            _currentItems.Clear();
        }

        int index = 0;
        private void LoadCollectedItems(List<GridItemData> gridItems)
        {
            Debug.Log("chk" + index++);
            for (int i = 0; i < gridItems.Count; i++)
            {
                Grid_ItemController grid_item = Instantiate(_gridItemPrefab, transform);
                grid_item.SetItem(gridItems[i]);
                _currentItems.Add(grid_item);
            }
        }
    }
}
