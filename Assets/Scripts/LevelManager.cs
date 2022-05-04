 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class LevelManager : MonoBehaviour
{
    [SerializeField] private float transitionTime;
    [SerializeField] private Animator animator;
    public static LevelManager instance; // Accessible by every class at any point

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this; // Singleton

        // animator = GameObject.Find("Transition Canvas").GetComponentInChildren<Animator>();

        DontDestroyOnLoad(gameObject); 
    }

    // Loads next level
    public void loadNextLevel()
    {
        StartCoroutine(delayedLoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void loadMainMenu()
    {
        StartCoroutine(delayedLoadLevel(0));
    }

    public void reloadLevel() 
    {
        StartCoroutine(delayedLoadLevel(SceneManager.GetActiveScene().buildIndex));
    }

    private IEnumerator delayedLoadLevel(int levelIndex)
    {
        animator = GameObject.Find("Transition Canvas").GetComponentInChildren<Animator>();
        animator.SetTrigger("close scene"); // Set the level change animation

        // Give animation time to run
        yield return new WaitForSeconds(transitionTime);

        // Load next scene
        SceneManager.LoadScene(levelIndex);
        
    }
}
