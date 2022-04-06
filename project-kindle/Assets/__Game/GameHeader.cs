using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHeader : MonoBehaviour
{
    // Variables

    private EventRunner eventrunner;
    [SerializeField] private GameObject textbox_prefab;
    private TextBox textbox_object;
    private Entity_Move_Manual player_object;

    private bool lockplayercontrols;

    private GameObject camera_object;
    
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
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunEvent(string eventkey)
    {

    }

    public void RunEvent(EventRunner e)
    {
        eventrunner = e;
        e.ContinueEvent();
    }

    public bool ContinueEvent()
    {
        if (eventrunner != null)
        {
            eventrunner.ContinueEvent();
            return true;
        }
        return false;
    }

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
