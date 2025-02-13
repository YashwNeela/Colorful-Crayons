using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC
{
    public class CutSceneEventManager : MonoBehaviour
    {
        private static CutSceneEventManager _instance;
        public static CutSceneEventManager Instance => _instance ??= new CutSceneEventManager();

        private Dictionary<string, Action> eventDictionary = new Dictionary<string, Action>();

        public void Subscribe(string eventName, Action listener)
        {
            if (!eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] = listener;
            }
            else
            {
                eventDictionary[eventName] += listener;
            }
        }

        public void Unsubscribe(string eventName, Action listener)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName] -= listener;
            }
        }

        public void TriggerEvent(string eventName)
        {
            if (eventDictionary.ContainsKey(eventName))
            {
                eventDictionary[eventName]?.Invoke();
            }
        }
    }
}
