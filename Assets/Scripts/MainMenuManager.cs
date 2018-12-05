using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUIManager : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject levelHub;
    public GameObject controlsScheme;

    public void DisplayLevelHub()
    {
        mainMenu.SetActive(false);
        levelHub.SetActive(true);
    }

    public void DisplayControlsScheme()
    {
        mainMenu.SetActive(false);
        controlsScheme.SetActive(true);
    }

    public void BackButton()
    {
        if (controlsScheme.activeSelf)
        {
            controlsScheme.SetActive(false);
            mainMenu.SetActive(true);
        }
        else if(levelHub.activeSelf)
        {
            levelHub.SetActive(false);
            mainMenu.SetActive(true);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
