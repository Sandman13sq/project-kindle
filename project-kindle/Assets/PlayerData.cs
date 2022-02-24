using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private HUDMeter healthmeter;
    [SerializeField] private HUDMeter energymeter;
    [SerializeField] private HUD_ActiveWeapon weaponinfo;

    // Common ==================================================

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Methods ==================================================

    public int SetHealth(int value)
    {
        healthmeter.SetValue(value);
        return 0;
    }

    public int AddHealth(int value)
    {
        healthmeter.AddValue(value);
        return 0;
    }

    public int SetEnergy(int value, int valuemax, bool matchprovisional = false)
    {
        energymeter.SetValue(value, matchprovisional);
        energymeter.SetValueMax(valuemax);
        return 0;
    }
    
    public int AddEnergy(int value)
    {
        energymeter.AddValue(value);
        return 0;
    }

    public int SetLevel(int value)
    {
        weaponinfo.SetLevel(value);
        return 0;
    }
}
