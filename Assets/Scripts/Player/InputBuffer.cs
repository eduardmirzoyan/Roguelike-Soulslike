using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer : MonoBehaviour
{
    public static InputBuffer instance;
    [SerializeField] private Keybindings keybindings;
    [SerializeField] private List<ActionItem> inputBuffer = new List<ActionItem>();  //The input buffer

    [SerializeField] private bool enableBuffering;
    [SerializeField] private int bufferRatio = 2;

    public float moveDirection { get; private set; }
    public bool jumpRequest { get; private set; }
    public bool dropDownRequest { get; private set; }
    public bool crouchRequest { get; private set; }
    public bool menuToggleRequest { get; private set; }
    public bool mainHandAttackRequest { get; private set; }
    public bool offHandAttackRequest { get; private set; }
    public bool rollRequest { get; private set; }
    public bool useFlaskRequest { get; private set; }
    public bool interactPressRequest { get; private set; }
    public bool interactReleaseRequest { get; private set; }
    public bool helpRequest { get; private set; }
    public bool pauseMenuRequest { get; private set; }

    private float rollTimer;
    private float jumpInputTimer;
    private float dropDownTimer;
    private float flaskInputTimer;

    public float mainAttackTime;
    public float offAttackTime;

    private void Awake()
    {
        if(GameManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        
        keybindings = GetComponent<Keybindings>();
    }

    private void Update()
    {
        checkInput();
        
        if (enableBuffering)
        {
            tryBufferedAction();
        }
    }

    private void FixedUpdate() {
        if (jumpInputTimer > 0) {
            jumpInputTimer -= Time.deltaTime;
        }
        else {
            jumpRequest = false;
        }

        if (rollTimer > 0) {
            rollTimer -= Time.deltaTime;
        }
        else {
            rollRequest = false;
        }

        if (dropDownTimer > 0) {
            dropDownTimer -= Time.deltaTime;
        }
        else {
            dropDownRequest = false;
        }
    }

    //Check inputs here, and add them to the input buffer
    private void checkInput()
    {
        // Get player movement input
        moveDirection = Input.GetAxis("Horizontal");

        // Check input for jump
        if (Input.GetKey(keybindings.crouchKey) && Input.GetKeyDown(keybindings.jumpKey)) {
            dropDownRequest = true;
            dropDownTimer = bufferRatio * Time.deltaTime;
        }
        else if (Input.GetKeyDown(keybindings.jumpKey)) { // Check input for jump
            jumpRequest = true;
            jumpInputTimer = bufferRatio * Time.deltaTime;
        }
            
        // Check input for menu
        if (Input.GetKeyDown(keybindings.inventoryKey))
            menuToggleRequest = !menuToggleRequest;
            

        // Check input for crouch
        if (Input.GetKey(keybindings.crouchKey))
            crouchRequest = true;
        else
            crouchRequest = false;

        // Check input for interacting
        if (Input.GetKeyDown(keybindings.interactKey) || Input.GetKeyDown(keybindings.interactKey2))
            interactPressRequest = true;
        else
            interactPressRequest = false;

        if (Input.GetKeyUp(keybindings.interactKey) || Input.GetKeyUp(keybindings.interactKey2))
            interactReleaseRequest = true;
        else
            interactReleaseRequest = false;

        // Check input for Flask usage
        if (Input.GetKeyDown(keybindings.flaskKey)) {
            useFlaskRequest = true;
        }

        if (useFlaskRequest && Input.GetKeyUp(keybindings.flaskKey)) {
            useFlaskRequest = false;
        }

        // Buffer Attacks and abilities
        if (Input.GetKeyDown(keybindings.rollKey)) {
            rollRequest = true;
            rollTimer = bufferRatio * Time.deltaTime;
        }

        // Check for main hand attack
        if (Input.GetKeyDown(keybindings.mainhandAttackKey)) {
            mainHandAttackRequest = true;
            mainAttackTime = Time.time;
        }

        // Check for main hand release
        if (mainHandAttackRequest && Input.GetKeyUp(keybindings.mainhandAttackKey)) {
            mainHandAttackRequest = false;
            mainAttackTime = Time.time - mainAttackTime;
        }

        // Check for off hand attack
        if (!offHandAttackRequest && Input.GetKeyDown(keybindings.offhandAttackKey)) {
            offHandAttackRequest = true;
            offAttackTime = Time.time;
        }

        // Check for off hand release
        if (offHandAttackRequest && Input.GetKeyUp(keybindings.offhandAttackKey)) {
            offHandAttackRequest = false;
            offAttackTime = Time.time - offAttackTime;
        }

        // Check for help screen
        if (Input.GetKeyDown(keybindings.helpKey)) {
            helpRequest = true;
        }
        else {
            helpRequest = false;
        }

        // On press set "attack" to true
        // ANd set timer to 0

        // On release, set attack to false and timer to time between press

    }

    public void resetAttackRequests() {
        mainHandAttackRequest = false;
        offHandAttackRequest = false;
    }

    public void resetFlaskRequest() {
        useFlaskRequest = false;
    }

    //Call when we want to process the inputBuffer
    private void tryBufferedAction()
    {
        // If there are any queued actions...
        if (inputBuffer.Count > 0)
        {
            foreach (ActionItem ai in inputBuffer.ToArray())  //Using ToArray so we iterate a copy of the list rather than the actual list, since we will be modifying the list in the loop
            {
                inputBuffer.Remove(ai);  //Remove it from the buffer
                if (ai.CheckIfExpired()) // Check if action has expired
                {
                    //Means the action is still within the allowed time, so we do the action and then break from processing more of the buffer
                    doAction(ai);
                    break;  //We probably only want to do 1 action at a time, so we just break here and don't process the rest of the inputBuffer
                }
            }
        }
    }

    private void doAction(ActionItem ai)
    {
        //code to jump, punch, kick, etc
        //actionAllowed = false;  //Every action probably has some kind of wait period until the next action is allowed, so we set this to false here.
        //Some code somewhere else needs to be written to set it back to true

        // Reset all requests
        resetAttackRequests();
    }
}
public class ActionItem
{
    public enum InputAction { LightAttack, HeavyAttack, SignatureAttack, UtilityAttack };
    public InputAction Action;
    public float Timestamp;

    public static float TimeBeforeActionsExpire = 0.25f; // Buffer time

    //Constructor
    public ActionItem(InputAction ia, float stamp)
    {
        Action = ia;
        Timestamp = stamp;
    }

    //returns true if this action hasn't expired due to the timestamp
    public bool CheckIfExpired()
    {
        bool returnValue = false;
        if (Timestamp + TimeBeforeActionsExpire >= Time.time)
        {
            returnValue = true;
        }
        return returnValue;
    }
}