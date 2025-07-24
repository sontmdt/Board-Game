using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : MonoBehaviour, IMenu
{
    private UIMainManager m_uiManager;
    private SoundController m_soundController;

    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnSetting;
    [SerializeField] private Button btnTimer;
    [SerializeField] private Button btnTurns;
    [SerializeField] private Button btnExit;
    [SerializeField] private Button btnExitSetting;
    [SerializeField] private Button btnOutGame;

    [SerializeField] private GameObject mode;
    [SerializeField] private GameObject setting;
    [SerializeField] private GameObject mainMenu;

    [SerializeField] private Slider musicVolume;
    [SerializeField] private Slider effectVolume;
    private void Awake()
    {
        m_soundController = FindObjectOfType<SoundController>();

        btnPlay.onClick.AddListener(OnClickPlay);
        btnSetting.onClick.AddListener(OnClickSetting);
        btnExit.onClick.AddListener(OnClickExit);
        btnExitSetting.onClick.AddListener(OnClickExit);
        btnTimer.onClick.AddListener(OnClickTimer);
        btnTurns.onClick.AddListener(OnClickTurns);
        btnOutGame.onClick.AddListener(OnClickOutGame);

        musicVolume.value = m_soundController.musicSource.volume;
        effectVolume.value = m_soundController.effectSource.volume;
        musicVolume.onValueChanged.AddListener(m_soundController.OnVolumeMusicChanged);
        effectVolume.onValueChanged.AddListener(m_soundController.OnVolumeSoundChanged);
    }
    private void OnDestroy()
    {
        if (btnTimer) btnTimer.onClick.RemoveAllListeners();
        if (btnTurns) btnTurns.onClick.RemoveAllListeners();
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
    private void OnClickTimer()
    {
        m_uiManager.LoadLevelTimer();
    }
    private void OnClickTurns()
    {
        m_uiManager.LoadLevelTurns();
    }
    private void OnClickPlay()
    {
        mainMenu.SetActive(false);
        mode.SetActive(true);
        setting.SetActive(false);
    }
    private void OnClickSetting()
    {
        mainMenu.gameObject.SetActive(false);
        mode.SetActive(false);
        setting.SetActive(true);
    }
    private void OnClickExit()
    {
        mainMenu.gameObject.SetActive(true);
        mode.SetActive(false);
        setting.SetActive(false);
    }
    private void OnClickOutGame()
    {
        Application.Quit();
    }
}