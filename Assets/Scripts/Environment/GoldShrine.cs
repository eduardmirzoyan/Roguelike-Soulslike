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

            // Heal 10 hp
            player.heal(15);
        }
        else {
            PopUpTextManager.instance.createPopup("Offer 50 gold in exchange for some life essence.", Color.gray, transform.position);
        }
        
    }
}
