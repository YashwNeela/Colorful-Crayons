using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC
{
    public static class StaticCoroutine
    {

        public static IEnumerator Co_GenericCoroutine(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}
