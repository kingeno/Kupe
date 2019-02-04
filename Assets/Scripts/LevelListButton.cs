using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelListButton : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _text;
    private LevelLoader _levelLoader;
    [SerializeField] private int levelNumber;

    private void Start()
    {
        if (!_levelLoader)
            _levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();
    }

    public void SetLevelName(string levelName)
    {
        _text.text = levelName;
    }

    public void SetLevelNumber(int number)
    {
        levelNumber = number;

    }

    public void OnButtonClick(int levelIndex)
    {
        levelIndex = levelNumber;
        _levelLoader.loadSpecificLevel(levelIndex);
    }
}
