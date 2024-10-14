using DG.Tweening;
using System;
using UnityEngine;

namespace TMKOC.Sorting.FruitSorting2D
{
    public class FruitSelect : Collectible
    {
        public Action<FruitType> OnFruitSelected;
        public Action<FruitType> OnFruitDeselected;

        [SerializeField] private bool m_IsSelected = false;
        [SerializeField] private FruitType m_FruitType;


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
                FruitSelected();
            else
                FruitDeselected();
        }

        private void FruitSelected()
        {
            transform.DOScale(1.25f, 0.25f);
            OnFruitSelected?.Invoke(m_FruitType);
        }

        private void FruitDeselected()
        {
            transform.DOScale(1f, 0.25f);
            OnFruitDeselected?.Invoke(m_FruitType);
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
