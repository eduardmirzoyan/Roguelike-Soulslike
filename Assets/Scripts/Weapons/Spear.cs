using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear : MonoBehaviour
{
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private string attackName;
    [SerializeField] private int maxCombo = 2;
    [SerializeField] private int comboValue = 0;

    [Header("Damage")]
    [SerializeField] private int damage;
    [SerializeField] private float attackSpeed = 1f;

    // Start is called before the first frame update
    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
    }

    public void attack() {
        // Reset combo if at max value
        if (comboValue > maxCombo) {
            comboValue = 0;
        }
        
        print(attackName + " " + comboValue);
        animationHandler.changeAnimationState(attackName + " " + comboValue);
        comboValue++;
    }

    public string getAttackName() {
        return attackName + " " + comboValue;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent(out Damageable damageable) &&  other.gameObject != this.gameObject) {
            Damage dmg = new Damage
            {
                source = DamageSource.fromPlayer,
                damageAmount = damage,
                origin = transform.parent,
                isTrue = false,
                isAvoidable = true,
                triggersIFrames = true,
            };
            damageable.takeDamage(dmg);
        }
    }
}
