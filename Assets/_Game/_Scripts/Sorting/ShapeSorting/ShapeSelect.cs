using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace TMKOC.Sorting.ShapeSorting{
public class ShapeSelect : Collectible
{
       public Action<ShapeType> OnShapeSelected;
        public Action<ShapeType> OnShapeDeselected;

        [SerializeField] private bool m_IsSelected = false;
        [SerializeField] private ShapeType m_ShapeType;

        [SerializeField] private GameObject m_HighLightGameobject;


        protected override void Awake()
        {
            base.Awake();
            

        }
        protected override void OnEnable()
        {
            SortingGameManager.OnGameStart += OnGameStart;
            SortingGameManager.OnGameRestart += OnGameRestart;
        }
        protected override void OnDisable()
        {
            SortingGameManager.OnGameStart -= OnGameStart;
            SortingGameManager.OnGameRestart -= OnGameRestart;
        }

        private void OnGameStart()
        {
          m_HighLightGameobject.SetActive(false);

            m_IsSelected = false;

        }

        private void OnGameRestart()
        {
          m_HighLightGameobject.SetActive(false);
            
            m_ObjectReseter.ResetObject();
        }
        private void OnMouseDown()
        {
            m_IsSelected = m_IsSelected ? false : true;

            if (m_IsSelected)
                ShapeSelected();
            else
                ShapeDeselected();
        }

        private void ShapeSelected()
        {
          //  m_SelectedSequence.Play();
          m_HighLightGameobject.SetActive(true);
            transform.DOScale(110f, 0.25f);
            OnShapeSelected?.Invoke(m_ShapeType);
        }

        private void ShapeDeselected()
        {
          m_HighLightGameobject.SetActive(false);

         //   m_SelectedSequence.PlayBackwards();
            transform.DOScale(100f, 0.25f);
            OnShapeDeselected?.Invoke(m_ShapeType);
        }

        protected override void OnTriggerEnter(Collider other)
        {

        }

        protected override void OnTriggerExit(Collider other)
        {

        }

        protected override void OnTriggerStay(Collider other)
        {

        }

}
}