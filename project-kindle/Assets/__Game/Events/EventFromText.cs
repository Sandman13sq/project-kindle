using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFromText : MasterObject
{
    [System.Serializable]
    private struct EventDef
    {
        public string key;
        [SerializeField][TextArea(4, 32)] public string text;
    }

    [SerializeField] private string calleventkey =  "";
    [SerializeField] private EventDef[] eventdefs;
    [SerializeField][TextArea(10, 32)] private string eventtext = "";

    // Start is called before the first frame update
    void Start()
    {
        // Parse text for events
        foreach (var def in eventdefs)
        {
            EventRunner.ParseEventText("#" + def.key + "\n" + def.text);
        }

        // Run Event
        if (calleventkey != "")
        {
            game.RunEvent(calleventkey);
        }
    }
}
