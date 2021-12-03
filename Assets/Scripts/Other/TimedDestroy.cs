using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    [SerializeField] public float secondsToDestroy;
    private void Start()
    {
        if(secondsToDestroy > 0)
            Destroy(gameObject, secondsToDestroy);
    }

    public void setDestroyTimer(float value)
    {
        Destroy(gameObject, value);
    }
}
