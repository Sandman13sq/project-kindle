using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFromText : MasterObject
{
    [SerializeField] string calleventkey =  "";
    [SerializeField][TextArea(10, 32)] private string eventtext = "";

    // Start is called before the first frame update
    void Start()
    {
        // Parse text for events
        if (eventtext != "")
        {
            EventRunner.ParseEventText(eventtext);
        }

        // Run Event
        if (calleventkey != "")
        {
            game.RunEvent(calleventkey);
        }
    }
}
