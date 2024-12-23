using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{
    public class Grid_ItemsHandler : MonoBehaviour
    {
        [SerializeField] private Grid_ItemController _gridItemPrefab;

        private List<Grid_ItemController> _currentItems = new();

        private void OnEnable()
        {
            SortingGameManager.OnGameRestart += ResetList;
            FruitCollector.OnLevelOver += LoadCollectedItems;
        }
        private void OnDisable()
        {
            SortingGameManager.OnGameRestart -= ResetList;
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
            for (int i = 0; i < gridItems.Count; i++)
            {
                Grid_ItemController grid_item = Instantiate(_gridItemPrefab, transform);
                grid_item.SetItem(gridItems[i]);
                _currentItems.Add(grid_item);
            }
        }
    }
}
