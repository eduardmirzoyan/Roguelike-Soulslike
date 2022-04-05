using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class MenuUI : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private bool isActive;

    private void Start()
    {
        isActive = false;
    }

    // Update is called once per frame
    private void Update()
    {
        animator.SetBool("isVisible", isActive);
    }

    public void show()
    {
        isActive = true;
    }
    public void hide()
    {
        isActive = false;
    }
}
