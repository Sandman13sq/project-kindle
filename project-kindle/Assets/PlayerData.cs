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

    public void HealthFlashMeter()
    {
        healthmeter.Flash();
    }

    public int SetEnergy(int value, int valuemax, bool matchprovisional = false)
    {
        energymeter.SetValue(value, matchprovisional);
        energymeter.SetValueMax(valuemax);
        return 0;
    }

    public void EnergyMaximizeProvisional()
    {
        energymeter.MaximizeProvisional();
    }

    public void EnergyFlashMeter()
    {
        energymeter.Flash();
    }
    
    public int SetLevel(int value)
    {
        weaponinfo.SetLevel(value);
        return 0;
    }

    public int SetAmmo(int value, int valuemax)
    {
        weaponinfo.SetAmmo(value);
        weaponinfo.SetAmmoMax(valuemax);
        return 0;
    }
}
