using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour, IMenu
{
    [SerializeField] private Button btnRestart;
    [SerializeField] private Button btnExit;
    [SerializeField] private List<GameObject> stars = new List<GameObject>(3);
    [SerializeField] private Text result;
    private UIMainManager m_uiManager;

    private void Awake()
    {
        btnRestart.onClick.AddListener(OnClickRestart);
        btnExit.onClick.AddListener(OnClickExit);
    }
    private void OnDestroy()
    {
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
    private void OnClickRestart()
    {
        m_uiManager.RestartGame();
    }
    private void OnClickExit()
    {
        m_uiManager.ShowMainMenu();
        m_uiManager.DeleteLevel();
    }
    public void UpdateResult(int starCount)
    {
        if (starCount < 1) result.text = "YOU LOSE!!!";
        else result.text = "YOU WIN!!!";
        for (int i = 0; i < stars.Count; i++)
        {
            if (i < starCount) stars[i].SetActive(true);
            else stars[i].SetActive(false);
        }
    }
}
