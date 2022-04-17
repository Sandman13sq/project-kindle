using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;
using UnityEngine.SceneManagement;

public class GameHeader : MonoBehaviour
{
    public enum GameFlag : int
    {
        zero,
        lock_player,
        show_gui,
        show_player,
    }

    // Variables
    [SerializeField] private GameObject player_prefab;
    [SerializeField] private GameObject textbox_prefab;
    [SerializeField] private AudioManager audiomanager;
    [SerializeField] private PlayerData playerdata;
    [SerializeField] private GameObject playerhud;
    private TextBox textbox_object;
    private Entity_Move_Manual player_object;

    private GameObject camera_object;
    private EventRunner eventrunner;

    private Dictionary<string, List<EventRunner.CommandDef>> eventmap;

    private int[] gameflags;
    private int[] sceneflags;
    const int FLAGDIV = sizeof(int) * 8;

    // =====================================================================
    
    // Functions

    public static GameHeader Instance { get; private set; }

    void Awake()
    {
        // Enforce singleton behavior
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this) 
        {
            Destroy(this);
            return;
        } 
        else 
        { 
            Instance = this; 
        }

        DontDestroyOnLoad(this.gameObject);

        // Find camera
        if (camera_object == null)
        {
            camera_object = GameObject.Find("__camera");
            DontDestroyOnLoad(camera_object);
        }

        // Player
        player_object = Instantiate(player_prefab).GetComponent<Entity_Move_Manual>();
        DontDestroyOnLoad(player_object.gameObject);

        eventmap = new Dictionary<string, List<EventRunner.CommandDef>>();

        // Set up game flags
        gameflags = new int[32];
        sceneflags = new int[32];
        
        GameFlagSet(GameFlag.show_player);
        GameFlagSet(GameFlag.show_gui);
        GameFlagSet(GameFlag.lock_player);

        // Create event runner
        eventrunner = gameObject.AddComponent(typeof(EventRunner)) as EventRunner;
        
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(string.Format("New Scene: \"{0}\"", scene.name));

        if (player_object != null)
        {
            SetCameraFocus(player_object.gameObject);
        }

        // Reset Scene Flags
        sceneflags = new int[32];
    }

    // Camera -------------------------------------------------

    public void SetCameraFocus(GameObject o)
    {
        CinemachineBrain cmachine = camera_object.GetComponent<CinemachineBrain>();
        
        foreach (var blend in cmachine.m_CustomBlends.m_CustomBlends)
        {
            GameObject.Find(blend.m_To).GetComponent<CinemachineVirtualCamera>().m_Follow = o.transform;
        }
    }

    // Flags --------------------------------------------------

    public void GameFlagSet(GameFlag flagindex) {GameFlagSet((int)flagindex);}
    public void GameFlagToggle(GameFlag flagindex) {GameFlagToggle((int)flagindex);}
    public void GameFlagClear(GameFlag flagindex) {GameFlagClear((int)flagindex);}
    public bool GameFlagGet(GameFlag flagindex) {return GameFlagGet((int)flagindex);}

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

        //Debug.Log(string.Format("New Event \"{0}\"", event_key));
        return evcommands;
    }

    // Get/Set ------------------------------------------------
    public void SetPlayer(Entity_Move_Manual plyr)
    {
        player_object = plyr;
    }

    public Entity_Move_Manual GetPlayer()
    {
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

    public Vector3 GetCameraPosition() {return camera_object.transform.position;}

    public PlayerData GetPlayerData() {return playerdata;}
    public AudioManager GetAudioManager() {return audiomanager;}

    // Events --------------------------------------------------

    public Sound PlaySound(string key)
    {
        return audiomanager.Play(key);
    }
}
