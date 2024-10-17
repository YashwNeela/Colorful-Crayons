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


        protected override void OnEnable()
        {
            Gamemanager.OnGameStart += OnGameStart;
            Gamemanager.OnGameRestart += OnGameRestart;
        }
        protected override void OnDisable()
        {
            Gamemanager.OnGameStart -= OnGameStart;
            Gamemanager.OnGameRestart -= OnGameRestart;
        }

        private void OnGameStart()
        {

            m_IsSelected = false;

        }

        private void OnGameRestart()
        {

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
            transform.DOScale(75f, 0.25f);
            OnShapeSelected?.Invoke(m_ShapeType);
        }

        private void ShapeDeselected()
        {
            transform.DOScale(63f, 0.25f);
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