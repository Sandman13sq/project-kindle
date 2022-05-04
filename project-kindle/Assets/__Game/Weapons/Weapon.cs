using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Weapons inherit from this class.
    This class should NOT be used on it's own
*/
public class Weapon : MasterObject
{
    // Level Stats
    [System.Serializable]
    public struct LevelProperties
    {
        [SerializeField] public int shotcountmax;
        [SerializeField] public float delaytime;    // Time between shots
        [SerializeField] public float autofiretime;    // Time between automatically shooting again
        [SerializeField] public float rechargetime;    // Time between adding ammo
        [SerializeField] public int levelenergy;
    }

    // Internal
    private bool active;

    [SerializeField] private Color primaryColor, accentColor;

    private int energy;  // Amount of energy weapon has
    private int levelmax;   // Max weapon level
    private int level;   // Current weapon level (Index 0 = Level 1)

    [SerializeField] private int ammomax = 0; // Max amount of ammo a weapon can have
    private int ammo;    // Current weapon ammo
    private int shotcount = 0;

    private float delayprogress;    // Used to delay shots
    private float autofireprogress;    // Used to delay shots
    private float rechargeprogress;    // Used to delay shots
    
    private float firebuffer;
    private float firebuffertime = 5.0f;

    [SerializeField] public GameObject[] projectiles;
    [SerializeField] public LevelProperties[] levelProperties;

    private Entity_Player player;
    private float hsign;    // Horizontal sign. {-1, 1}
    private float vsign;    // Vertical sign. {-1, 0, 1}

    // Projectile instantiation offsets
    private float shootoffset_leftright = 40.0f;
    private float shootoffset_centery = -16.0f;
    private float shootoffset_up = 56.0f;
    private float shootoffset_down = 56.0f;

    private PlayerData playerdata;

    // =======================================================================

    protected LevelProperties ActiveProperties {get {return levelProperties[GetLevelIndex()];}}

    // =======================================================================

    // Start is called before the first frame update
    protected virtual void Start()
    {
        ResetValues();

        levelmax = levelProperties.Length;
        playerdata = game.GetPlayerData();
        player = game.GetPlayer();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        bool cancontrol = !game.GameFlagGet(_GameHeader.GameFlag.lock_player);

        if (player == null) {return;}

        hsign = player.GetHSign();
        vsign = player.GetVSign();

        if (firebuffer > 0.0f)
        {
            firebuffer = Mathf.Max(0.0f, firebuffer-1.0f);
        }

        // Shoot
        if ( cancontrol && Input.GetButtonDown("Fire1") )
        {
            firebuffer = firebuffertime;
            if(!HasAmmo())
                game.PlaySound("EmptyWeapon");
        }

        // Autofire
        if ( cancontrol && Input.GetButton("Fire1") )
        {
            if (autofireprogress > 0.0f)
            {
                autofireprogress = Mathf.Max(0.0f, autofireprogress-1.0f);
                if (autofireprogress == 0.0f)
                {
                    autofireprogress = ActiveProperties.autofiretime;
                    firebuffer = 10.0f;

                    if(!HasAmmo())
                        game.PlaySound("EmptyWeapon");
                }
            }
        }

        // No recharge if key is held down
        else
        {
            if ( ammo < ammomax )   // Ammo is not at max
            {
                if (rechargeprogress < ActiveProperties.rechargetime)
                {
                    rechargeprogress += 1.0f;
                }
                else
                {
                    rechargeprogress = 0.0f;
                    AddAmmo(1);
                }
            }

            autofireprogress = ActiveProperties.autofiretime;
        }

        // Check for fire projectile
        if ( IsActive() )
        {
            if ( UpdateDelay() && BelowShotCount() && HasAmmo() )
            {
                // temp schüt
                if (firebuffer > 0.0f)
                {
                    firebuffer = 0.0f;
                    delayprogress = ActiveProperties.delaytime;

                    float xaim, yaim;

                    if (vsign != 0.0f && !(vsign == -1.0f && player.GetOnGround()) )
                    {
                        xaim = 0.0f;
                        yaim = vsign;
                    }
                    else
                    {
                        xaim = hsign;
                        yaim = 0.0f;
                    }

                    IncrementShotCount();
                    AddAmmo(-1);
                    
                    Fire(Mathf.Atan2(yaim, xaim));
                    OnShoot();
                    player.OnShoot();
                }
            }
            playerdata.SetAmmo(ammo, ammomax);  // Update Ammo Counts
        }
    }

    // Methods ====================================================

    public void ResetValues()
    {
        energy = 0;
        ammo = ammomax;
        shotcount = 0;
        delayprogress = 0;
        autofireprogress = ActiveProperties.autofiretime;
        rechargeprogress = 0;

        level = GetCurrentLevelIndex();
    }

    // Sets active weapon for shooting
    public void SetActive(bool isactive)
    {
        active = isactive;
        if (active)
        {
            playerdata.SetEnergy(CurrentLevelEnergy(), CurrentLevelEnergyMax());
            playerdata.SetLevel(GetLevel());
            playerdata.SetAmmo(ammo, ammomax);
        }
    }

    public bool IsActive() {return active;}

    // Shoots weapon projectile
    public virtual void Fire(float dir)
    {
        ShootProjectile(level, dir);
    }

