using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Movement))]
[RequireComponent(typeof(Health))]
[RequireComponent(typeof(Stamina))]
[RequireComponent(typeof(EffectableEntity))]
[RequireComponent(typeof(AnimationHandler))]
[RequireComponent(typeof(Damageable))]
[RequireComponent(typeof(CombatStats))]
[RequireComponent(typeof(Keybindings))]
[RequireComponent(typeof(EquipmentHandler))]
[RequireComponent(typeof(CombatHandler))]
[RequireComponent(typeof(FamiliarHandler))]
[RequireComponent(typeof(EnchantableEntity))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(RollingHandler))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ComplexMovement mv;
    [SerializeField] private PlatformHandler platformHandler;
    [SerializeField] private Displacable displace;
    [SerializeField] private Health health;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private CombatStats stats;
    [SerializeField] private Stamina stamina;
    [SerializeField] private InputBuffer inputBuffer;
    [SerializeField] private Menu menu;
    [SerializeField] private CombatHandler combatHandler;
    [SerializeField] private RollingHandler rollingHandler;

    [Header("Items")]
    [SerializeField] private Inventory inventory;
    [SerializeField] protected Flask flask;

    [Header("Animation")]
    [SerializeField] private string idleAnimation;
    [SerializeField] private string walkAnimation;
    [SerializeField] private string crouchAnimation;
    [SerializeField] private string crouchWalkAnimation;
    [SerializeField] private string risingAnimation;
    [SerializeField] private string fallingAnimation;
    [SerializeField] private string wallslideAnimation;
    [SerializeField] private string staggerAnimation;
    [SerializeField] private string deadAnimation;

    [Header("Player Skills")]
    [SerializeField] public List<Skill> playerSkills;

    [Header("Temp UI STUFF")]
    [SerializeField] private BaseEffect burnEffect;
 
    public float regenTimer;
    private bool isJump;
    [SerializeField] private PlayerState savedState;

    [SerializeField] private bool enableWalljump = false;

    public enum PlayerState
    {
        idle,
        walking,
        crouching,
        crouchWalking,
        airborne,
        wallsliding,
        walljumping,
        rolling,
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
        platformHandler = GetComponent<PlatformHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        health = GetComponent<Health>();
        displace = GetComponent<Displacable>();
        stats = GetComponent<CombatStats>();
        stamina = GetComponent<Stamina>();
        inventory = GetComponentInChildren<Inventory>();
        combatHandler = GetComponent<CombatHandler>();
        inputBuffer = GetComponent<InputBuffer>();
        rollingHandler = GetComponent<RollingHandler>();

        // Gets flask
        flask = GetComponentInChildren<Flask>();

        // Set player's stamina
        stamina = GetComponent<Stamina>();
        regenTimer = stamina.getRegenerationRate(); // Stamina regen timer

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

                if (inputBuffer.moveDirection != 0)
                    state = PlayerState.walking;
                if (inputBuffer.crouchRequest)
                    state = PlayerState.crouching;
                if(!mv.isGrounded())
                    state = PlayerState.airborne;
                
                handleRollRequest();

                handlePlayerAttackRequest();

                handleDisplacementRequest();

                break;
            case PlayerState.walking:
                animationHandler.changeAnimationState(walkAnimation);

                checkIfStaminaShouldRegen();

                handleMovementRequest();

                handleJumpRequest();

                if (inputBuffer.moveDirection == 0)
                    state = PlayerState.idle;
                if (inputBuffer.crouchRequest && inputBuffer.moveDirection != 0)
                    state = PlayerState.crouchWalking;
                if (!mv.isGrounded())
                    state = PlayerState.airborne;
                
                handleRollRequest();

                handlePlayerAttackRequest();

                handleDisplacementRequest();

                break;
            case PlayerState.crouching:
                animationHandler.changeAnimationState(crouchAnimation);

                checkIfStaminaShouldRegen();

                handleDropDownRequst();

                pickUpNearbyItems();

                // Conditions to change state
                if (inputBuffer.crouchRequest && inputBuffer.moveDirection != 0)
                    state = PlayerState.crouchWalking;
                if (!inputBuffer.crouchRequest)
                    state = PlayerState.idle;
                if (!mv.isGrounded())
                    state = PlayerState.airborne;

                handleRollRequest();

                handleDisplacementRequest();

                break;
            case PlayerState.crouchWalking:
                animationHandler.changeAnimationState(crouchWalkAnimation);

                mv.crouchWalk(inputBuffer.moveDirection * (1 + stats.movespeedMultiplier));

                checkIfStaminaShouldRegen();

                handleDropDownRequst();

                pickUpNearbyItems();

                // Conditions to change state
                if (!(inputBuffer.crouchRequest && inputBuffer.moveDirection != 0))
                    state = PlayerState.crouching;
                if (!mv.isGrounded())
                    state = PlayerState.airborne;

                handleRollRequest();

                handleDisplacementRequest();

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
                if (enableWalljump && mv.onWall() && !mv.isGrounded() && inputBuffer.moveDirection != 0)
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
            case PlayerState.rolling:
                
                rollingHandler.roll();
                
                if (rollingHandler.isDoneRolling()) {
                    rollingHandler.startCooldown();
                    stats.percentDodgeChance -= 1f;
                    state = PlayerState.idle;
                }

            break;
            case PlayerState.attacking:
                // Let the respective attack handle the player movement during the attack

                if (!inputBuffer.mainHandAttackRequest) {
                    // Attempt to release weapon
                    combatHandler.mainHandRelease(inputBuffer.mainAttackTime);
                }

                if (!inputBuffer.offHandAttackRequest) {
                    // Attempt to release weapon
                    combatHandler.offHandRelease(inputBuffer.mainAttackTime);
                }
                
                // For handling combos
                handlePlayerAttackRequest();

                // If the weapon is done attacking, then return player to idle state
                if (combatHandler.isDoneAttacking()) {
                    inputBuffer.resetAttackRequests();
                    state = PlayerState.idle;
                    return;
                }

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
                animationHandler.changeAnimationState(staggerAnimation);

                mv.setFacingDirection(-displace.getKnockbackDirection());

                handleDisplacementRequest();

                displace.performDisplacement();

                if (!displace.isDisplaced()) {
                    state = (PlayerState)System.Enum.Parse(typeof(PlayerState), displace.loadState());
                }    

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
        if (Input.GetKeyDown(KeyCode.G))  // used for testing
        {
            // Nothin
            displace.triggerKnockback(200, 0.25f, transform.position + Vector3.right);
        }
        if (Input.GetKeyDown(KeyCode.H))  // used for testing
        {
            // Nothin
            //displace.triggerStun(1f);
            GetComponent<EffectableEntity>().addEffect(burnEffect.InitializeEffect(gameObject));
            
        }

        if (health.isEmpty() && state != PlayerState.dead) {
            // Stop
            mv.Walk(0);

            state = PlayerState.dead;
        }


        if (playerIsFree())
        {
            // Handle input
            if (mv.isGrounded() && inputBuffer.jumpRequest)
                isJump = true;

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
        if (isJump && mv.isGrounded()) {
            mv.Jump();
            isJump = false;
        }
    }

    private void handleDisplacementRequest()
    {
        // If you are displaced
        if (displace.isDisplaced()) {
            // Save your current state
            displace.saveState(state.ToString());
            state = PlayerState.knockedback;
        }
    }

    private void handleDropDownRequst() {
        if (inputBuffer.dropDownRequest) {
            platformHandler.dropFromPlatform();
        }
    }

    private void handleRollRequest() {
        if (inputBuffer.rollRequest && rollingHandler.canRoll()) {
            if (inputBuffer.moveDirection > 0.2f)
                rollingHandler.startRoll(1);
            else if (inputBuffer.moveDirection < -0.2f)
                rollingHandler.startRoll(-1);
            else 
                rollingHandler.startRoll(mv.getFacingDirection());

            // Give 100% dodge chance during roll
            stats.percentDodgeChance += 1f;

            state = PlayerState.rolling;
        }
    }

    private void handlePlayerAttackRequest()
    {
        // You can only attack if you are grounded
        if (!mv.isGrounded())
            return;

        if (inputBuffer.mainHandAttackRequest && combatHandler.mainHandAttack())
        {
            state = PlayerState.attacking;
        }
        else if (inputBuffer.offHandAttackRequest && combatHandler.offhandAttack())
        {
            state = PlayerState.attacking;
        }
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

    private void pickUpNearbyItems()
    {   
        var collider2D = GetComponent<Collider2D>();
        var droppedItems = Physics2D.OverlapBoxAll((Vector2)collider2D.transform.position + collider2D.offset, collider2D.bounds.size, 
                            0, 1 << LayerMask.NameToLayer("Loot"));
        if (droppedItems.Length == 0) {
            return;
        }
            
        foreach (var droppedItem in droppedItems) {
            var worldItem = droppedItem.GetComponent<WorldItem>();
            if (worldItem != null)
            {
                GameManager.instance.CreatePopup("You picked up " + worldItem.GetItem().name, transform.position);
                inventory.addItem(worldItem.GetItem());

                Destroy(droppedItem.gameObject);
            }
        }
    }

    private void interactWithNearbyObjects()
    {
        var collider2D = GetComponent<Collider2D>();
        var iteractables = Physics2D.OverlapBoxAll(collider2D.transform.position, collider2D.bounds.size, 0, 1 << LayerMask.NameToLayer("Interactables"));
        if (iteractables.Length == 0) {
            return;
        }
            
        foreach (var hit in iteractables) {
            if (hit.TryGetComponent(out Shrine shrine)) {
                shrine.activate(this);
            }

            if (hit.TryGetComponent(out Chest chest)) {
                chest.open();
            }
            
            if (hit.TryGetComponent(out Door door)) {
                door.open();
            }
        }
    }

    private void useFlask()
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

    private void heal(int amount)
    {
        GameManager.instance.CreatePopup("You have healed " + amount + " HP.", transform.position);
        health.increaseHealth(amount);
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

    private void OnDrawGizmosSelected() {
        var collider2D = GetComponent<Collider2D>();
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube((Vector2)collider2D.transform.position + collider2D.offset, collider2D.bounds.size);
    }
}