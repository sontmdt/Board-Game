using DG.Tweening;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Diagnostics;

public class Board
{
    private int boardSizeX;
    private int boardSizeY;
    public int boardPointForOneStar;
    public int boardPointForTwoStar;
    public int boardPointForThreeStar;
    public int boardPointPerCandy = 0;

    private Transform m_root;
    private int m_matchMin;
    public Cell[,] m_cellList { get; private set; }
    public Board(Transform transform, GameSettings gameSettings)
    {
        boardSizeX = gameSettings.BoardSizeX;
        boardSizeY = gameSettings.BoardSizeY;
        boardPointForOneStar = gameSettings.PointForOneStar;
        boardPointForTwoStar = gameSettings.PointForTwoStar;
        boardPointForThreeStar = gameSettings.PointForThreeStar;
        boardPointPerCandy = gameSettings.PointPerCandy;

        m_root = transform;
        m_matchMin = gameSettings.MatchesMin;
        m_cellList = new Cell[boardSizeX, boardSizeY];

        CreateBoard();
    }
    private void CreateBoard()
    {
        Vector3 originPoint = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f);
        GameObject prefabBackGround = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject currentCellBg = GameObject.Instantiate(prefabBackGround);
                currentCellBg.transform.position = originPoint + new Vector3(x, y, 0);
                currentCellBg.transform.SetParent(m_root);

