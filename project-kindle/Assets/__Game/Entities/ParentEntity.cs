using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentEntity : MonoBehaviour
{
    // Only exists to hold parent entity references
    [SerializeField] Entity entity;

    public void SetEntity(Entity e) {entity = e;}
    public Entity GetEntity() {return entity;}
}
