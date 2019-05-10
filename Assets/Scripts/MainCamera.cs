using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class MainCamera : MonoBehaviour
{
    
    private GameManager gameManager;
    private InGameUIManager _inGameUIManager;
    private Camera _camera;
    [SerializeField] private GameObject _CMFreeLookCam;

    [Header("Background Colors")]
    public GameObject background;

    private Vector3 startPos;
    private Quaternion startRotation;

    private bool canColorSwap;
    public static bool isFreeLookActive;

    [Header("Transition Time")]
    public float colorTransitionTime;
    public float levelCompletedColorTransitionTime;
    [HideInInspector] public bool levelCompletedColorSwap;

    public Vignette m_vignette;
    public DepthOfField m_depthOfField;

    public PostProcessVolume volume;
    public float vignetteValue;
    public bool depthOfField_IsActive;

    private void Awake()
    {
        if (!background.activeSelf)
            background.SetActive(true);
    }

    void Start()
    {
        isFreeLookActive = false;
        startPos = transform.position;
        startRotation = transform.rotation;

        if (!_CMFreeLookCam)
            _CMFreeLookCam = GameObject.FindGameObjectWithTag("CMFreeLookCamera");

        if (!gameManager)
            gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        if (!_inGameUIManager)
            _inGameUIManager = GameObject.FindGameObjectWithTag("InGameUIManager").GetComponent<InGameUIManager>();

        if (!_CMFreeLookCam)
            _CMFreeLookCam = GameObject.FindGameObjectWithTag("CMFreeLookCamera");

        _camera = GetComponent<Camera>();

        volume = gameObject.GetComponent<PostProcessVolume>();

        volume.profile.TryGetSettings(out m_vignette);
        volume.profile.TryGetSettings(out m_depthOfField);

        //m_vignette = GetComponent<Vignette>();
        //m_depthOfField = GetComponent<DepthOfField>();


    }

    private void Update()
    {
        if (!isFreeLookActive)
        {
            if ((Input.GetKey(KeyCode.C) || Input.GetMouseButton(2)))
                FreeLook();
        }
        else if (isFreeLookActive)
        {
            if (!Input.GetKey(KeyCode.C) && !Input.GetMouseButton(2))
                FreeLook();
        }

        if (GameManager.gameIsPaused)
        {
            m_vignette.intensity.value = 0.4f;
            m_depthOfField.enabled.value = true;
        }
        else if (!GameManager.gameIsPaused)
        {
            m_vignette.intensity.value = 0.3f;
            m_depthOfField.enabled.value = false;
        }
    }

    public void FreeLook()
    {
        if (_CMFreeLookCam && !GameManager.gameIsPaused)
        {
            if (!isFreeLookActive)
            {
                GameManager.playerCanModifyBoard = false;
                isFreeLookActive = true;
            }
            else if (isFreeLookActive)
            {
                if (GameManager.simulationHasBeenLaunched || GameManager.simulationIsRunning)
                    GameManager.playerCanModifyBoard = false;
                else
                    GameManager.playerCanModifyBoard = true;
                isFreeLookActive = false;
            }

            if (isFreeLookActive)
                _CMFreeLookCam.SetActive(true);
            else if (!isFreeLookActive)
                _CMFreeLookCam.SetActive(false);
        }
    }

    public void SetToStartPos()
    {
        transform.position = startPos;
        transform.rotation = startRotation;

    }
}
