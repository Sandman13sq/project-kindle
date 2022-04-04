using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Master object to Inherit from.
    Gives access to game header object.
*/
public class MasterObject : MonoBehaviour
{
    // Save me from Object Oriented Hell
    static private GameHeader _game = null;
    public static GameHeader game
    {
        get
        {
            // Create game object if not found 
            if(_game == null)
            {
                //_game = (new GameObject("__game")).AddComponent<GameHeader>();
                _game = GameObject.Find("__game").GetComponent<GameHeader>();
            }
            
            return _game;
        }
    } 
}
