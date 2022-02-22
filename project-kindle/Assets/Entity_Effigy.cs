using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_Effigy : Entity
{


    // Common ====================================================================

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (damageshake > 0)
        {
            damageshake = Mathf.Max(0.0f, damageshake - 1.0f);

            // Set x offset
            if (damageshake > 0)
            {
                float xshift = ((Mathf.Repeat(damageshake, 4.0f) < 2.0f)? -3: 3); // Shift left/right every 4 frames
                spriterenderer.transform.localPosition = new Vector3(xshift, 0.0f, 0.0f);
            }
            // Reset offset
            else
            {
                spriterenderer.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            }
            
        }
    }

    // Methods ====================================================================
}
