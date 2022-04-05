using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlaskUI : MonoBehaviour
{

    [SerializeField] private Flask flask;
    [SerializeField] private Image flaskImage;
    [SerializeField] private Sprite fullFlask;
    [SerializeField] private Sprite emptyFlask;

    // Start is called before the first frame update
    private void Start()
    {
        flask = GameObject.Find("Player").GetComponentInChildren<Flask>(); // Gets the flask object from player
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (flask.isEmpty())
        {
            flaskImage.sprite = emptyFlask;
        }
        else
        {
            flaskImage.sprite = fullFlask;
        }
    }
}
