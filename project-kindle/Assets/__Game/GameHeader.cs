using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHeader : MonoBehaviour
{
    // Variables

    private GameObject eventmanager;
    [SerializeField] private GameObject textbox_prefab;
    private TextBox textbox_object;
    private Entity_Move_Manual player_object;
    
    // =====================================================================
    
    // Functions

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        player_object = null;
        textbox_object = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Game Header Start()");
        GetTextbox().AddText("Hello World\nAnd Goodbye Boredom!");
        Debug.Log(textbox_object);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RunEvent(string eventkey)
    {

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
}
