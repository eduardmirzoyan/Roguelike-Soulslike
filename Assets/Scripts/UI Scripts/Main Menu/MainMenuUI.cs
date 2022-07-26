using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame() {
        // Load level 1
        LevelManager.instance.loadNextLevel(new Vector3(7, 6, 0));
    }

    public void QuitGame() {
        // Quit game
        Debug.Log("QUIT GAME!");
        Application.Quit();
    }
}
