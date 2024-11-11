using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.Sorting
{
    public class LevelFailUI : MonoBehaviour
    {
        [SerializeField] private Transform m_LevelFailDataHolderParent;
        [SerializeField] private LevelFailDataHolder m_LevelFailDataHolderPrefab;

        [SerializeField] private GameObject m_DetailContainer;




        public void ToggleDetailContainer(bool value)
        {
            
                m_DetailContainer.GetComponent<LayoutElement>().ignoreLayout = value;
        }
        public void SetLevelFailData(Sprite icon,Color iconColor, bool isCorrect, bool isempty = false)
        {
            

            ToggleDetailContainer(false);
            //Now set the data based on game over condition
                LevelFailDataHolder data = Instantiate(m_LevelFailDataHolderPrefab, m_LevelFailDataHolderParent);
                data.SetData(icon, iconColor,isCorrect, isempty);
        }

        public void ClearChildren()
        {
            //First clear all childrent of level fail data holder parent
            int childCount = m_LevelFailDataHolderParent.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                Destroy(m_LevelFailDataHolderParent.GetChild(i).gameObject);
            }
        }



    }
}
