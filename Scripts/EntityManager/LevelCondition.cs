using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCondition : MonoBehaviour
{
    public event Action ConditionCompleteEvent = delegate { };
    
    public bool m_conditionCompleted = false;

    protected Text m_text;

    public virtual void Setup(float value, Text text)
    {
        m_text = text;
    }
    public virtual void Setup(float value, Text text, GameManager mngr)
    {
        m_text = text;
    }
    public virtual void Setup(float value, Text text, BoardController boardController)
    {
        m_text = text;
    }
    protected virtual void UpdateText() { }
    protected void OnConditionComplete()
    {
        m_conditionCompleted = true;
        
        ConditionCompleteEvent();
    }
    protected virtual void OnDestroy() { }
}
