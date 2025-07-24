using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameStarted : MonoBehaviour, IMenu
{
    public Text LevelConditionView;

    [SerializeField] private Button btnPause;
    [SerializeField] private GameObject currentScoreBar;
    [SerializeField] private Text score;
    [SerializeField] private List<GameObject> stars = new List<GameObject>(3);
    private UIMainManager m_uiManager;
    private void Awake()
    {
        btnPause.onClick.AddListener(OnClickPause);
    }
    private void OnDestroy()
    {
        btnPause.onClick.RemoveAllListeners();
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
    private void OnClickPause()
    {
        m_uiManager.ShowPauseMenu();
    }
    public void UpdateScoreBar(float currentPoint, float scorePercent)
    {
        if (scorePercent <= 1f) currentScoreBar.transform.DOScaleX(scorePercent, 0.3f).SetEase(Ease.OutQuad);
        else currentScoreBar.transform.DOScaleX(1f, 0.3f).SetEase(Ease.OutQuad);
        score.text = currentPoint.ToString();
    }
    public void UpdateStar(int starCount)
    {
        for (int i = 0; i < stars.Count; i++)
        {
            if (i < starCount) stars[i].SetActive(true);
            else stars[i].SetActive(false);
        }
    }
}
