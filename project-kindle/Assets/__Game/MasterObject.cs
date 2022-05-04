using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Master object to Inherit from.
    Gives access to game header object.
*/
public class MasterObject : MonoBehaviour
{
    // Layer constants
    protected const int LAYER_WORLD_INDEX = 3;
    protected const int LAYER_WORLD_BIT = 1 << LAYER_WORLD_INDEX;
    protected const int LAYER_ENTITY_INDEX = 6; 
    protected const int LAYER_ENTITY_BIT = 1 << LAYER_ENTITY_INDEX;
    protected const int LAYER_HITBOX_INDEX = 7; 
    protected const int LAYER_HITBOX_BIT = 1 << LAYER_HITBOX_INDEX;
    protected const int LAYER_HURTBOX_INDEX = 8; 
    protected const int LAYER_HURTBOX_BIT = 1 << LAYER_HURTBOX_INDEX;


    // Save me from Object Oriented Hell
    static private _GameHeader _game = null;
    public static _GameHeader game
    {
        get
        {
            // Create game object if not found 
            if(_game == null)
            {
                GameObject o = GameObject.Find("__game");
                if (o != null)
                {
                    _game = o.GetComponent<_GameHeader>();
                }
                else
                {
                    Debug.Log("Missing Game Object in Scene! Add the prefab!");
                    //_game = (new GameObject("__game")).AddComponent<_GameHeader>();
                }

                //_game = (new GameObject("__game")).AddComponent<_GameHeader>();
                //_game = _GameHeader.Instance;
            }
            
            return _game;
        }
    }

    public static float ApproachTS(float x, float target, float step = 1.0f)
    {
        return DmrMath.Approach(x, target, step*game.TimeStep);
    }
}
