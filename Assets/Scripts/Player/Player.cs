using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(EffectableEntity))]
[RequireComponent(typeof(AnimationHandler))]
[RequireComponent(typeof(Displacable))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(Collidable))]
[RequireComponent(typeof(CombatStats))]
[RequireComponent(typeof(Keybindings))]
[RequireComponent(typeof(EquipmentHandler))]
[RequireComponent(typeof(CombatHandler))]
[RequireComponent(typeof(FamiliarHandler))]
[RequireComponent(typeof(EnchantableEntity))]
public class Player : MonoBehaviour
{
    [Header("Composition")]
    [SerializeField] private ComplexMovement mv;
    [SerializeField] private Displacable displace;
    [SerializeField] private Health health;
    [SerializeField] private Collidable interactionCollider;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private CombatStats stats;
    [SerializeField] private Inventory inventory;
    [SerializeField] private Stamina stamina;
    [SerializeField] private EquipmentHandler equipmentHandler;
    [SerializeField] private InputBuffer inputBuffer;
    [SerializeField] private Menu menu;
    [SerializeField] public Keybindings keybindings { get; private set; }
    [SerializeField] public CombatHandler combatHandler { get; private set; }
    [SerializeField] public FamiliarHandler familiarHandler { get; private set; }

    [Header("Flask")]
    [SerializeField] protected Flask flask;

    [Header("Animation Names")]
    [SerializeField] private string idleAnimation;
    [SerializeField] private string walkAnimation;
    [SerializeField] private string crouchAnimation;
    [SerializeField] private string crouchWalkAnimation;
    [SerializeField] private string risingAnimation;
    [SerializeField] private string fallingAnimation;
    [SerializeField] private string wallslideAnimation;
    [SerializeField] private string deadAnimation;

    [Header("Player Skills")]
    [SerializeField] public List<Skill> playerSkills;

    [Header("Temp UI STUFF")]
    [SerializeField] private BossHealthBarUI bossHealthBar;

    [SerializeField] private Enchantment enchantment;
    [SerializeField] private GameObject tempFam;

    public float regenTimer;

    public enum PlayerState
    {
        idle,
        walking,
        crouching,
        crouchWalking,
        airborne,
        wallsliding,
        walljumping,
        attacking,
        interacting,
        inMenu,
        knockedback,
        dead
    }
    [SerializeField] private PlayerState state;

    // Handle physics in Fixed Update, used for physics, ridgid bodys and collisions
    // Handle inputs in Update. simple timers, non-physics objects (don't include anything 'framerate' dependant)
    protected void Start()
    {
        mv = GetComponent<ComplexMovement>();
        animationHandler = GetComponent<AnimationHandler>();
        health = GetComponent<Health>();
        displace = GetComponent<Displacable>();
        interactionCollider = GetComponent<Collidable>();
        stats = GetComponent<CombatStats>();
        stamina = GetComponent<Stamina>();
        inventory = GetComponentInChildren<Inventory>();
        keybindings = GetComponent<Keybindings>();
        equipmentHandler = GetComponent<EquipmentHandler>();
        combatHandler = GetComponent<CombatHandler>();
        inputBuffer = GetComponent<InputBuffer>();
        familiarHandler = GetComponent<FamiliarHandler>();

        // Gets flask
        flask = GetComponentInChildren<Flask>();

        // Set player's stamina
        stamina = GetComponent<Stamina>();
        regenTimer = stamina.getRegenerationRate(); // Stamina regen timer

        // Disable boss healthbar for now
        bossHealthBar.gameObject.SetActive(false);

        // Set the player's start state
        state = PlayerState.idle;
    }

    protected void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.idle:
                animationHandler.changeAnimationState(idleAnimation);

                checkIfStaminaShouldRegen();

                handleMovementRequest();

                handleJumpRequest();

                // Conditions to change state
                handlePlayerAttackRequest();

                handleDisplacementRequest();

                if (inputBuffer.moveDirection != 0)
                    state = PlayerState.walking;
                if (inputBuffer.crouchRequest)
                    state = PlayerState.crouching;
                if(!mv.isGrounded())
                    state = PlayerState.airborne;

