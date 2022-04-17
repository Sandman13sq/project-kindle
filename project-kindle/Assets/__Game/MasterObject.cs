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
                GameObject o = GameObject.Find("__game");
                if (o != null)
                {
                    _game = o.GetComponent<GameHeader>();
                }
                else
                {
                    Debug.Log("Missing Game Object in Scene! Add the prefab!");
                    //_game = (new GameObject("__game")).AddComponent<GameHeader>();
                }

                //_game = (new GameObject("__game")).AddComponent<GameHeader>();
                //_game = GameHeader.Instance;
            }
            
            return _game;
        }
    } 
}
