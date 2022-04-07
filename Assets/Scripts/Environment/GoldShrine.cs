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
        
        if (GameManager.instance.gold > 50) 
        {  
            // Play animation
            animator.Play(activateAnimation);

            // Take gold
            GameManager.instance.addGold(-50);

            // Heal 50 hp
            player.heal(50);
        }
        else {
            GameManager.instance.CreatePopup("Not enough gold.", transform.position, Color.gray);
        }
        
    }
}
