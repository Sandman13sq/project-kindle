using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Kindle_Weapon_Sprites : MonoBehaviour
{
    [System.Serializable]
    public struct SpriteDef
    {
        [SerializeField] public string sourcespritename;
        [SerializeField] public Sprite[] sprites;
    }

    [SerializeField] private int colorindex;

    [SerializeField] private SpriteRenderer spriterenderer;
    [SerializeField] private SpriteRenderer kindlespriterenderer;
    [SerializeField] private SpriteDef[] spritemap;
    private Dictionary<string, Sprite> spritedict;
    
    public void Start()
    {
        spritedict = new Dictionary<string, Sprite>();
        foreach (SpriteDef def in spritemap)
        {
            for (var i = 0; i < def.sprites.Length; i++)
            {
                spritedict[def.sourcespritename + "_" + i.ToString()] = def.sprites[i];
            }
        }
    }

    public void LateUpdate()
    {
        string spritename = kindlespriterenderer.sprite.name;
        
        // Set sprite if key exists
        if (spritedict.ContainsKey(spritename))
        {
            spriterenderer.enabled = true;
            spriterenderer.flipX = kindlespriterenderer.flipX;
            spriterenderer.sprite = spritedict[spritename];
        }
        // Hide sprite
        else
        {
            //Debug.Log(String.Format("No key for \"{}\"", spritename));
            spriterenderer.enabled = false;
        }
    }
}
