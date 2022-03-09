using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputPrinter : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("A Button")){
            Debug.Log("A button is being pressed!");
        }
    }
}
