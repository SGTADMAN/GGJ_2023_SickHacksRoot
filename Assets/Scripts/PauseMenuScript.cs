using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuScript : MonoBehaviour
{
    public bool paused = false;
    public GameObject pauseMenus, firstPauseMenuElement;
    public Texture2D normalCursor;
    PlayerInput playerInput;
    public AudioMixerSnapshot pausedSnapshot;
    EventSystem eventSystem;

    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
        eventSystem = FindObjectOfType<EventSystem>();
        Cursor.visible = false;
    }
    public void PauseAction(InputAction.CallbackContext context)
    {
        if (paused == true)
        {
            ResumeGame();            
        }
        else if (paused == false)
        {
            pauseMenus.SetActive(true);
            paused = true;
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            if (!playerInput.devices[0].GetType().ToString().Contains("Keyboard"))
                Cursor.visible = false;
            else
                Cursor.visible = true;
            eventSystem.SetSelectedGameObject(firstPauseMenuElement);

        }
    }

    public void ResumeGame()
    {
        pauseMenus.SetActive(false);
        paused = false;
        Time.timeScale = 1;
        eventSystem.SetSelectedGameObject(null);
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ReturnToMain()
    {
        SceneManager.LoadScene("MainMenu");
    }
    
}
