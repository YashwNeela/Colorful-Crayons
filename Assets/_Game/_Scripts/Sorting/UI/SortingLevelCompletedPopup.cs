using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TMKOC.Sorting{
public class SortingLevelCompletedPopup : LevelCompletedPopup
{
     protected override void OnGameOver()
    {
        SortingLevel currentLevel = SortingLevelManager.Instance.GetCurrentLevel() as SortingLevel;
        if(currentLevel.Collectors == null){
            m_LevelFailUI.ToggleDetailContainer(true);
            return;
        }
       List<Collector> collectors = currentLevel.Collectors.ToList();
        List<SnapPoint> snapPoints = new List<SnapPoint>();

        for(int i =0;i<collectors.Count;i++)
        {
            if(!collectors[i].ShouldIncludeScore)
                continue;
            for(int j = 0;j<collectors[i].SnapPoints.Length;j++)
            {
                snapPoints.Add(collectors[i].SnapPoints[j]);
            }
        }
        m_LevelFailUI.ClearChildren();
        for(int i =0;i<snapPoints.Count;i++)
        {
            if(snapPoints[i].CurrentCollectible != null)
            m_LevelFailUI.SetLevelFailData(snapPoints[i].CurrentCollectible.spriteRenderer.sprite,
            snapPoints[i].CurrentCollectible.spriteRenderer.color,snapPoints[i].HasValidCollectible());
            else
            m_LevelFailUI.SetLevelFailData(null,Color.white, false,true);

            
        }
    }

        public override void OnNextLevelButtonClicked()
        {
            CloudUI.Instance.PlayColoudEnterAnimation();
        }
    }
}