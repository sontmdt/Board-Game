using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public Cell Cell { get; private set; }

    public Transform View { get; private set; }

    private Vector3 originalScale = new Vector3(1,1,1);
    public virtual void SetView()
    {
        string prefabName = GetPrefabName();
        if (prefabName == null || prefabName == "") return;
        GameObject prefab = Resources.Load<GameObject>(prefabName);
        if (!prefab) return;
        View = GameObject.Instantiate(prefab).transform;
    }
    protected virtual string GetPrefabName() { return string.Empty; }

    public virtual void SetCell(Cell cell)
    {
        Cell = cell;
    }
    public void SetViewPosition(Vector3 position)
    {
        if (View) View.position = position;
    }
    public void SetViewParent(Transform parrent)
    {
        if (View) View.SetParent(parrent);
    }
    //Set layer
    public void SetHigherOrderLayer()
    {
        if (!View) return;
        SpriteRenderer spriteRenderer = View.GetComponent<SpriteRenderer>();
        if (!spriteRenderer) return;
        spriteRenderer.sortingOrder = 1;
    }
    public void SetLowerOrderLayer()
    {
        if (!View) return;
        SpriteRenderer spriteRenderer = View.GetComponent<SpriteRenderer>();
        if (!spriteRenderer) return;
        spriteRenderer.sortingOrder = 0;
    }
    //Animation
    internal void AnimationMoveToPosition()
    {
        if (!View) return;
        View.DOMove(Cell.transform.position, 0.2f);
    }
    internal void ShowAppearAnimation()
    {
        if (!View) return;

        Vector3 itemScale = View.localScale;
        View.localScale = Vector3.one * 0.1f;
        View.DOScale(itemScale, 0.1f);
    }
    internal void AnimateForHint()
    {
        if (!View) return;
        View.DOPunchScale(View.localScale * 0.4f, 0.2f).SetLoops(-1);
    }
    internal void StopAnimateForHint()
    {
        if (!View) return;
        View.DOKill();
        View.localScale = originalScale;
    }
    //
    internal virtual bool IsSameType(Item other)
    {
        return false;
    }
    internal virtual void ExplodeView()
    {
        if (!View) return;
        View.DOScale(0.1f, 0.1f).OnComplete(
            () =>
            {
                GameObject.Destroy(View.gameObject);
                View = null;
            });
    }
    internal virtual void ExplodeView(string typeStraight)
    {
        if (!View) return;
        View.DOScale(0.1f, 0.1f).OnComplete(
            () =>
            {
                GameObject.Destroy(View.gameObject);
                View = null;
            });
    }
    internal void Clear()
    {
        Cell = null;
        if (!View) return;
        GameObject.Destroy(View.gameObject);
        View = null;
    }
}
