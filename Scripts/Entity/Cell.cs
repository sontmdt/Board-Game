using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int BoardX {  get; private set; }
    public int BoardY { get; private set; }
    public Item Item { get; private set; }
    public Cell NeighbourUp { get; set; }
    public Cell NeighbourBottom { get; set; }
    public Cell NeighbourRight { get; set; }
    public Cell NeighbourLeft { get; set; }

    public Board m_board;

    public void Setup(int cellX, int cellY, Board board)
    {
        this.BoardX = cellX;
        this.BoardY = cellY;
        this.m_board = board;  
    }
    public bool IsEmpty => Item == null;
    public bool IsNeighbour(Cell other)
    {
        return BoardX == other.BoardX && Mathf.Abs(BoardY - other.BoardY) == 1
            || BoardY == other.BoardY && Mathf.Abs(BoardX - other.BoardX) == 1;
    }
    public void Free()
    {
        Item = null;
    }
    public void Assign(Item item)
    {
        Item = item;
        Item.SetCell(this);
    }
    public void ApplyItemPosition(bool withAppearAnimation)
    {
        Item.SetViewPosition(this.transform.position);
        if (withAppearAnimation) Item.ShowAppearAnimation();
    }
    //Animation
    internal void AnimateItemForHint()
    {
        if (Item == null) return;
        Item.AnimateForHint();
    }
    internal void StopHintAnimation()
    {
        if (Item == null) return;
        Item.StopAnimateForHint();
    }
    internal void ApplyItemMoveToPosition()
    {
        if (Item == null) return;
        Item.AnimationMoveToPosition();
    }
    //
    internal bool IsSameType(Cell other)
    {
        return Item != null && other.Item != null && other.Item.IsSameType(Item);
    }
    internal bool IsSameVertical(Cell other)
    {
        return Item != null && other.Item != null && other.BoardY == Item.Cell.BoardY;
    }
    internal bool IsSameHorizontal(Cell other)
    {
        return Item != null && other.Item != null && other.BoardX == Item.Cell.BoardX;
    }
    internal void ExplodeItem()
    {
        if (Item == null) return;
        Item.ExplodeView();
        Item = null;
    }
    internal void ExplodeItem(string typeStraight)
    {
        if (Item == null) return;
        Item.ExplodeView(typeStraight);
        Item = null;
    }
    internal void Clear()
    {
        if (Item == null) return;
        Item.Clear();
        Item = null;
    }

}
