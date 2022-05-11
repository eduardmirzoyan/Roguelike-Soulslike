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
[RequireComponent(typeof(Stats))]
[RequireComponent(typeof(Keybindings))]
[RequireComponent(typeof(EquipmentHandler))]
[RequireComponent(typeof(CombatHandler))]
[RequireComponent(typeof(FamiliarHandler))]
[RequireComponent(typeof(EnchantableEntity))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(RollingHandler))]
[RequireComponent(typeof(InteractionHandler))]
public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ComplexMovement mv;
    [SerializeField] private PlatformHandler platformHandler;
    [SerializeField] private Displacable displace;
    [SerializeField] private Health health;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Stats stats;
    [SerializeField] private Stamina stamina;
    [SerializeField] private InputBuffer inputBuffer;
    [SerializeField] private Menu menu;
    [SerializeField] private CombatHandler combatHandler;
    [SerializeField] private RollingHandler rollingHandler;
    [SerializeField] private InteractionHandler interactionHandler;

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
    [SerializeField] private string rollAnimation;
    [SerializeField] private string deadAnimation;

    [Header("Player Skills")]
    [SerializeField] public List<Skill> playerSkills;
    [SerializeField] private bool enableWalljump = false;


    // Temp Testing stuff
    [SerializeField] private BaseEffect effect;
    [SerializeField] private float knocbackStrength;
    [SerializeField] private float knocbackTime;

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

    // INTERACTION HANDLER
    [SerializeField] private List<Shrine> interactingShrines;

    // Handle physics in Fixed Update, used for physics, ridgid bodys and collisions
    // Handle inputs in Update. simple timers, non-physics objects (don't include anything 'framerate' dependant)
    protected void Start()
    {
        mv = GetComponent<ComplexMovement>();
        platformHandler = GetComponent<PlatformHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        health = GetComponent<Health>();
        displace = GetComponent<Displacable>();
        stats = GetComponent<Stats>();
        stamina = GetComponent<Stamina>();
        inventory = GetComponentInChildren<Inventory>();
        combatHandler = GetComponent<CombatHandler>();
        inputBuffer = InputBuffer.instance;
        rollingHandler = GetComponent<RollingHandler>();
        interactionHandler = GetComponent<InteractionHandler>();

        // Gets flask
        flask = GetComponentInChildren<Flask>();

        // Set player's stamina
        stamina = GetComponent<Stamina>();
        // regenTimer = stamina.getRegenerationRate(); // Stamina regen timer

        // Set the player's start state
        state = PlayerState.idle;

        interactingShrines = new List<Shrine>();

        animationHandler.changeAnimationState(idleAnimation);
    }

    protected void FixedUpdate()
    {
        switch (state)
        {
            case PlayerState.idle:
                // Do nothing lol
                if (Input.GetKey(KeyCode.UpArrow)) {
                    PlayerCamera.instance.checkPanUp();
                }

                // Handle jump request
                if (inputBuffer.jumpRequest && mv.isGrounded()) {
                    mv.Jump();
                }

                // Handle airborne
                if (!mv.isGrounded()) {
                    PlayerCamera.instance.resetPan();
                    state = PlayerState.airborne;
                    break;
                }

                // Handle displacement
                if (displace.isDisplaced()) {
                    PlayerCamera.instance.resetPan();
                    // Change animation
                    animationHandler.changeAnimationState(staggerAnimation);
                    state = PlayerState.knockedback;
                    break;
                }

                // Handle main-hand attack request
                if (inputBuffer.mainHandAttackRequest && combatHandler.mainHandAttack())
                {
                    PlayerCamera.instance.resetPan();
                    // Animation handled in combat handler
                    state = PlayerState.attacking;
                    break;
                }
                
                // Handle off-hand attack request
                if (inputBuffer.offHandAttackRequest && combatHandler.offhandAttack())
                {
                    PlayerCamera.instance.resetPan();
                    // Animation handled in combat handler
                    state = PlayerState.attacking;
                    break;
                }

                // Handle move request
                if (inputBuffer.moveDirection != 0) {
                    PlayerCamera.instance.resetPan();
                    animationHandler.changeAnimationState(walkAnimation);
                    state = PlayerState.walking;
                    break;
                }

                // Handle crouch request
                if (inputBuffer.crouchRequest) {
                    PlayerCamera.instance.resetPan();
                    animationHandler.changeAnimationState(crouchAnimation);
                    state = PlayerState.crouching;
                    break;
                }
                
                // Handle roll request
                if (inputBuffer.rollRequest && rollingHandler.canRoll()) {
                    PlayerCamera.instance.resetPan();
                    // Roll in your moving direction
                    rollingHandler.startRoll(inputBuffer.moveDirection);

                    // Give 100% dodge chance during roll
                    stats.percentDodgeChance += 1f;

                    // Start animation
                    animationHandler.changeAnimationState(rollAnimation);

                    state = PlayerState.rolling;
                    break;
                }

                break;
            case PlayerState.walking:
                // Move
                mv.Walk(inputBuffer.moveDirection * (1 + stats.movespeedMultiplier));

                // Handle jump request
                if (inputBuffer.jumpRequest && mv.isGrounded()) {
                    mv.Jump();
                }

                // Handle airborne
                if (!mv.isGrounded()) {
                    state = PlayerState.airborne;
                    break;
                }

                // Handle displacement
                if (displace.isDisplaced()) {
                    // Change animation
                    animationHandler.changeAnimationState(staggerAnimation);
                    state = PlayerState.knockedback;
                    break;
                }

                // Handle main-hand attack request
                if (inputBuffer.mainHandAttackRequest && combatHandler.mainHandAttack())
                {
                    // Animation handled in combat handler
                    state = PlayerState.attacking;
                    break;
                }
                
                // Handle off-hand attack request
                if (inputBuffer.offHandAttackRequest && combatHandler.offhandAttack())
                {
                    // Animation handled in combat handler
                    state = PlayerState.attacking;
                    break;
                }

                // Handle idle request
                if (inputBuffer.moveDirection == 0) {
                    animationHandler.changeAnimationState(idleAnimation);
                    state = PlayerState.idle;
                    break;
                }

                // Handle crouch request
                if (inputBuffer.crouchRequest) {
                    animationHandler.changeAnimationState(crouchAnimation);
                    state = PlayerState.crouching;
                    break;
                }
                
                // Handle roll request
                if (inputBuffer.rollRequest && rollingHandler.canRoll()) {
                    // Roll in your moving direction
                    rollingHandler.startRoll(inputBuffer.moveDirection);

                    // Give 100% dodge chance during roll
                    stats.percentDodgeChance += 1f;

                    // Start animation
                    animationHandler.changeAnimationState(rollAnimation);

                    state = PlayerState.rolling;
                    break;
                }

                break;
            case PlayerState.crouching:
                // Pick up items
                interactionHandler.pickUpNearbyItems();

                // Attempt to pan down
                PlayerCamera.instance.checkPanDown();

                // Handle displacement
                if (displace.isDisplaced()) {
                    PlayerCamera.instance.resetPan();
                    // Change animation
                    animationHandler.changeAnimationState(staggerAnimation);
                    state = PlayerState.knockedback;
                    break;
                }

                // Handle airborne
                if (!mv.isGrounded()) {
                    PlayerCamera.instance.resetPan();
                    state = PlayerState.airborne;
                    break;
                }

                // Handle drop down
                if (inputBuffer.dropDownRequest) {
                    PlayerCamera.instance.resetPan();
                    platformHandler.dropFromPlatform();
                    state = PlayerState.airborne;
                    break;
                }

                // Handle crouchwalking request
                if (inputBuffer.crouchRequest && inputBuffer.moveDirection != 0) {
                    PlayerCamera.instance.resetPan();
                    animationHandler.changeAnimationState(crouchWalkAnimation);
                    state = PlayerState.crouchWalking;
                    break;
                }
                
                // Handle un-crouching
                if (!inputBuffer.crouchRequest) {
                    PlayerCamera.instance.resetPan();
                    animationHandler.changeAnimationState(idleAnimation);
                    state = PlayerState.idle;
                    break;
                }

                // Handle roll request
                if (inputBuffer.rollRequest && rollingHandler.canRoll()) {
                    PlayerCamera.instance.resetPan();
                    // Roll in your moving direction
                    rollingHandler.startRoll(inputBuffer.moveDirection);

                    // Give 100% dodge chance during roll
                    stats.percentDodgeChance += 1f;

                    // Start animation
                    animationHandler.changeAnimationState(rollAnimation);

                    state = PlayerState.rolling;
                }

                break;
            case PlayerState.crouchWalking:
                // Handle crouch moving
                mv.crouchWalk(inputBuffer.moveDirection * (1 + stats.movespeedMultiplier));

                // Pick up items
                interactionHandler.pickUpNearbyItems();

                // Handle airborne
                if (!mv.isGrounded()) {
                    state = PlayerState.airborne;
                    break;
                }

                // Handle displacement
                if (displace.isDisplaced()) {
                    // Change animation
                    animationHandler.changeAnimationState(staggerAnimation);
                    state = PlayerState.knockedback;
                    break;
                }

                // Handle drop down
                if (inputBuffer.dropDownRequest) {
                    platformHandler.dropFromPlatform();
                    state = PlayerState.airborne;
                    break;
                }

                // Handle back to idle state
                if (!inputBuffer.crouchRequest && inputBuffer.moveDirection == 0) {
                    animationHandler.changeAnimationState(idleAnimation);
                    state = PlayerState.idle;
                    break;
                }

                // Handle back to walk state
                if (!inputBuffer.crouchRequest && inputBuffer.moveDirection != 0) {
                    animationHandler.changeAnimationState(walkAnimation);
                    state = PlayerState.walking;
                    break;
                }

                // Handle back to crouch state
                if (inputBuffer.crouchRequest && inputBuffer.moveDirection == 0) {
                    animationHandler.changeAnimationState(crouchAnimation);
                    state = PlayerState.crouching;
                    break;
                }

                // Handle roll request
                if (inputBuffer.rollRequest && rollingHandler.canRoll()) {
                    // Roll in your moving direction
                    rollingHandler.startRoll(inputBuffer.moveDirection);

                    // Give 100% dodge chance during roll
                    stats.percentDodgeChance += 1f;

                    // Start animation
                    animationHandler.changeAnimationState(rollAnimation);

                    state = PlayerState.rolling;
                    break;
                }

                break;
            case PlayerState.airborne:
                // Handle animation
                if (mv.checkFalling()) {
                    animationHandler.changeAnimationState(fallingAnimation);
                } else if (mv.checkRising()) {
                    animationHandler.changeAnimationState(risingAnimation);
                }
                
                // Allow movement, while in the air
                mv.Walk(inputBuffer.moveDirection * (1 + stats.movespeedMultiplier));

                // Handle displacement
                if (displace.isDisplaced()) {
                    // Change animation
                    animationHandler.changeAnimationState(staggerAnimation);
                    state = PlayerState.knockedback;
                    break;
                }

                // Handle back to idle
                if (mv.isGrounded() && !platformHandler.IsDropping()) {
                    animationHandler.changeAnimationState(idleAnimation);
                    state = PlayerState.idle;
                    break;
                }

                // Handle wall-sliding if enabled
                if (enableWalljump && mv.onWall() && !mv.isGrounded() && inputBuffer.moveDirection != 0) {
                    // Don't care
                    state = PlayerState.wallsliding;
                    break;
                }

                break;

            // NO WALLSIDING FOR NOW
            # region WallSliding
            case PlayerState.wallsliding: // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
                animationHandler.changeAnimationState(wallslideAnimation);

                mv.wallSlide();
                mv.Walk(inputBuffer.moveDirection * (1 + stats.movespeedMultiplier));

                // Conditions to change state
                if (inputBuffer.jumpRequest && stamina.currentStamina >= mv.getWallSlideStaminaDrain())
                {
                    mv.wallJump();
                    StartCoroutine(wallJumpTimer());
                    state = PlayerState.walljumping;
                }
                else if (mv.isGrounded())
                    state = PlayerState.idle;
                else if (!mv.onWall())
                    state = PlayerState.airborne;
                break;
            case PlayerState.walljumping:

                // Check if the player is falling or rising
                if (mv.checkFalling())
                    animationHandler.changeAnimationState(fallingAnimation);
                if (mv.checkRising())
                    animationHandler.changeAnimationState(risingAnimation);

                break; // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    # endregion

            case PlayerState.rolling:
                // Handle rolling
                rollingHandler.roll();
                
                if (rollingHandler.isDoneRolling()) {
                    // End roll
                    rollingHandler.endRoll();

                    // Stop moving
                    mv.Walk(0);
                    
                    // Return dodge chance
                    stats.percentDodgeChance -= 1f;

                    // Set animation
                    animationHandler.changeAnimationState(idleAnimation);

                    // Change states
                    state = PlayerState.idle;
                }

            break;
            case PlayerState.attacking:
                // Let the combat handler handle the player movement during attack

                if (!inputBuffer.mainHandAttackRequest) {
                    // Attempt to release weapon
                    combatHandler.mainHandRelease(inputBuffer.mainAttackTime);
                }

                if (!inputBuffer.offHandAttackRequest) {
                    // Attempt to release weapon
                    combatHandler.offHandRelease(inputBuffer.offAttackTime);
                }

                // Handle back to idle
                if (combatHandler.isDoneAttacking()) {
                    // Stop moving
                    mv.Walk(0);

                    // Reset attack requests
                    inputBuffer.resetAttackRequests();

                    // Change animation
                    animationHandler.changeAnimationState(idleAnimation);
                    state = PlayerState.idle;
                    break;
                }

                // Handle roll cancel
                if (inputBuffer.rollRequest && combatHandler.weaponsAreRecovering() && rollingHandler.canRoll()) {
                    // Cancel the current animation
                    combatHandler.cancelAllAttacks();

                    // Roll in your moving direction
                    rollingHandler.startRoll(inputBuffer.moveDirection);

                    // Give 100% dodge chance during roll
                    stats.percentDodgeChance += 1f;

                    // Start animation
                    animationHandler.changeAnimationState(rollAnimation);

                    state = PlayerState.rolling;
                    break;
                }

                // Handle attack requests for handling combos
                if (inputBuffer.mainHandAttackRequest && combatHandler.mainHandAttack())
                {
                    // Animation handled in combat handler
                    state = PlayerState.attacking;
                    break;
                }
                
                if (inputBuffer.offHandAttackRequest && combatHandler.offhandAttack())
                {
                    // Animation handled in combat handler
                    state = PlayerState.attacking;
                    break;
                }

                // Handle new displacements
                if (displace.isDisplaced()) {
                    // Cancel any attack animations
                    combatHandler.cancelAllAttacks();
                    // Change animation
                    animationHandler.changeAnimationState(staggerAnimation);
                    state = PlayerState.knockedback;
                    break;
                }

                break;
            case PlayerState.interacting:
                // TODO

                break;
            case PlayerState.inMenu:
                // Wait for input

                // Conditions to change state
                if (!inputBuffer.menuToggleRequest)
                {
                    menu.menuEnabled = false;

                    animationHandler.changeAnimationState(idleAnimation);
                    state = PlayerState.idle;
                    break;
                }
                break;
            case PlayerState.knockedback:
                // Perfom any active displacements
                displace.performDisplacement();

                // Handle new displacements
                if (displace.isDisplaced()) {
                    // Change animation
                    animationHandler.changeAnimationState(staggerAnimation);
                    state = PlayerState.knockedback;
                    break;
                }

                // If displacement is done, then go back to idle
                if (!displace.isDisplaced()) {
                    animationHandler.changeAnimationState(idleAnimation);
                    state = PlayerState.idle;
                    break;
                }    

                break;
            case PlayerState.dead:
                // Do nothing, the game is over lol
                break;
        }
    }

    protected void Update()
    {
        // Better jump?
        mv.improvedJumpHandling(KeyCode.Space);

        if (Input.GetKeyDown(KeyCode.G))  // used for testing
        {
            // Nothin
            // displace.triggerKnockback(knocbackStrength, knocbackTime, transform.position + Vector3.right);
            stamina.useStamina(10);
        }
        if (Input.GetKeyDown(KeyCode.H))  // used for testing
        {
            //displace.triggerKnockback(600, 0.25f, transform.position + Vector3.right);
            // Nothin
            GetComponent<EffectableEntity>().addEffect(effect.InitializeEffect(gameObject));
        }

        // Check for death
        if (health.isEmpty() && state != PlayerState.dead) {
            // Stop
            mv.Walk(0);

            // Set animation
            animationHandler.changeAnimationState(deadAnimation);

            // Change state
            state = PlayerState.dead;
        }

        if (playerIsFree())
        {

            // Handle opening the menu
            if (inputBuffer.menuToggleRequest)
            {
                menu.menuEnabled = true;

                // Stop moving
                mv.Walk(0);

                // Set animation
                animationHandler.changeAnimationState(idleAnimation);

                // Change state
                state = PlayerState.inMenu;
            }

            // Use flask
            if (inputBuffer.useFlaskRequest) {
                // If you are at full health, do nothing
                if(health.isFull()) {
                    PopUpTextManager.instance.createVerticalPopup("Already at full HP.", Color.gray, transform.position);
                }
                else {
                    useFlask();
                }
                // Reset request
                inputBuffer.resetFlaskRequest();
            }
                
            // Interacting with surroundings
            if (inputBuffer.interactPressRequest) {
                interactionHandler.interactWithNearbyObjects();
            }
            else if (inputBuffer.interactReleaseRequest) {
                interactionHandler.releaseNearbyObjects();
            }
            
        }
    }

    public bool isIdle() {
        return state == PlayerState.idle;
    }

    private bool playerIsFree()
    {
        return state == PlayerState.idle || state == PlayerState.walking || state == PlayerState.airborne || state == PlayerState.crouching || state == PlayerState.crouchWalking;
    }

    private IEnumerator wallJumpTimer()
    {
        state = PlayerState.walljumping;
        yield return new WaitForSeconds(mv.wallJumpTime);
        state = PlayerState.airborne;
    }

    private void useFlask()
    {
        if (flask.use()) // Uses flask and checks result
            heal((int)(health.getMaxHP() * flask.getHealPercentage()));
        else
            PopUpTextManager.instance.createVerticalPopup("Flask is empty.", Color.gray, transform.position);
    }

    public void heal(int amount)
    {
        PopUpTextManager.instance.createVerticalPopup("+" + amount + " HP.", Color.green, transform.position);
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
        PopUpTextManager.instance.createVerticalPopup("You have learned the skill: " + skill.name, Color.white, transform.position);
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
}