                break;
            case PlayerState.walking:
                animationHandler.changeAnimationState(walkAnimation);

                handleMovementRequest();

                handleJumpRequest();

                checkIfStaminaShouldRegen();

                // Conditions to change state
                handlePlayerAttackRequest();

                handleDisplacementRequest();

                if (inputBuffer.moveDirection == 0)
                    state = PlayerState.idle;
                if (inputBuffer.crouchRequest && inputBuffer.moveDirection != 0)
                    state = PlayerState.crouchWalking;
                if (!mv.isGrounded())
                    state = PlayerState.airborne;
                    
                break;
            case PlayerState.crouching:
                animationHandler.changeAnimationState(crouchAnimation);

                pickUpNearbyItems();

                checkIfStaminaShouldRegen();

                handleDisplacementRequest();

                // Conditions to change state
                if (inputBuffer.crouchRequest && inputBuffer.moveDirection != 0)
                    state = PlayerState.crouchWalking;
                if (!inputBuffer.crouchRequest)
                    state = PlayerState.idle;
                if (!mv.isGrounded())
                    state = PlayerState.airborne;

                break;
            case PlayerState.crouchWalking:
                animationHandler.changeAnimationState(crouchWalkAnimation);

                mv.crouchWalk(inputBuffer.moveDirection * (1 + stats.movespeedMultiplier));

                pickUpNearbyItems();

                checkIfStaminaShouldRegen();

                handleDisplacementRequest();

                // Conditions to change state
                if (!(inputBuffer.crouchRequest && inputBuffer.moveDirection != 0))
                    state = PlayerState.crouching;
                if (!mv.isGrounded())
                    state = PlayerState.airborne;

                break;
            case PlayerState.airborne:
                // Allow air control
                mv.Walk(inputBuffer.moveDirection * (1 + stats.movespeedMultiplier));

                // Check if the player is falling or rising
                if (mv.checkFalling())
                    animationHandler.changeAnimationState(fallingAnimation);
                if (mv.checkRising())
                    animationHandler.changeAnimationState(risingAnimation);

                handleDisplacementRequest();

                // Change state if you are not airborne
                if (mv.onWall() && !mv.isGrounded() && inputBuffer.moveDirection != 0)
                    state = PlayerState.wallsliding;
                else if (mv.isGrounded())
                {
                    state = PlayerState.idle;
                }

                break;
            case PlayerState.wallsliding:
                animationHandler.changeAnimationState(wallslideAnimation);

                mv.wallSlide();
                mv.Walk(inputBuffer.moveDirection * (1 + stats.movespeedMultiplier));

                handleDisplacementRequest();

                // Conditions to change state
                if (inputBuffer.jumpRequest && stamina.currentStamina >= mv.getWallSlideStaminaDrain())
                {
                    mv.wallJump();
                    stamina.drainStamina(mv.getWallSlideStaminaDrain());
                    StartCoroutine(wallJumpTimer());
                    state = PlayerState.walljumping;
                }
                else if (mv.isGrounded())
                    state = PlayerState.idle;
                else if (!mv.onWall())
                    state = PlayerState.airborne;
                break;
            case PlayerState.walljumping:

                handleDisplacementRequest();

                // Check if the player is falling or rising
                if (mv.checkFalling())
                    animationHandler.changeAnimationState(fallingAnimation);
                if (mv.checkRising())
                    animationHandler.changeAnimationState(risingAnimation);

                break;
            case PlayerState.attacking:
                // Let the respective attack handle the player movement during the attack

                handleComboRequest();

                handleDisplacementRequest();

                if (!combatHandler.attacking)
                    state = PlayerState.idle;
                break;
            case PlayerState.interacting:
                // TODO

                break;
            case PlayerState.inMenu:
                // Player is in menu

                // Play idle animation
                animationHandler.changeAnimationState(idleAnimation);

                // Conditions to change state
                if (!inputBuffer.menuToggleRequest)
                {
                    menu.menuEnabled = false;
                    state = PlayerState.idle;
                }
                break;
            case PlayerState.knockedback:
                // TODO
                animationHandler.changeAnimationState(idleAnimation);

