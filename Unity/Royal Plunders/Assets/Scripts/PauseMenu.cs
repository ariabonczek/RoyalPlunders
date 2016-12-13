using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    public bool menuOpen = false; // public facing menu state

    public Transform pauseMenu; // the pause menu
    public Transform sceneSelectMenu; // the scene menu
    public Transform mainCamera; // the main camera

    public GameObject pausefirstItem; // first item of focus in the pause menu
    public GameObject sceneFirstItem; // first item of focus in the scene menu

    // invoked by the player pausing the game
    public void HandleMenu()
    {
        if (!pauseMenu.gameObject.activeInHierarchy && !menuOpen) // if the menu is not open
        {
            pauseMenu.gameObject.SetActive(true); // open the pause menu
            Time.timeScale = 0; // pause the game
            mainCamera.GetComponent<ThirdPersonCamera>().enabled = false; // disable the camera controls

            GameObject myEventSystem = GameObject.Find("EventSystem"); // get the event system
            myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(pausefirstItem); // set the focus to the first pause item
            menuOpen = true; // the menu is now open for business
        }
        else // the menu needs to close now
        {
            pauseMenu.gameObject.SetActive(false); // close the pause menu
            sceneSelectMenu.gameObject.SetActive(false); // close the scene select menu
            Time.timeScale = 1; // resumt time
            mainCamera.GetComponent<ThirdPersonCamera>().enabled = true; // re-enable the third person camera
            menuOpen = false; // the menu is not open
        }
    }

    // resume the game
    public void ResumeGame()
    {
        pauseMenu.gameObject.SetActive(false); // hide the pause menu
        Time.timeScale = 1; // resume time
        mainCamera.GetComponent<ThirdPersonCamera>().enabled = true; // re-enable the third person camera
    }

    // enter level select
    public void SelectScene()
    {
        pauseMenu.gameObject.SetActive(false); // hide the pause menu
        sceneSelectMenu.gameObject.SetActive(true); // show the level select menu

        GameObject myEventSystem = GameObject.Find("EventSystem"); // get the event system
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(sceneFirstItem); // set the focus to the first scene item
    }

    // loads a level given a name
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    // leave level select and enter the pause menu
    public void CloseSceneSelect()
    {
        GameObject myEventSystem = GameObject.Find("EventSystem"); // get the event system
        myEventSystem.GetComponent<UnityEngine.EventSystems.EventSystem>().SetSelectedGameObject(pausefirstItem); // set the focus to the first pause item

        pauseMenu.gameObject.SetActive(true); // show the pause menu
        sceneSelectMenu.gameObject.SetActive(false); // hide the level select menu
    }

    // restarts the current level anew
    public void RestartLevel()
    {
        int scene = SceneManager.GetActiveScene().buildIndex; // get the current scene
        SceneManager.LoadScene(scene, LoadSceneMode.Single); // load it anew
        Time.timeScale = 1; // resume time
    }

    // quits the game
    public void QuitGame()
    {
        Application.Quit(); // close the game
    }
}