using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public static bool isPaused = false;

    public GameObject pauseScreenUI;

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isPaused) {
                Resume();
            }
            else {
                Pause();
            }
        }
    }

    private void Resume() {
        pauseScreenUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    private void Pause() {
        pauseScreenUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void restart() {
        Time.timeScale = 1f;
        LevelManager.instance.reloadLevel();
        Destroy(GameManager.instance.GetPlayer().gameObject);
    }

    public void loadMainMenu() {
        Time.timeScale = 1f;
        LevelManager.instance.loadMainMenu();
    }

    public void quitGame() {
        Debug.Log("Quit game");
        Application.Quit();
    }
    
}
