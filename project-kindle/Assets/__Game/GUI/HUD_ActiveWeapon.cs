using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_ActiveWeapon : MasterObject
{
    [SerializeField] private int level; 
    [SerializeField] private int ammo; 
    [SerializeField] private int ammomax; 
    
    [SerializeField] private GameObject levelstringobj; 
    [SerializeField] private GameObject numeratorobj; 
    [SerializeField] private GameObject denominatorobj;
    private UnityEngine.UI.Text levelstringtextcomponent;
    private UnityEngine.UI.Text numeratortextcomponent;
    private UnityEngine.UI.Text denominatortextcomponent;
    [SerializeField] private UnityEngine.UI.Image weaponiconrdr;
    [SerializeField] private Sprite[] weaponiconsprites;

    private Image[] visualcomponents_images;
    private Text[] visualcomponents_text;

    private bool guiactive;

    [SerializeField] Color textcolor_value, textcolor_empty;

    // Start is called before the first frame update
    void Start()
    {
        levelstringtextcomponent = levelstringobj.GetComponent<UnityEngine.UI.Text>();
        numeratortextcomponent = numeratorobj.GetComponent<UnityEngine.UI.Text>();
        denominatortextcomponent = denominatorobj.GetComponent<UnityEngine.UI.Text>();

        // Find components to hide when GUI is disabled
        visualcomponents_text = GetComponentsInChildren<Text>();
        visualcomponents_images = GetComponentsInChildren<Image>();
        guiactive = true;

        SetLevel(1);
        SetAmmo(100);
        SetAmmoMax(100);
    }

    // Update is called once per frame
    void Update()
    {
        if (guiactive != game.GameFlagGet(_GameHeader.GameFlag.show_gui))
        {
            guiactive = game.GameFlagGet(_GameHeader.GameFlag.show_gui);
            foreach (var vis in visualcomponents_images) {vis.enabled = guiactive;}
            foreach (var vis in visualcomponents_text) {vis.enabled = guiactive;}
        }
    }

    // Method ===============================================

    public void SetLevel(int _value)
    {
        level = _value;
        levelstringtextcomponent.text = "Lv " + level.ToString();
    }

    public void SetAmmo(int _value)
    {
        ammo = _value;

        // Infinite Ammo
        if (ammomax <= 0)
        {
            numeratortextcomponent.text = "-- /";
        }
        // Ammo amount
        else
        {
            numeratortextcomponent.text = ammo.ToString() + " /";
        }

        // Text color
        if (ammomax > 0 && ammo == 0) // No ammo, color red
        {
            numeratortextcomponent.color = textcolor_empty;
            denominatortextcomponent.color = textcolor_empty;
        }
        else // Has ammo, color white
        {
            numeratortextcomponent.color = textcolor_value;
            denominatortextcomponent.color = textcolor_value;
        }
    }

    public void SetAmmoMax(int _value)
    {
        ammomax = _value;

        // Infinite ammo
        if (ammomax <= 0)
        {
            numeratortextcomponent.text = "-- /";
            denominatortextcomponent.text = "--";
        }
        // Nonzero Ammo
        else
        {
            denominatortextcomponent.text = ammomax.ToString();
        }
    }

    public void SetWeapon(int weaponindex)
    {
        weaponiconrdr.sprite = weaponiconsprites[weaponindex];
    }
}
