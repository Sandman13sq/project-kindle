using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int healthmax;
    [SerializeField] private int energy;
    [SerializeField] private int energymax;
    
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
    
    public int AddEnergy(int value)
    {
        energymeter.AddValue(value);
        return 0;
    }
}
