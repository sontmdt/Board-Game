using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePause : MonoBehaviour, IMenu
{
    private UIMainManager m_uiManager;
    private SoundController m_soundController;

    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnExit;

    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider effectVolume;
    private void Awake()
    {
        m_soundController = FindObjectOfType<SoundController>();

        btnPlay.onClick.AddListener(OnClickPlay);
        btnRestart.onClick.AddListener(OnClickRestart);
        btnExit.onClick.AddListener(OnClickExit);

        musicVolume.value = m_soundController.musicSource.volume;
        effectVolume.value = m_soundController.effectSource.volume;
        musicVolume.onValueChanged.AddListener(m_soundController.OnVolumeMusicChanged);
        effectVolume.onValueChanged.AddListener(m_soundController.OnVolumeSoundChanged);
    }
    private void OnDestroy()
    {
        btnPlay.onClick.RemoveAllListeners();
        btnRestart.onClick.RemoveAllListeners();
        btnExit.onClick.RemoveAllListeners();
    }
    public void Setup(UIMainManager uiManager)
    {
        m_uiManager = uiManager;
    }
    public void Show()
    {
        this.gameObject.SetActive(true);
    }
    public void Hide()
    {
        this.gameObject.SetActive(false);
    }
    private void OnClickPlay()
    {
        m_uiManager.ShowGameMenu();
    }
    private void OnClickRestart()
    {
        m_uiManager.RestartGame();
    }
    private void OnClickExit()
    {
        Debug.Log("cdds");
        m_uiManager.ShowMainMenu();
        m_uiManager.DeleteLevel();
    }
}