                //handleDisplacementRequest();

                displace.performDisplacement();

                if (!displace.isDisplaced)
                    state = PlayerState.idle;

                break;
            case PlayerState.dead:
                // Do nothing, the game is over lol
                animationHandler.changeAnimationState(deadAnimation);

                break;
        }

        // If the player's health drops to 0 at ANY point, set the player state to dead
        if (health.isEmpty() && state != PlayerState.dead)
        {
            mv.Stop();
            state = PlayerState.dead;
        }
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            /*var body = GetComponent<Rigidbody2D>();
            if(body != null)
            {
                Debug.Log("added force");
                body.AddForce(Vector2.right * 3, ForceMode2D.Impulse);
            }*/
            var shield = GetComponentInChildren<PlayerShield>();
            shield.raiseShield();

        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            //familiarHandler.despawnFamiliar(tempFam);
            /*var ench = GetComponent<EnchantableEntity>();
            if (ench != null)
            {
                ench.removeEnchantment(enchantment);
            }*/
            var shield = GetComponentInChildren<PlayerShield>();
            shield.lowerShield();
        }
        if (playerIsFree())
        {
            // Temp sprinting function
            if (inputBuffer.sprintRequest)
                stats.movespeedMultiplier = 1f;
            else
                stats.movespeedMultiplier = 0;

            // Menu logic
            handleMenuRequest();

            // Use flask
            if (inputBuffer.useFlaskRequest)
                useFlask();

            // Interacting with surroundings
            if (inputBuffer.interactRequest)
                interactWithNearbyObjects();
        }
    }

    private void handleMovementRequest()
    {
        mv.Walk(inputBuffer.moveDirection * (1 + stats.movespeedMultiplier));
    }

    private void handleJumpRequest()
    {
        if (inputBuffer.jumpRequest && mv.isGrounded())
            mv.Jump();
    }

    private void handleDisplacementRequest()
    {
        if (displace.isDisplaced)
        {
            interuptCurrentAction();
            state = PlayerState.knockedback;
        }
    }

    private void handlePlayerAttackRequest()
    {
        // You can only attack if you are grounded
        if (!mv.isGrounded())
            return;

        // Get input from buffer and do the attack
        if (equipmentHandler.weapon != null && inputBuffer.lightAttackRequest)
        {
            combatHandler.attemptToLightAttack();
            state = PlayerState.attacking;
        } 
        else if (equipmentHandler.weapon != null && inputBuffer.heavyAttackRequest)
        {
            combatHandler.attemptToHeavyAttack();
            state = PlayerState.attacking;
        }
        else if (inputBuffer.signatureAbilityRequest)
        {
            combatHandler.attemptToUseSignatureAbility();
            state = PlayerState.attacking;
        }
        else if (inputBuffer.utilityAbilityRequest)
        {
            combatHandler.attemptToUseUtilityAbility();
            state = PlayerState.attacking;
        }
    }

    private void handleComboRequest()
    {
        if (equipmentHandler.weapon != null && inputBuffer.lightAttackRequest)
            combatHandler.attemptToLightAttack();
    }

    private void handleMenuRequest()
    {
        // Enable menu
        if (inputBuffer.menuToggleRequest)
        {
            menu.menuEnabled = true;
            mv.Stop();
            state = PlayerState.inMenu;
        }
    }

    private bool playerIsFree()
    {
        return state == PlayerState.idle || state == PlayerState.walking || state == PlayerState.airborne || state == PlayerState.crouching || state == PlayerState.crouchWalking;
    }

    private void checkIfStaminaShouldRegen()
    {
        if (!stamina.isFull()) // Don't regen stamina if stamina is full, during wallsliding/climbing, or during defense
        {
            if (regenTimer > 0)
            {
                regenTimer -= Time.deltaTime;
            }
            else
            {
                stamina.regenerateStamina();
                regenTimer = stamina.getRegenerationRate();
            }
        }
    }

    private IEnumerator wallJumpTimer()
    {
        state = PlayerState.walljumping;
        yield return new WaitForSeconds(mv.wallJumpTime);
        state = PlayerState.airborne;
    }

    protected void pickUpNearbyItems()
    {
        interactionCollider.checkCollisions(pickUp);
    }

    protected void pickUp(Collider2D coll)
    {
        var worldItem = coll.GetComponent<WorldItem>();
        if (worldItem != null)
        {
            GameManager.instance.CreatePopup("You picked up " + worldItem.GetItem().name, transform.position);
            inventory.addItem(worldItem.GetItem());

            Destroy(coll.gameObject);
        }
    }

    protected void interactWithNearbyObjects()
    {
        interactionCollider.checkCollisions(interactWith);
    }

    protected void interactWith(Collider2D coll)
    {
        var interactable = coll.GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.interactWith();
        }
    }

    protected void useFlask()
    {
        if(health.isFull())
        {
            GameManager.instance.CreatePopup("You are already at full Health.", transform.position);
            return;
        }

        if (flask.use()) // Uses flask and checks result
        {
            heal((int)(health.getMaxHP() * flask.getHealPercentage()));
        }
        else
            GameManager.instance.CreatePopup("Your flask is empty.", transform.position);
    }

    public void heal(int amount)
    {
        GameManager.instance.CreatePopup("You have healed " + amount + " HP.", transform.position);
        health.increaseHealth(amount);
    }

    // Interupts any action that the player is doing
    public void interuptCurrentAction()
    {
        // If player has weapon in action, stop it
        combatHandler.cancelCurrentAttack();

        // Send event that any action was completed
        GameEvents.current.triggerActionFinish();
    }

    // Skill handling
    public List<Skill> getPlayerSkills()
    {
        return playerSkills;
    }

    // Assumes that you already know the prereq skills and knowledge
    public void learnSkill(Skill skill)
    {
        GameManager.instance.CreatePopup("You have learned the skill: " + skill.name, transform.position);
        // Nested switch in order to decide on what changes the skill should do
        if (skill is ActiveSkill)
        {
            // Create a copy so if we ever edit the ability, we won't affect real thing
            var abilityCopy = Instantiate(((ActiveSkill)skill).ability);

            if (combatHandler.signatureAbilityHolder.isEmpty())
                combatHandler.equipSignatureAbility(abilityCopy);

            combatHandler.allPlayerAbilities.Add(abilityCopy);
            playerSkills.Add(skill);
        }
        else
        {
            // Logic of adding ability upgrade to appropriate ability

            // Go down the list of prereq's of the upgrade until associated ability is found
            var prereq = skill.prerequisite;
            while (!(prereq is ActiveSkill)) 
            {
                // If any abiltiy is null, then give error
                if (prereq is null)
                    throw new NullReferenceException("Active skill related to " + skill.name + " was not found.");
                prereq = prereq.prerequisite;
            }

            // Assume you found the correct ability
            // Then look for that ability in play abilites
            foreach(var ability in combatHandler.allPlayerAbilities)
            {
                // Check if the player has an ability with the same name as the 
                if (ability.name == ((ActiveSkill)prereq).ability.name)
                {
                    // If the passive skill has no upgrade, give error
                    if (((PassiveSkill)skill).upgrade == null)
                        throw new NullReferenceException("The skill you waned to add was null, dumbass");

                    // Add the upgrade to the correct ability
                    ability.addUpgrade(((PassiveSkill)skill).upgrade);

                    // Then call the upgrades instatiation if the ability is already active
                    if (combatHandler.signatureAbilityHolder.getAbility()?.name == ability.name || combatHandler.utilityAbilityHolder.getAbility()?.name == ability.name)
                    {
                        ((PassiveSkill)skill).upgrade.upgradeInstaniation(gameObject, ability);
                    }

                    // Give feedback and add to players total list of skills
                    playerSkills.Add(skill);
                    break;
                }
            }

        }
    }

    public void setBossHealthBar(EnemyAI boss)
    {
        if(boss == null)
        {
            bossHealthBar.gameObject.SetActive(false);
        }
        else
        {
            bossHealthBar.setBoss(boss);
            bossHealthBar.gameObject.SetActive(true);
        }
    }
}