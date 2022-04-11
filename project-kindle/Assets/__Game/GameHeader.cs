using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHeader : MonoBehaviour
{
    // Variables
    [SerializeField] private EventRunner eventrunner;
    [SerializeField] private GameObject textbox_prefab;
    private TextBox textbox_object;
    private Entity_Move_Manual player_object;

    private bool lockplayercontrols;

    private GameObject camera_object;

    private Dictionary<string, List<EventRunner.CommandDef>> eventmap;
    
    // =====================================================================
    
    // Functions

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        //player_object = null;
        //textbox_object = null;
        //camera_object = null;
        
        if (camera_object == null)
        {
            camera_object = GameObject.Find("__camera");
            DontDestroyOnLoad(camera_object);
        }

        lockplayercontrols = false;

        eventmap = new Dictionary<string, List<EventRunner.CommandDef>>();

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Events --------------------------------------------------
    
    // Executes event with key
    public void RunEvent(string eventkey)
    {
        if ( EventExists(eventkey) )
        {
            eventrunner.SetEventCommands(eventmap[eventkey].ToArray());
            eventrunner.ContinueEvent();
        }
        else
        {
            Debug.Log(string.Format("Event \"{0}\" not found", eventkey));
        }
    }
    
    // Returns true if event with given key exists
    public bool EventExists(string key) {return eventmap.ContainsKey(key);}

    // Executes next event commmand
    public bool ContinueEvent()
    {
        if (eventrunner != null)
        {
            eventrunner.ContinueEvent();
            return true;
        }
        return false;
    }

    // Creates and returns new event command list
    public List<EventRunner.CommandDef> DefineEvent(string event_key)
    {
        var evcommands = new List<EventRunner.CommandDef>();
        eventmap[event_key] = evcommands;

        Debug.Log(string.Format("New Event \"{0}\"", event_key));
        return evcommands;
    }

    // Get/Set ------------------------------------------------
    public void SetPlayer(Entity_Move_Manual plyr)
    {
        player_object = plyr;
    }

    public Entity_Move_Manual GetPlayer()
    {
        if (player_object == null)
            {player_object = (new GameObject("player")).AddComponent<Entity_Move_Manual>();}
        return player_object;
    }

    public TextBox GetTextbox()
    {
        if (textbox_object == null)
            {textbox_object = Instantiate(textbox_prefab).GetComponent<TextBox>();}
        return textbox_object;
    }

    public EventRunner GetEventRunner()
    {
        if (eventrunner == null)
            {eventrunner = (new GameObject("__eventrunner")).GetComponent<EventRunner>();}
        return eventrunner;
    }

    public void SetContolsLocked(bool locked) {lockplayercontrols = locked;}
    public bool GetContolsLocked() {return lockplayercontrols;}

    public Vector3 GetCameraPosition() {return camera_object.transform.position;}
}
