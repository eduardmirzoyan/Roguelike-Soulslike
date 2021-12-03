using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputBuffer : MonoBehaviour
{
    [SerializeField] private Keybindings keybindings;
    [SerializeField] private List<ActionItem> inputBuffer = new List<ActionItem>();  //The input buffer
    [SerializeField] private bool actionAllowed;  //set to true whenever we want to process actions from the input buffer, set to false when an action has to wait in the buffer
                                                  //Notice I don't have any code here which sets actionAllowed to true, because that probably depends on states like in the middle of throwing a punch animation, or checking if a jump has finished, or whatever

    public bool jumpRequest { get; private set; }
    public bool crouchRequest { get; private set; }
    public float moveDirection { get; private set; }
    public bool menuToggleRequest { get; private set; }
    public bool lightAttackRequest { get; private set; }
    public bool heavyAttackRequest { get; private set; }
    public bool signatureAbilityRequest { get; private set; }
    public bool utilityAbilityRequest { get; private set; }

    public bool interactRequest { get; private set; }

    public bool useFlaskRequest { get; private set; }

    // Temp
    public bool sprintRequest { get; private set; }

    private void Awake()
    {
        keybindings = GetComponent<Keybindings>();
        actionAllowed = true;
    }

    private void Start()
    {
        GameEvents.current.onActionFinish += enableActionAllowed;
        GameEvents.current.onActionStart += disableActionAllowed;
    }

    public void Update()
    {
        checkInput();
        if (actionAllowed)
        {
            tryBufferedAction();
        }
    }

    //Check inputs here, and add them to the input buffer
    private void checkInput()
    {
        // Get player movement input
        moveDirection = Input.GetAxis("Horizontal");

        // Check input for jump
        if (Input.GetKeyDown(keybindings.jumpKey))
            jumpRequest = true;
        else
            jumpRequest = false;

        // Check input for crouch
        if (Input.GetKey(keybindings.crouchKey))
            crouchRequest = true;
        else
            crouchRequest = false;

        // Check input for menu
        if (Input.GetKeyDown(keybindings.inventoryKey))
            menuToggleRequest = !menuToggleRequest;

        // Check input for interacting
        if (Input.GetKeyDown(keybindings.interactKey) || Input.GetKeyDown(keybindings.interactKey2))
            interactRequest = true;
        else
            interactRequest = false;

        // Check input for Sprinting
        if (Input.GetKey(keybindings.sprintKey))
            sprintRequest = true;
        else
            sprintRequest = false;

        // Check input for Flask usage
        useFlaskRequest = Input.GetKeyDown(keybindings.flaskKey) ? true : false;

        if (Input.GetKeyDown(KeyCode.H))
            actionAllowed = false;

        // Buffer Attacks and abilities

        if (Input.GetKey(KeyCode.UpArrow) && Input.GetKeyDown(keybindings.primaryAttackKey))
        {
            inputBuffer.Add(new ActionItem(ActionItem.InputAction.HeavyAttack, Time.time)); // Adds Jump action into buffer at time
        }
        else if (Input.GetKeyDown(keybindings.primaryAttackKey))
        {
            inputBuffer.Add(new ActionItem(ActionItem.InputAction.LightAttack, Time.time)); // Adds Jump action into buffer at time
        }
        else if (Input.GetKeyDown(keybindings.signatureAbilitKey))
        {
            inputBuffer.Add(new ActionItem(ActionItem.InputAction.SignatureAttack, Time.time)); // Adds Jump action into buffer at time
        }
        else if (Input.GetKey(keybindings.utilityAbiltyKey))
        {
            inputBuffer.Add(new ActionItem(ActionItem.InputAction.UtilityAttack, Time.time)); // Adds Jump action into buffer at time
        }
        else
        {
            lightAttackRequest = false;
            heavyAttackRequest = false;
            signatureAbilityRequest = false;
            utilityAbilityRequest = false;
        }
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


        // Then toggle the chosen action
        switch (ai.Action)
        {
            case ActionItem.InputAction.LightAttack:
                lightAttackRequest = true;
                break;
            case ActionItem.InputAction.HeavyAttack:
                heavyAttackRequest = true;
                break;
            case ActionItem.InputAction.SignatureAttack:
                signatureAbilityRequest = true;
                break;
            case ActionItem.InputAction.UtilityAttack:
                utilityAbilityRequest = true;
                break;
        }
    }

    private void enableActionAllowed() => actionAllowed = true;

    private void disableActionAllowed() => actionAllowed = false;
}
public class ActionItem
{
    public enum InputAction { LightAttack, HeavyAttack, SignatureAttack, UtilityAttack, Jump, Crouch };
    public InputAction Action;
    public float Timestamp;

    public static float TimeBeforeActionsExpire = 0.1f; // Buffer time

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