using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BonusItem : Item
{
    public eBonusType ItemType;
    public void SetType(eBonusType type)
    {
        ItemType = type;
    }
    protected override string GetPrefabName()
    {
        string prefabName = string.Empty;
        switch (ItemType)
        {
            case eBonusType.NONE:
                break;
            case eBonusType.STRAIGHT:
                prefabName = Constants.PREFAB_BONUS_HORIZONTAL;
                break;
            case eBonusType.SQUARE:
                prefabName = Constants.PREFAB_BONUS_VERTICAL;
                break;
            case eBonusType.ALL:
                prefabName = Constants.PREFAB_BONUS_BOMB;
                break;
        }
        return prefabName;
    }
    internal override bool IsSameType(Item other)
    {
        BonusItem item = other as BonusItem;
        return item != null && item.ItemType == ItemType;
    }
    internal override void ExplodeView(string typeStraight)
    {
        BonusEffect(typeStraight);

        base.ExplodeView(typeStraight);
    }

    private void BonusEffect(string typeStraight)
    {
        switch (ItemType)
        {
            case eBonusType.STRAIGHT:
                ExplodeStraight(typeStraight);
                break;
            case eBonusType.SQUARE:
                ExplodeSquare();
                break;

        }
    }
    private void ExplodeStraight(string typeStraight)
    {
        if (typeStraight == "Vertical") ExplodeVerticalLine();
        if (typeStraight == "Horizontal") ExplodeHorizontalLine();
    }

    private void ExplodeSquare()
    {
        List<Cell> list = new List<Cell>();
        if (Cell.NeighbourUp) list.Add(Cell.NeighbourUp);
        if (Cell.NeighbourBottom) list.Add(Cell.NeighbourBottom);

        if (Cell.NeighbourLeft)
        {
            list.Add(Cell.NeighbourLeft);
            if (Cell.NeighbourLeft.NeighbourUp) list.Add(Cell.NeighbourLeft.NeighbourUp);
            if (Cell.NeighbourLeft.NeighbourBottom) list.Add(Cell.NeighbourLeft.NeighbourBottom);
        }
        if (Cell.NeighbourRight)
        {
            list.Add(Cell.NeighbourRight);
            if (Cell.NeighbourRight.NeighbourUp) list.Add(Cell.NeighbourRight.NeighbourUp);
            if (Cell.NeighbourRight.NeighbourBottom) list.Add(Cell.NeighbourRight.NeighbourBottom);
        }
        for (int i = 0; i < list.Count; i++)
        {
            list[i].ExplodeItem();
        }
    }

    private void ExplodeVerticalLine()
    {
        List<Cell> list = new List<Cell>();

        Cell currentCell = Cell;
        while (true)
        {
            Cell nextCell = currentCell.NeighbourUp;
            if (nextCell == null) break;

            list.Add(nextCell);
            currentCell = nextCell;
        }

        currentCell = Cell;
        while (true)
        {
            Cell nextCell = currentCell.NeighbourBottom;
            if (nextCell == null) break;

            list.Add(nextCell);
            currentCell = nextCell;
        }


        for (int i = 0; i < list.Count; i++)
        {
            list[i].ExplodeItem();
        }
    }

    private void ExplodeHorizontalLine()
    {
        List<Cell> list = new List<Cell>();

        Cell currentCell = Cell;
        while (true)
        {
            Cell nextCell = currentCell.NeighbourRight;
            if (nextCell == null) break;

            list.Add(nextCell);
            currentCell = nextCell;
        }

        currentCell = Cell;
        while (true)
        {
            Cell nextCell = currentCell.NeighbourLeft;
            if (nextCell == null) break;

            list.Add(nextCell);
            currentCell = nextCell;
        }


        for (int i = 0; i < list.Count; i++)
        {
            list[i].ExplodeItem();
        }

    }
}
