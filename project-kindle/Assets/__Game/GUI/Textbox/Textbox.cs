using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Textbox : MonoBehaviour
{
    // ====================================================================

    // Variables

    [SerializeField] private UnityEngine.UI.Image boxsprite;
    [SerializeField] private UnityEngine.UI.Image arrowsprite;
    [SerializeField] private RectTransform boxrect;
    [SerializeField] private UnityEngine.UI.Text textcomponent;

    enum State
    {
        open,
        print,
        wait,
        close
    }

    private State state;
    private float statestep;
    private const float maxwidth = 700.0f;
    private const float maxheight = 100.0f;

    [SerializeField] string textstring;
    private string textstringshow;
    private int textpos;
    private float arrowxstart;

    // ====================================================================

    // Functions

    // Start is called before the first frame update
    void Start()
    {
        textstringshow = "";
        textpos = 0;
        textcomponent.text = "";

        boxrect.sizeDelta = new Vector2(0.0f, 0.0f);
        arrowxstart = arrowsprite.transform.position.x;
        arrowsprite.enabled = false;

        statestep = 0.0f;
        state = State.open;
    }

    // Update is called once per frame
    void Update()
    {
        switch(state)
        {
            // ------------------------------------------------------------------
            case(State.open):   // Text box grows into size. Moves to "print" state when done
            {
                const float statetime = 10.0f;
                arrowsprite.enabled = false;
                textcomponent.enabled = false;

                // Update rectangle size
                if (statestep < statetime)
                {
                    statestep += 1.0f;
                    boxrect.sizeDelta = new Vector2(
                        maxwidth*(statestep/statetime),
                        maxheight*(statestep/statetime)
                    );
                }
                // Move to "print" state
                else
                {
                    statestep = 0.0f;
                    state = State.print;
                }
            }
            break;

            // ------------------------------------------------------------------
            case(State.print):  // Text is updated. Moves to "wait" state when done
            {
                arrowsprite.enabled = false;
                textcomponent.enabled = true;

                // Increase text position every frame
                if (textpos < textstring.Length)
                {
                    textpos += 1;
                    textstringshow = textstring.Substring(0, textpos);
                    textcomponent.text = textstringshow;
                }
                // Move to "wait" state
                else
                {
                    statestep = 0.0f;
                    state = State.wait;
                }
            }
            break;

            // ------------------------------------------------------------------
            case(State.wait):   // Idle state
            {
                statestep += 0.1f;
                arrowsprite.enabled = true;
                textcomponent.enabled = true;

                // Arrow points right
                arrowsprite.transform.position = new Vector2(
                    arrowxstart + Mathf.Sin(statestep)*4.0f,
                    arrowsprite.transform.position.y
                    );
                
                // Move to close state (temporary)
                if (statestep >= 20.0f)
                {
                    statestep = 0.0f;
                    state = State.close;
                }
            }
            break;

            // ------------------------------------------------------------------
            case(State.close):  // Text box shrinks
            {
                const float statetime = 10.0f;
                arrowsprite.enabled = false;
                textcomponent.enabled = false;
                
                if (statestep < statetime)
                {
                    statestep += 1.0f;
                    boxrect.sizeDelta = new Vector2(
                        maxwidth*(1.0f-statestep/statetime),
                        maxheight*(1.0f-statestep/statetime)
                    );
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            break;
        }
    }
}
