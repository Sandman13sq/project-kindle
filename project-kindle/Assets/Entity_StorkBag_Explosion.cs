using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity_StorkBag_Explosion : Entity
{
    [SerializeField] private CircleCollider2D circlecollider;
    [SerializeField] private GameObject defeatcloud_prefab;
    private float life = 2.0f;
    private float radius;

    // Start is called before the first frame update
    void Start()
    {
        radius = circlecollider.radius;
        
        for (var i = 0; i < 5; i++)
        {
            Instantiate(defeatcloud_prefab).transform.position = transform.position + new Vector3(
                Random.Range(-radius, radius),
                Random.Range(-radius, radius),
                0f
            );
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (life > 0.0f) {life -= game.GetTrueTimeStep();}
        else
        {
            Destroy(gameObject);
        }
    }
}