    public WeaponProjectile ShootProjectile(int projectileindex, float dir)
    {
        WeaponProjectile proj = Instantiate(projectiles[projectileindex]).GetComponent<WeaponProjectile>();

        proj.transform.position = player.transform.position + new Vector3(
            Mathf.Cos(dir)*shootoffset_leftright, 
            (
                Mathf.Max(0.0f, Mathf.Sin(dir))*shootoffset_up + 
                Mathf.Min(0.0f, Mathf.Sin(dir))*shootoffset_down + 
                Mathf.Abs(Mathf.Cos(dir))*shootoffset_centery
            ), 
            -6.0f);
        
        proj.SetDirectionRad(dir, hsign);
        proj.SetSourceWeapon(this);

        return proj;
    }

    // Called when weapon is fired
    protected virtual void OnShoot()
    {

    }

    // Progress delay timer. Returns true if delay is zero
    protected bool UpdateDelay(float ts = 1.0f)
    {
        if (delayprogress > 0.0f) {delayprogress = Mathf.Max(delayprogress-ts, 0.0f);}
        return delayprogress == 0.0f;
    }

    // Returns true if delay is zero
    protected bool NoDelay() {return delayprogress == 0.0f;}

    // Returns true if shot count is less than max, or if there is no shot limit (max <= 0)
    protected bool BelowShotCount() {return (ActiveProperties.shotcountmax <= 0) || (shotcount < ActiveProperties.shotcountmax);}

    // Return true if weapon has ammo or infinite ammo (ammomax is <= 0)
    protected bool HasAmmo() {return ammomax <= 0 || ammo > 0;}

    public int GetAmmo() {return ammo;}    // Current ammo count
    public int GetAmmoMax() {return ammomax;}  // Max ammo count
    public int GetEnergy() {return energy;}    // Total weapon energy
    public int GetLevel() {return level+1;}  // Current weapon level
    public int GetLevelIndex() {return level;}  // Current weapon level index
    public int GetLevelMax() {return levelmax;}   // Max weapon level

    // Returns total amount of energy needed for level
    int CalcLevelEnergy(int _level)
    {
        return (GetLevelIndex() > 0)? energy - ActiveProperties.levelenergy: energy;
    }

    public int GetMaxEnergy()
    {
        return levelProperties[levelmax-1].levelenergy;
    }

    // Returns energy value for level (displayed in meter)
    public int CurrentLevelEnergy()
    {
        return (GetLevelIndex() > 0)? energy - levelProperties[GetLevelIndex()-1].levelenergy: energy;
    }
    
    // Returns energy value for level (displayed in meter)
    public int CurrentLevelEnergyMax()
    {
        return (GetLevelIndex() == 0)? 
            levelProperties[0].levelenergy: 
            levelProperties[GetLevelIndex()].levelenergy-levelProperties[GetLevelIndex()-1].levelenergy;
    }

    // Returns level using energy
    public int GetCurrentLevelIndex()
    {
        if (levelmax == 0) {return 0;}
        int e = energy;
        
        for (int i = 0; i < levelmax; i++)
        {
            if (e < levelProperties[i].levelenergy) {return i;}
        }
        return levelmax-1;
    }

    // Adds to current ammo count
    int AddAmmo(int value)
    {
        ammo = System.Math.Max(System.Math.Min(ammo+value, ammomax), 0);
        return ammo;
    }

    // Adds to max ammo count
    int ExtendAmmo(int value)
    {
        ammomax = System.Math.Max(ammomax+value, 0);
        return ammomax;
    }

    // Adds energy to weapon, increasing level if sufficient
    public int AddEnergy(int value, bool showgraphic = false)
    {
        if (levelmax == 0) {return value;}

        int e = value; // Leftover energy (if any)
        int lastlevel = level;

        // Value is positive
        if (value > 0)
        {
            if (energy+value > GetMaxEnergy())
            {
                e -= GetMaxEnergy()-energy;
                energy = GetMaxEnergy();
            }
            else
            {
                energy += e;
                e = 0;
            }
        }
        // Value is negative
        else if (value < 0)
        {
            if (energy+value < 0)
            {
                e -= energy;
                energy = 0;
            }
            else
            {
                energy += e;
                e = 0;
            }
        }

        // Update values
        level = GetCurrentLevelIndex();
        playerdata.SetEnergy(CurrentLevelEnergy(), CurrentLevelEnergyMax(), value > 0);
        playerdata.SetLevel(GetLevel());

        // Update UI
        if (value > 0 && e != value)
        {
            playerdata.EnergyFlashMeter();
        }

        if (level < lastlevel)
        {
            playerdata.EnergyMaximizeProvisional();
        }

        return e;
    }

    // Returns true if weapon delay is above zero
    bool InDelay()
    {
        return delayprogress > 0.0f;
    }

    public int IncrementShotCount()
    {
        shotcount += 1;
        return shotcount;
    }

    public int DecrementShotCount()
    {
        if (shotcount > 0) {shotcount -= 1;};
        return shotcount;
    }

    // Utility ====================================================

    public Color GetColorPrimary() {return primaryColor;}
    public Color GetColorAccent() {return accentColor;}
}
