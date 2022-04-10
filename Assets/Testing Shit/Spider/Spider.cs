using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    private Animator animator;
    private DamageFlash damageFlash;
    private DamageParticles damageParticles;
    private Rigidbody2D rb;
    private Movement mv;
    [SerializeField] private float hurtTime;
    [SerializeField] private float pushForce;
    [SerializeField] private float pushDuration;
    [SerializeField] private Transform bleedpoint;

    Vector2 hitPoint;
    private bool isHurt;
    private float pushTimer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        damageFlash = GetComponent<DamageFlash>();
        rb = GetComponent<Rigidbody2D>();
        mv = GetComponent<Movement>();
        damageParticles = GetComponent<DamageParticles>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) {
            // Get mouse position
            hitPoint = UnityEngine.Camera.main.ScreenToWorldPoint(Input.mousePosition);
            StartCoroutine(hurt(hurtTime));
        }
    }

    private void FixedUpdate() {
        if (isHurt) {
            pushForce = Mathf.Lerp(pushForce, 0, pushTimer / pushDuration);
            Vector2 pushDirection = (Vector2)transform.position - hitPoint;
            mv.setFacingDirection(-pushDirection.normalized.x);
            rb.velocity = new Vector2(pushDirection.normalized.x * pushForce * Time.deltaTime, rb.velocity.y);
        }
        else {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    private IEnumerator hurt(float time) {
        animator.Play("Hurt");
        if (damageFlash != null)
            damageFlash.Flash();
        // Set velocity
        Vector2 pushDirection = (Vector2)transform.position - hitPoint;
        rb.velocity = new Vector2(pushDirection.normalized.x * pushForce * Time.deltaTime, rb.velocity.y);

        if (damageParticles != null)
            damageParticles.spawnDamageParticles();
        
        isHurt = true;
        yield return new WaitForSeconds(time);
        isHurt = false;
        animator.Play("Idle");
    }
}
