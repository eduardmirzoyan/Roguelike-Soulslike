using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class Door : MonoBehaviour
{
    public void open()
    {
        LevelManager.instance.loadNextLevel();
    }
}
