using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldShrine : Shrine
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Values")]
    [SerializeField] private int goldCap;
    [SerializeField] private int storedGold;
    [SerializeField] private bool used;

    [Header("Animation")]
    [SerializeField] private string idleAnimation;
    [SerializeField] private string addGoldAnimation;
    [SerializeField] private string activateAnimation;

    private void Start()
    {
        animator = GetComponent<Animator>();
        storedGold = 0;
        used = false;
    }

    public void addGold(int amount) 
    {
        used = false;
        if (storedGold >= goldCap) {
            return;
        }
        animator.Play(addGoldAnimation);
        storedGold += amount;
    }

    public override void activate(Player player)
    {
        if (!used) 
        {  
            // Take stored gold
            animator.Play(activateAnimation);
            GameManager.instance.CreatePopup("+ " + storedGold + " GOLD", transform.position, Color.yellow);
            GameManager.instance.addGold(storedGold);
            storedGold = 0;
            used = true;
        }
        
    }
}
