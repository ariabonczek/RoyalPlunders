using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public bool menuOpen = false;

    public Transform pauseMenu;
    public Transform sceneSelectMenu;
    public Transform mainCamera;

    public GameObject pausefirstItem;
    public GameObject sceneFirstItem;

    public void HandleMenu()
    {
        if (!pauseMenu.gameObject.activeInHierarchy && !menuOpen)
        {
            pauseMenu.gameObject.SetActive(true);
            Time.timeScale = 0;
            mainCamera.GetComponent<ThirdPersonCamera>().enabled = false;

            GameObject myEventSystem = GameObject.Find("EventSystem");
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(pausefirstItem);
            menuOpen = true;
        }
        else
        {
            pauseMenu.gameObject.SetActive(false);
            sceneSelectMenu.gameObject.SetActive(false);
            Time.timeScale = 1;
            mainCamera.GetComponent<ThirdPersonCamera>().enabled = true;
            menuOpen = false;
        }
    }

    public void ResumeGame()
    {
        pauseMenu.gameObject.SetActive(false);
        Time.timeScale = 1;
        mainCamera.GetComponent<ThirdPersonCamera>().enabled = true;
    }

    public void SelectScene()
    {
        pauseMenu.gameObject.SetActive(false);
        sceneSelectMenu.gameObject.SetActive(true);

        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(sceneFirstItem);
    }

    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void CloseSceneSelect()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem");
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(pausefirstItem);

        pauseMenu.gameObject.SetActive(true);
        sceneSelectMenu.gameObject.SetActive(false);
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