using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private bool isLocked = false;

    public void enter()
    {
        if (!isLocked)
            LevelManager.instance.loadNextLevel(new Vector3(7, 6, 0));
        else {
            PopUpTextManager.instance.createVerticalPopup("Door is locked.", Color.gray, transform.position);
        }
    }
}
