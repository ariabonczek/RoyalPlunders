using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public Transform pauseMenu;
    public Transform mainCamera;

    public void HandleMenu()
    {
        if (!pauseMenu.gameObject.activeInHierarchy)
        {
            pauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0;
            mainCamera.GetComponent<ThirdPersonCamera>().enabled = false;
        }
        else
        {
            pauseMenu.gameObject.SetActive(false);
            Time.timeScale = 1;
            mainCamera.GetComponent<ThirdPersonCamera>().enabled = true;
        }
    }

    public void ResumeGame()
    {
        pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1;
        mainCamera.GetComponent<ThirdPersonCamera>().enabled = true;
    }

    public void RestartLevel()
    {
        int scene = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
