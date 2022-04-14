using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFromText : MasterObject
{
    [SerializeField][TextArea(10, 32)] private string eventtext = "";

    // Start is called before the first frame update
    void Start()
    {
        if (eventtext != "")
        {
            EventRunner.ParseEventText(eventtext);
        }
    }
}
