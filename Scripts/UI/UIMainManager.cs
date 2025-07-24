using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIMainManager : MonoBehaviour
{
    private IMenu[] m_menuList;
    private GameManager m_gameManager;
    public UIGameStarted uiGameStarted;
    public UIGameOver uiGameOver;
    private void Awake()
    {
        m_menuList = GetComponentsInChildren<IMenu>(true);
    }
    private void Start()
    {
        for (int i = 0; i < m_menuList.Length; ++i)
        {
            m_menuList[i].Setup(this);
            if (m_menuList[i] is UIGameStarted) uiGameStarted = (UIGameStarted)m_menuList[i];
            if (m_menuList[i] is UIGameOver) uiGameOver = (UIGameOver)m_menuList[i];
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (m_gameManager.StateGame == eStateGame.GAME_STARTED)
            {
                m_gameManager.SetState(eStateGame.GAME_PAUSE);
            }
            else if (m_gameManager.StateGame == eStateGame.GAME_PAUSE)
            {
                m_gameManager.SetState(eStateGame.GAME_STARTED);
            }
        }
    }
    internal void Setup(GameManager gameManager)
    {
        m_gameManager = gameManager;
        m_gameManager.StateChangedAction += OnGameStateChange;
    }
    private void OnGameStateChange(eStateGame stateGame)
    {
        switch (stateGame)
        {
            case eStateGame.GAME_SETUP:
                break;
            case eStateGame.MAIN_MENU:
                ShowMenu<UIMainMenu>();
                break;
            case eStateGame.GAME_STARTED:
                ShowMenu<UIGameStarted>();
                break;
            case eStateGame.GAME_PAUSE:
                ShowMenu<UIGamePause>();
                break;
            case eStateGame.GAME_OVER:
                ShowMenu<UIGameOver>();
                break;
        }
    }
    private void ShowMenu<T>() where T : IMenu
    {
        for (int i = 0; i < m_menuList.Length; i++)
        {
            IMenu menu = m_menuList[i];
            if (menu is T) menu.Show();
            else menu.Hide();
        }
    }
    //
    internal Text GetLevelConditionView()
    {
        UIGameStarted uiGameStarted = m_menuList
            .Where(x => x is UIGameStarted)
            .Cast<UIGameStarted>()
            .FirstOrDefault();
        if (!uiGameStarted) return null;
        return uiGameStarted.LevelConditionView;
    }

    //Show current UI state
    internal void ShowMainMenu()
    {
        m_gameManager.ClearLevel();
        m_gameManager.SetState(eStateGame.MAIN_MENU);
        m_gameManager.m_soundController.StopGameMusic();
        m_gameManager.PlayOpeningMusic();
    }
    internal void ShowPauseMenu()
    {
        m_gameManager.SetState(eStateGame.GAME_PAUSE);
    }
    internal void ShowGameMenu()
    {
        if (m_gameManager.StateGame != eStateGame.GAME_PAUSE)
        {
            m_gameManager.m_soundController.StopGameMusic();
            m_gameManager.PlayInGameMusic();
        }
        m_gameManager.SetState(eStateGame.GAME_STARTED);
    }
    //Load level mode
    internal void RestartGame()
    {
        m_gameManager.RestartGame();
    }
    internal void DeleteLevel()
    {
        m_gameManager.DeleteLevel();
    }
    internal void LoadLevelTurns()
    {
        m_gameManager.LoadLevelMode(eLevelMode.TURNS);
    }
    internal void LoadLevelTimer()
    {
        m_gameManager.LoadLevelMode(eLevelMode.TIMER);
    }
}
