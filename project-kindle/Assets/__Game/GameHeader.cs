using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHeader : MonoBehaviour
{
    // Variables
    [SerializeField] private GameObject textbox_prefab;
    [SerializeField] private AudioManager audiomanager;
    [SerializeField] private PlayerData playerdata;
    private TextBox textbox_object;
    private Entity_Move_Manual player_object;

    private bool lockplayercontrols;

    private GameObject camera_object;
    private EventRunner eventrunner;

    private Dictionary<string, List<EventRunner.CommandDef>> eventmap;

    private int[] gameflags;
    private int[] sceneflags;
    const int FLAGDIV = sizeof(int) * 8;
    
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

        gameflags = new int[32];
        sceneflags = new int[32];

        eventrunner = gameObject.AddComponent(typeof(EventRunner)) as EventRunner;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Flags --------------------------------------------------

    public void GameFlagSet(int flagindex) {gameflags[flagindex / FLAGDIV] |= 1 << (flagindex % FLAGDIV);}
    public void GameFlagToggle(int flagindex) {gameflags[flagindex / FLAGDIV] ^= 1 << (flagindex % FLAGDIV);}
    public void GameFlagClear(int flagindex) {gameflags[flagindex / FLAGDIV] &= ~(1 << (flagindex % FLAGDIV));}
    public bool GameFlagGet(int flagindex) {return (gameflags[flagindex / FLAGDIV] & (1 << (flagindex % FLAGDIV))) != 0;}

    public void SceneFlagSet(int flagindex) {sceneflags[flagindex / FLAGDIV] |= 1 << (flagindex % FLAGDIV);}
    public void SceneFlagToggle(int flagindex) {sceneflags[flagindex / FLAGDIV] ^= 1 << (flagindex % FLAGDIV);}
    public void SceneFlagClear(int flagindex) {sceneflags[flagindex / FLAGDIV] &= ~(1 << (flagindex % FLAGDIV));}
    public bool SceneFlagGet(int flagindex) {return (sceneflags[flagindex / FLAGDIV] & (1 << (flagindex % FLAGDIV))) != 0;}

    // Events --------------------------------------------------
    
    // Executes event with key
    public void EndEvent()
    {
        eventrunner.Clear();
    }
    public void RunEvent(string eventkey)
    {
        if (eventkey == "")
        {
            EndEvent();
        }
        else if ( EventExists(eventkey) )
        {
            Debug.Log(string.Format("Running Event \"{0}\"", eventkey));
            EndEvent();
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
    public bool EventIsRunning() {return eventrunner.IsRunning();}

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

    public PlayerData GetPlayerData() {return playerdata;}
    public AudioManager GetAudioManager() {return audiomanager;}

    // Events --------------------------------------------------

    public Sound PlaySound(string key)
    {
        return audiomanager.Play(key);
    }
}
