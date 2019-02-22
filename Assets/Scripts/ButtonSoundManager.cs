using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSoundManager : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button _button;

    [Header("Sounds names")]
    public string onPointerEnter;
    public string onPointerClick;

    private void Awake()
    {
        _button = gameObject.GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(onPointerEnter);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(onPointerClick);
    }

    public void PlaySound(string soundName)
    {
        if (_button.interactable)
        {
            AudioManager.instance.Play(soundName);
        }
    }
}
