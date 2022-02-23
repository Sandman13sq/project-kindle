using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempCamera : MonoBehaviour
{
    [SerializeField] private GameObject targetobj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetobj == null) {return;}

        Vector3 possrc = transform.position;
        Vector3 posdest = targetobj.transform.position;
        const float div = 16.0f;

        transform.position = new Vector3(
            possrc.x + (posdest.x-possrc.x)/div,
            possrc.y + (posdest.y-possrc.y)/div,
            possrc.z
        );
    }
}
