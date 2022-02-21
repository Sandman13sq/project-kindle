using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private HUDMeter healthmeter;
    [SerializeField] private HUDMeter energymeter;

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

    public int SetEnergy(int value)
    {
        energymeter.SetValue(value);
        return 0;
    }
    
    public int AddEnergy(int value)
    {
        energymeter.AddValue(value);
        return 0;
    }
}
