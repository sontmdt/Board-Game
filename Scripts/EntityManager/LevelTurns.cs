using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelTurns : LevelCondition
{
    private int m_turns;

    private BoardController m_boardController;
    public override void Setup(float value, Text text, BoardController boardController)
    {
        base.Setup(value, text);

        m_turns = (int)value;

        m_boardController = boardController;

        m_boardController.OnMoveEvent += OnMove;
        UpdateText();

    }
    private void OnMove()
    {
        if (m_conditionCompleted) return;
        m_turns--;
        UpdateText();
        if (m_turns <= 0) OnConditionComplete();
    }
    protected override void UpdateText()
    {
        m_text.text = string.Format("TURNS:\n{0}", m_turns);
    }
    protected override void OnDestroy()
    {
        if (!m_boardController) return;
        m_boardController.OnMoveEvent -= OnMove;
    }
}
