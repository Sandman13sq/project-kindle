using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using static DmrMath;

public class Player_GameOver : MasterObject
{
    [SerializeField] private SpriteRenderer spriterenderer;
    private Color color1 = new Color(0.0f, 0.0f, 0.0f, 0.0f);
    private Color color2 = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    private float fadestep = 0.0f;

    int state;

    public static Player_GameOver instance;

    void Awake()
    {
        // Enforce singleton behavior
        if (instance == null) {instance = this;}
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        fadestep = 0.0f;
        state = 0;
        game.GameFlagSet(GameHeader.GameFlag.lock_player);

        Update();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 campos = game.GetCameraPosition();
        transform.position = new Vector3(
            campos.x, campos.y, -14.0f
        );

        switch(state)
        {
            // Fade and slowdown
            case(0): {
                const float steptime = 60.0f;

                Application.targetFrameRate = 30;
                fadestep += 1.0f;
                spriterenderer.color = Color.Lerp(color1, color2, fadestep/steptime);

                if (fadestep > steptime)
                {
                    state++;
                    fadestep = 0.0f;
                }
                break;
            }

            // Wait to restart
            case(1): {
                const float steptime = 60.0f;

                fadestep += 1.0f;
                if (fadestep > steptime)
                {
                    state++;
                    fadestep = 0.0f;

                    Application.targetFrameRate = 60;

                    game.GetPlayer().ResetHealth();
                    game.GetPlayerData().ResetWeapons();

                    // Restart scene
                    UnityEngine.SceneManagement.SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                }
                break;
            }
                
            // Fade from white
            case(2): {
                const float steptime = 60.0f;
                spriterenderer.color = Color.Lerp(color2, color1, fadestep/steptime);

                fadestep += 1.0f;
                if (fadestep > steptime)
                {
                    state++;
                    fadestep = 0.0f;
                    Destroy(gameObject);
                }
                break;
            }
        }
    }
}