                Cell cell = currentCellBg.GetComponent<Cell>();
                cell.Setup(x, y, this);
                m_cellList[x, y] = cell;
            }
        }

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (y + 1 < boardSizeY) m_cellList[x, y].NeighbourUp = m_cellList[x, y + 1];
                if (x + 1 < boardSizeX) m_cellList[x, y].NeighbourRight = m_cellList[x + 1, y];
                if (y > 0) m_cellList[x, y].NeighbourBottom = m_cellList[x, y - 1];
                if (x > 0) m_cellList[x, y].NeighbourLeft = m_cellList[x - 1, y];
            }
        }
    }
    internal void Fill()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cellList[x, y];
                NormalItem item = new NormalItem();
                List<eNormalType> types = new List<eNormalType>();

                //Get Item Type in bottom Neighbour and left Neighbour
                if (cell.NeighbourBottom != null)
                {
                    NormalItem normalItem = cell.NeighbourBottom.Item as NormalItem;
                    if (normalItem != null)
                    {
                        types.Add(normalItem.ItemType);
                    }
                }
                if (cell.NeighbourLeft != null)
                {
                    NormalItem normalItem = cell.NeighbourLeft.Item as NormalItem;
                    if (normalItem != null)
                    {
                        types.Add(normalItem.ItemType);
                    }
                }
                item.SetType(Utils.RandomNormalTypeExcept(types.ToArray()));
                item.SetView();
                item.SetViewParent(m_root);

                cell.Assign(item);
                cell.ApplyItemPosition(false);
            }
        }
    }
    internal void Shuffle()
    {
        List<Item> items = new List<Item>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                items.Add(m_cellList[x, y].Item);
                m_cellList[x, y].Free();
            }
        }
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                int randomIndex = UnityEngine.Random.Range(0, items.Count);
                m_cellList[x, y].Assign(items[randomIndex]);
                m_cellList[x, y].ApplyItemMoveToPosition();

                items.RemoveAt(randomIndex);
            }
        }
    }

    internal void FillGapsWithNewItems()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cellList[x, y];
                if (!cell.IsEmpty) continue;

                NormalItem item = new NormalItem();
                item.SetType(Utils.RandomNormalType());
                item.SetView();
                item.SetViewParent(m_root);
                cell.Assign(item);
                //Note: Can update
                cell.ApplyItemPosition(true);
            }
        }
    }
    internal void ExplodeAllItems()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cellList[x, y];
                cell.ExplodeItem();
            }
        }
    }
    //Note: Read Again
    internal void Swap(Cell cellOne, Cell cellTwo, Action callback)
    {
        Item itemOne = cellOne.Item;
        cellOne.Free();
        Item itemTwo = cellTwo.Item;
        cellOne.Assign(itemTwo);
        cellTwo.Free();
        cellTwo.Assign(itemOne);

        itemOne.View.DOMove(cellTwo.transform.position, 0.3f);
        itemTwo.View.DOMove(cellOne.transform.position, 0.3f).OnComplete(() =>
        {
            if (callback != null) callback();
        });
    }

    //Bonus Item
    internal void ConvertMatchesToBonusItem(List<Cell> matches, Cell cellToConvert)
    {
        eMatchDirection direction = GetMatchDirection(matches);

        BonusItem bonusItem = new BonusItem();
        switch (direction)
        {
            case eMatchDirection.ALL:
                bonusItem.SetType(eBonusType.ALL);
                break;
            case eMatchDirection.SQUARE:
                bonusItem.SetType(eBonusType.SQUARE);
                break;
            case eMatchDirection.STRAIGHT:
                bonusItem.SetType(eBonusType.STRAIGHT);
                break;
        }
        if (bonusItem == null) return;
        if (cellToConvert == null)
        {
            int rndIndex = UnityEngine.Random.Range(0, matches.Count);
            cellToConvert = matches[rndIndex];
        }
        bonusItem.SetView();
        bonusItem.SetViewParent(m_root);
        
        cellToConvert.Free();
        cellToConvert.Assign(bonusItem);
        cellToConvert.ApplyItemPosition(true);
    }
    public List<Cell> CheckBonusIfCompatible(List<Cell> matches)
    {
        var direction = GetMatchDirection(matches);
        var bonusItems = matches.Where(x => x.Item is BonusItem).FirstOrDefault();

        if (bonusItems == null) return matches;

        List<Cell> result = new List<Cell>();
        switch (direction)
        {
            case eMatchDirection.ALL:
                foreach (var cell in matches)
                {
                    BonusItem bonus = cell.Item as BonusItem;
                    if (bonus == null || bonus.ItemType == eBonusType.ALL)
                    {
                        result.Add(cell);
                    }
                }
                break;
            case eMatchDirection.SQUARE:
                foreach (var cell in matches)
                {
                    BonusItem bonus = cell.Item as BonusItem;
                    if (bonus == null || bonus.ItemType == eBonusType.SQUARE)
                    {
                        result.Add(cell);
                    }
                }
                break;
            case eMatchDirection.STRAIGHT:
                foreach (var cell in matches)
                {
                    BonusItem bonus = cell.Item as BonusItem;
                    if (bonus == null || bonus.ItemType == eBonusType.STRAIGHT)
                    {
                        result.Add(cell);
                    }
                }
                break;
        }
        return result;
    }
    internal eMatchDirection GetMatchDirection(List<Cell> matches)
    {
        if (matches == null || matches.Count < m_matchMin) return eMatchDirection.NONE;

        if (matches.Count <= 4)
        {
            var verticalMatches = matches.Where(x => x.BoardY == matches[0].BoardY).ToList();
            if (verticalMatches.Count == matches.Count)
            {
                return eMatchDirection.STRAIGHT;
            }
            var horizontalMatches = matches.Where(x => x.BoardX == matches[0].BoardX).ToList();
            if (horizontalMatches.Count == matches.Count)
            {
                return eMatchDirection.STRAIGHT;
            }
        }
        if (matches.Count >= 5)
        {
            bool checkSameVertical = true;
            bool checkSameHorizontal = true;
            for (int i = 1; i < matches.Count; i++)
            {
                if (!matches[i].IsSameVertical(matches[i - 1])) checkSameVertical = false;
                if (!matches[i].IsSameHorizontal(matches[i - 1])) checkSameHorizontal = false;
            }
            if (checkSameVertical || checkSameHorizontal) return eMatchDirection.ALL;
            else return eMatchDirection.SQUARE;
        }
        return eMatchDirection.NONE;
    }
    //
    internal List<Cell> FindFirstMatch()
    {
        List<Cell> cells = new List<Cell>();
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cellList[x, y];
                var matchesHorizontal = GetHorizontalMatches(cell);
                if (matchesHorizontal.Count >= m_matchMin)
                {
                    cells = matchesHorizontal;
                    break;
                }
                var matchesVertical = GetVerticalMatches(cell);
                if (matchesVertical.Count >= m_matchMin)
                {
                    cells = matchesVertical;
                    break;
                }
            }
        }

        return cells;
    }
    //
    internal List<Cell> GetHorizontalMatches(Cell cell)
    {
        List<Cell> cells = new List<Cell>();
        cells.Add(cell);

        Cell currentCell = cell;
        while (true)
        {
            Cell left = currentCell.NeighbourLeft;
            if (left == null) break;
            if (left.IsSameType(cell))
            {
                cells.Add(left);
                currentCell = left;
            }
            else break;
        }
        currentCell = cell;
        while (true)
        {
            Cell right = currentCell.NeighbourRight;
            if (right == null) break;
            else if (right.IsSameType(cell))
            {
                cells.Add(right);
                currentCell = right;
            }
            else break;
        }

        return cells;
    }
    internal List<Cell> GetVerticalMatches(Cell cell)
    {
        List<Cell> cells = new List<Cell>();
        cells.Add(cell);

        Cell currentCell = cell;
        while (true)
        {
            Cell up = currentCell.NeighbourUp;
            if (up == null) break;
            if (up.IsSameType(cell))
            {
                cells.Add(up);
                currentCell = up;
            }
            else break;
        }
        currentCell = cell;
        while (true)
        {
            Cell bottom = currentCell.NeighbourBottom;
            if (bottom == null) break;
            else if (bottom.IsSameType(cell))
            {
                cells.Add(bottom);
                currentCell = bottom;
            }
            else break;
        }

        return cells;
    }

    // Very Important
    internal List<Cell> GetPotentialMatches()
    {
        List<Cell> potentialMatches = new List<Cell>();

        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cellList[x, y];

                //Check cell 1 or 3
                //Up
                if (cell.NeighbourUp != null)
                {
                    potentialMatches = GetPotentialMatches(cell, cell.NeighbourUp, cell.NeighbourUp.NeighbourUp);
                    if (potentialMatches.Count > 0) break;
                }
                //Bottom
                if (cell.NeighbourBottom != null)
                {
                    potentialMatches = GetPotentialMatches(cell, cell.NeighbourBottom, cell.NeighbourBottom.NeighbourBottom);
                    if (potentialMatches.Count > 0) break;
                }
                //Right
                if (cell.NeighbourRight != null)
                {
                    potentialMatches = GetPotentialMatches(cell, cell.NeighbourRight, cell.NeighbourRight.NeighbourRight);
                    if (potentialMatches.Count > 0) break;
                }
                //Left
                if (cell.NeighbourLeft != null)
                {
                    potentialMatches = GetPotentialMatches(cell, cell.NeighbourLeft, cell.NeighbourLeft.NeighbourLeft);
                    if (potentialMatches.Count > 0) break;
                }

                //Check cell 2
                //Vertical matches
                Cell cellTwo = cell.NeighbourUp;
                if (cellTwo != null && cellTwo.NeighbourUp != null && cellTwo.NeighbourUp.IsSameType(cell))
                {
                    Cell potentialCell = LookForTheSecondCellForMatchesVertical(cellTwo, cell);
                    if (potentialCell != null)
                    {
                        //132
                        potentialMatches.Add(cell);
                        potentialMatches.Add(cellTwo.NeighbourUp);
                        potentialMatches.Add(potentialCell);
                        break;
                    }
                }
                //Horizontal matches
                cellTwo = null;
                cellTwo = cell.NeighbourRight;
                if (cellTwo != null && cellTwo.NeighbourRight != null && cellTwo.NeighbourRight.IsSameType(cell))
                {
                    Cell potentialCell = LookForTheSecondCellForMatchesHorizontal(cellTwo, cell);
                    if (potentialCell != null)
                    {
                        //132
                        potentialMatches.Add(cell);
                        potentialMatches.Add(cellTwo.NeighbourRight);
                        potentialMatches.Add(potentialCell);
                        break;
                    }
                }
            }
            if (potentialMatches.Count > 0) break;
        }
        return potentialMatches;
    }

    private List<Cell> GetPotentialMatches(Cell cellOne, Cell cellTwo, Cell cellThree)
    {
        List<Cell> potentialMatches = new List<Cell>();

        if (cellTwo != null && cellTwo.IsSameType(cellOne))
        {
            Cell third = LookForTheThirdCell(cellTwo, cellThree);
            if (third != null)
            {
                potentialMatches.Add(cellOne);
                potentialMatches.Add(cellTwo);
                potentialMatches.Add(third);
            }
        }

        return potentialMatches;
    }
    //Check cell 2
    private Cell LookForTheSecondCellForMatchesVertical(Cell cellTwo, Cell cellOne)
    {
        if (cellTwo == null) return null;
        if (cellTwo.IsSameType(cellOne)) return null;

        //Right
        Cell potentialCell = cellTwo.NeighbourRight;
        if (potentialCell != null && potentialCell.IsSameType(cellOne)) return potentialCell;
        //Left
        potentialCell = null;
        potentialCell = cellTwo.NeighbourLeft;
        if (potentialCell != null && potentialCell.IsSameType(cellOne)) return potentialCell;

        return null;
    }

    private Cell LookForTheSecondCellForMatchesHorizontal(Cell cellTwo, Cell cellOne)
    {
        if (cellTwo == null) return null;
        if (cellTwo.IsSameType(cellOne)) return null;

        //Up
        Cell potentialCell = cellTwo.NeighbourUp;
        if (potentialCell != null && potentialCell.IsSameType(cellOne)) return potentialCell;
        //Bottom
        potentialCell = null;
        potentialCell = cellTwo.NeighbourBottom;
        if (potentialCell != null && potentialCell.IsSameType(cellOne)) return potentialCell;

        return null;
    }
    //
    //Check cell 1 or 3
    private Cell LookForTheThirdCell(Cell cellTwo, Cell cellThree)
    {
        if (cellThree == null) return null;
        if (cellThree.IsSameType(cellTwo)) return null;

        //Up
        Cell third = CheckSameTypeCell(cellThree.NeighbourUp, cellTwo);
        if (third != null) return third;
        third = null;
        //Bottom 
        third = CheckSameTypeCell(cellThree.NeighbourBottom, cellTwo);
        if (third != null) return third;
        third = null;
        //Right
        third = CheckSameTypeCell(cellThree.NeighbourRight, cellTwo);
        if (third != null) return third;
        third = null;
        //Left
        third = CheckSameTypeCell(cellThree.NeighbourLeft, cellTwo);
        if (third != null) return third;
        third = null;

        return null;
    }
    private Cell CheckSameTypeCell(Cell cell1, Cell cell2)
    {
        if (cell1 != null && cell1 != cell2 && cell1.IsSameType(cell2))
        {
            return cell1;
        }
        return null;
    }
    //
    internal int ShiftDownItems()
    {
        int totalEmptyCell = 0;
        for (int x = 0; x < boardSizeX; x++)
        {
            int emptyCellNumber = 0;
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cellList[x, y];
                if (cell.IsEmpty)
                {
                    emptyCellNumber++;
                    continue;
                }
                if (emptyCellNumber == 0) continue;

                Cell holder = m_cellList[x, y - emptyCellNumber];
                Item item = cell.Item;
                cell.Free();
                holder.Assign(item);
                item.View.DOMove(holder.transform.position, 0.3f);
            }
            totalEmptyCell += emptyCellNumber;
        }
        return totalEmptyCell;
    }
    internal void Clear()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cellList[x, y];
                cell.Clear();

                GameObject.Destroy(cell.gameObject);
                m_cellList[x, y] = null;
            }
        }
    }
}
