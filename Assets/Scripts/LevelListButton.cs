using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelListButton : MonoBehaviour
{

    private LevelLoader _levelLoader;

    public Image outlinLevelImage;

    [SerializeField] private int _levelNumber;
    [SerializeField] private TextMeshProUGUI _levelName;
    [SerializeField] private Image levelImage = null;

    private void Start()
    {
        if (!_levelLoader)
            _levelLoader = GameObject.FindGameObjectWithTag("LevelLoader").GetComponent<LevelLoader>();

        if (!outlinLevelImage)
            outlinLevelImage = GetComponent<Image>();
    }

    public void SetLevelName(string levelName)
    {
        _levelName.text = levelName;
    }

    public void SetLevelNumber(int number)
    {
        _levelNumber = number;

    }

    public void SetLevelImage(int number)
    {
        if (levelImage)
        {
            levelImage.sprite = Resources.Load<Sprite>("LevelScreenshots/" + number);
        }
        if (outlinLevelImage)
        {
            outlinLevelImage = GetComponent<Image>();
            outlinLevelImage.sprite = Resources.Load<Sprite>("LevelScreenshots/Outlines/" + number);
        }
    }

    public void OnButtonClick(int levelIndex)
    {
        levelIndex = _levelNumber;
        _levelLoader.loadSpecificLevel(levelIndex);
    }
}
