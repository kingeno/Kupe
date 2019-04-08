using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelListController : MonoBehaviour
{
    [SerializeField]
    private GameObject buttonTemplate;

    private LevelLoader _levelLoader;

    private List<GameObject> buttons;

    private void Start()
    {
        if (!_levelLoader)
            _levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();

        GenerateLevelList();
    }

    public void GenerateLevelList()
    {
        buttons = new List<GameObject>();

        if (buttons.Count > 0)
        {
            foreach (GameObject button in buttons)
            {
                Destroy(button.gameObject);
            }

            buttons.Clear();
        }

        for (int i = 1; i <= SceneManager.sceneCountInBuildSettings - 1; i++)
        {
            GameObject button = Instantiate(buttonTemplate) as GameObject;
            button.SetActive(true);

            button.name = "level_" + i;
            button.GetComponent<LevelListButton>().SetLevelName("Level " + i);
            button.GetComponent<LevelListButton>().SetLevelNumber(i);
            button.GetComponent<LevelListButton>().SetLevelImage(i);

            button.transform.SetParent(buttonTemplate.transform.parent, false);
        }
    }

    public void OnButtonClick(int levelIndex)
    {
        _levelLoader.LoadSpecificLevel(levelIndex);
    }
}
