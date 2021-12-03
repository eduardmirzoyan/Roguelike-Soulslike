 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Animator))]
public class LevelManager : MonoBehaviour
{
    [SerializeField] private float transitionTime;

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

        DontDestroyOnLoad(gameObject); 
    }

    // Loads next level
    public void loadNextLevel()
    {
        StartCoroutine(delayedLoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private IEnumerator delayedLoadLevel(int levelIndex)
    {
        var animator = GameObject.Find("Transition Canvas").GetComponentInChildren<Animator>();
        animator.SetTrigger("close scene"); // Set the level change animation

        // OPTIMIZE THIS LATER!
        GameObject.Find("Player").GetComponentInChildren<Player>().setBossHealthBar(null); // Disable boss health bar...

        // Give animation time to run
        yield return new WaitForSeconds(transitionTime);

        // Load next scene
        SceneManager.LoadScene(levelIndex);
    }
}
