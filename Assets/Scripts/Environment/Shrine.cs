using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collidable))]
[RequireComponent(typeof(Interactable))]
public abstract class Shrine : MonoBehaviour
{
    [SerializeField] protected Collidable collider;
    [SerializeField] protected Interactable interactable;

    protected virtual void Start() 
    { 
        collider = GetComponent<Collidable>();
        interactable = GetComponent<Interactable>();
    }

    public abstract void activate();
}
