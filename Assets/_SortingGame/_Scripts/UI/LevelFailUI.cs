using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace TMKOC.Sorting
{
    public class LevelFailUI : MonoBehaviour
    {
        [SerializeField] private Transform m_LevelFailDataHolderParent;
        [SerializeField] private LevelFailDataHolder m_LevelFailDataHolderPrefab;





        public void SetLevelFailData(int count)
        {
            //First clear all childrent of level fail data holder parent
            int childCount = m_LevelFailDataHolderParent.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(m_LevelFailDataHolderParent.GetChild(i).gameObject);
            }


            //Now set the data based on game over condition
            for (int i = 0; i < count; i++)
            {
                LevelFailDataHolder data = Instantiate(m_LevelFailDataHolderPrefab, m_LevelFailDataHolderParent);
                data.SetData(null, false);
            }
        }




    }
}
