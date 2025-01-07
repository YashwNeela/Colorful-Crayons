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

        // A generic coroutine that waits until the given condition is false, then fires the action
        public static IEnumerator Co_WaitUntil(Func<bool> condition, Action onComplete)
        {
            yield return new WaitUntil(condition);
            // Wait until the condition is no longer true
            

            // Fire the action when the condition is no longer true
            onComplete?.Invoke();
        }

       // public static Ienu

    }
}
