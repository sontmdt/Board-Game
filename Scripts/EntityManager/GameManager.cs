using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public event Action<eStateGame> StateChangedAction = delegate { };
 
    private BoardController m_boardController;
    private UIMainManager m_uiManager;
    private LevelCondition m_levelCondition;
    private GameSettings m_gameSettings;

    public SoundController m_soundController;

    private string currentMode = "";

    private eStateGame m_stateGame;
    public eStateGame StateGame
    {
        get { return m_stateGame; }
        set
        {
            m_stateGame = value;
            StateChangedAction(m_stateGame);
        }
    }
    private void Awake()
    {
        StateGame = eStateGame.GAME_SETUP;

        m_gameSettings = Resources.Load<GameSettings>(Constants.GAME_SETTINGS_PATH);

        m_uiManager = FindObjectOfType<UIMainManager>();

        m_uiManager.Setup(this);

        m_soundController = GetComponent<SoundController>();

    }
    private void Start()
    {
        StateGame = eStateGame.MAIN_MENU;
        PlayOpeningMusic();
    }
    private void Update()
    {
        if (m_boardController != null) m_boardController.Update();
    }
    internal void SetState(eStateGame stateGame)
    {
        StateGame = stateGame;
        if (StateGame == eStateGame.GAME_PAUSE)
        {
            m_boardController.gameObject.SetActive(false);
            DOTween.PauseAll();
        }
        else
        {
            if (m_boardController != null) m_boardController.gameObject.SetActive(true);
            DOTween.PlayAll();
        }
    }
    internal void LoadLevelMode(eLevelMode mode)
    {
        m_boardController = new GameObject("BoardController").AddComponent<BoardController>();
        m_boardController.ChangedScoreBar += m_uiManager.uiGameStarted.UpdateScoreBar;
        m_boardController.ChangedStar += m_uiManager.uiGameStarted.UpdateStar;
        m_boardController.StartGame(this, m_gameSettings);
        if (mode == eLevelMode.TURNS)
        {
            currentMode = "TURNS";
            m_levelCondition = this.gameObject.AddComponent<LevelTurns>();
            m_levelCondition.Setup(m_gameSettings.LevelTurns, m_uiManager.GetLevelConditionView(), m_boardController);
        }
        else if (mode == eLevelMode.TIMER)
        {
            currentMode = "TIMER";
            m_levelCondition = this.gameObject.AddComponent<LevelTimer>();
            m_levelCondition.Setup(m_gameSettings.LevelTimer, m_uiManager.GetLevelConditionView(), this);//Read again
        }
        m_levelCondition.ConditionCompleteEvent += GameOver;//Read again
        StateGame = eStateGame.GAME_STARTED;
        PlayInGameMusic();
    }

    public void GameOver()
    {
        m_boardController.CanSwap = false;
        m_soundController.StopGameMusic();
        StartCoroutine(WaitBoardController());
    }
    private IEnumerator WaitBoardController()
    {
        while (m_boardController.IsBusy)
        {
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(1f);
        Destroy(m_boardController.gameObject);
        m_uiManager.uiGameOver.UpdateResult(m_boardController.Score());
        if (m_levelCondition != null)
        {
            m_levelCondition.ConditionCompleteEvent -= GameOver; //Read again
            Destroy(m_levelCondition);
            m_levelCondition = null;
        }
        StateGame = eStateGame.GAME_OVER;
    }
    public void RestartGame()
    {
        if (currentMode == "") return;
        if (currentMode == "TURNS")
        {
            DeleteLevel();
            LoadLevelMode(eLevelMode.TURNS);
        }
        else if (currentMode == "TIMER")
        {
            DeleteLevel();
            LoadLevelMode(eLevelMode.TIMER);
        }
    }
    public void DeleteLevel()
    {
        StateGame = eStateGame.MAIN_MENU;
        Destroy(this.gameObject.GetComponent<LevelCondition>());
        ClearLevel(); 
    }
    //Sound
    internal void SwitchSound()
    {
        m_soundController.PlaySwitchSound();
    }
    internal void CollapseSound()
    {
        m_soundController.PlayCollapseSound();
    }
    internal void PlayOpeningMusic()
    {
        m_soundController.PlayOpeningMusic();
    }
    internal void PlayInGameMusic()
    {
        m_soundController.PlayInGameMusic();
    }
    internal void StopGameMusic()
    {
        m_soundController.StopGameMusic();
    }
    //
    internal void ClearLevel()
    {
        if (!m_boardController) return;
        m_boardController.Clear();
        Destroy(m_boardController.gameObject);
        m_boardController = null;
    }
}
