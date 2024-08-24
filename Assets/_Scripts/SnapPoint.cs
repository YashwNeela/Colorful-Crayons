using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Colorful_Crayons
{
    public class SnapPoint : MonoBehaviour
    {
        public bool IsOccupied { get; set; } = false;

        void OnEnable()
        {
            Gamemanager.OnGameStart += OnGameStart;
        }

        private void OnGameStart()
        {
            IsOccupied = false;
        }

        void OnDisable()
        {
            Gamemanager.OnGameStart -= OnGameStart;

        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
        }

    }
}
