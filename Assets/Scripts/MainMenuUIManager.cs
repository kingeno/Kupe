using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUIManager : MonoBehaviour {

    public GameObject mainMenu;
    public GameObject levelHub;
    public GameObject controlsScheme;
    public GameObject levelContainer1;
    public GameObject levelContainer2;
    public GameObject levelContainer3;
    public GameObject levelContainer4;

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

    public void LevelHubArrowButton(string direction)
    {
        if(direction == "right")
        {
            if (levelContainer1.activeSelf)
            {
                levelContainer1.SetActive(false);
                levelContainer2.SetActive(true);
            }
            else if (levelContainer2.activeSelf)
            {
                levelContainer2.SetActive(false);
                levelContainer3.SetActive(true);
            }
            else if (levelContainer3.activeSelf)
            {
                levelContainer3.SetActive(false);
                levelContainer4.SetActive(true);
            }
        }
        else if (direction == "left")
        {
            if (levelContainer4.activeSelf)
            {
                levelContainer4.SetActive(false);
                levelContainer3.SetActive(true);
            }
            else if (levelContainer3.activeSelf)
            {
                levelContainer3.SetActive(false);
                levelContainer2.SetActive(true);
            }
            else if (levelContainer2.activeSelf)
            {
                levelContainer2.SetActive(false);
                levelContainer1.SetActive(true);
            }
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
