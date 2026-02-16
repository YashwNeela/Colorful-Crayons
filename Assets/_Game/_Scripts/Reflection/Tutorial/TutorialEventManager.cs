using System;
using System.Collections.Generic;

namespace TMKOC{


public class TutorialEventManager
{
    private static TutorialEventManager _instance;
    public static TutorialEventManager Instance => _instance ??= new TutorialEventManager();

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