using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadSceneAsync("MainScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenBlog()
    {
        Application.OpenURL("https://adamluttonblog.co.uk/");
    }
    public void OpenKoFi()
    {
        Application.OpenURL("https://ko-fi.com/sgtadman");
    }
    public void OpenItch()
    {
        Application.OpenURL("https://sgtadman.itch.io/");
    }
}
