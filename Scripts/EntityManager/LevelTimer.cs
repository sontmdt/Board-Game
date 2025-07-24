using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelTimer : LevelCondition
{
    private float m_time;
    private GameManager m_gameManager;
    public override void Setup(float value, Text text, GameManager gameManager)
    {
        base.Setup(value, text, gameManager);
        m_gameManager = gameManager;
        m_time = value;
        UpdateText();
    }
    private void Update()
    {
        if (m_conditionCompleted) return;
        if (m_gameManager.StateGame != eStateGame.GAME_STARTED) return;
        m_time -= Time.deltaTime;
        UpdateText();
        if (m_time <= -1) OnConditionComplete();
    }
    protected override void UpdateText()
    {
        if (m_time < 0f) return;
        m_text.text = string.Format("TIME:\n{0:00}", m_time);
    }
}
