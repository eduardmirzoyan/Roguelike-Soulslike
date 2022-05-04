using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpHandler : MonoBehaviour
{
    [SerializeField] private InputBuffer inputBuffer;
    [SerializeField] private Animator helpScreenAnimator;
    [SerializeField] private bool helpEnabled;

    // Start is called before the first frame update
    private void Start()
    {
        inputBuffer = GetComponent<InputBuffer>();
        helpEnabled = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (inputBuffer.helpRequest) {
            if (!helpEnabled) {
                helpScreenAnimator.Play("Enabled");
                helpEnabled = true;
            }
            else {
                helpScreenAnimator.Play("Disabled");
                helpEnabled = false;
            }
                
        }
    }
}
