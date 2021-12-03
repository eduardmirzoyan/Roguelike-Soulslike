using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Player player;
    [SerializeField] public bool isActive;

    private void Start()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    // Update is called once per frame
    private void Update()
    {
        animator.SetBool("isRaised", isActive);
/*        if (player.isInMenu)
        {
             animator.SetBool("isRaised", true);
        }
        else
        {
            animator.SetBool("isRaised", false);
        }*/
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
