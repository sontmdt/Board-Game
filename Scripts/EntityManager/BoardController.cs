using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnMoveEvent = delegate { };
    public event Action<float, float> ChangedScoreBar = delegate { };
    public event Action<int> ChangedStar = delegate { };

    private Board m_board;
    private GameManager m_gameManager;
    private GameSettings m_gameSettings;
    private Camera m_camera;
    private Collider2D m_collider;
    public bool IsBusy { get; private set; }
    public bool CanSwap;
    private bool m_gameOver;
    private bool m_isHintShown;
    
    private float m_timeAfterFill;
    private bool m_isDragging;
    private List<Cell> m_potentialMatches;

    public float currentPoint;
    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;
        m_gameSettings = gameSettings;
        m_camera = Camera.main;
        m_board = new Board(this.transform, gameSettings);
        
        currentPoint = 0f;
        ChangedScoreBar(0f, 0f);
        ChangedStar(0);

        CanSwap = true;

        m_gameManager.StateChangedAction += OnGameStateChange;
        Fill();
    }
    private void Fill()
    {
        m_board.Fill();
        FindMatchesAndCollapse();
    }
    private void OnGameStateChange(eStateGame stateGame)
    {
        switch (stateGame)
        {
            case eStateGame.GAME_STARTED:
                IsBusy = false;
                break;
            case eStateGame.GAME_PAUSE:
                IsBusy = true;
                break;
            case eStateGame.GAME_OVER:
                m_gameOver = true;
                StopHint();
                break;
        }
    }
    public void Update()
    {
        if (m_gameOver) return;
        if (IsBusy) return;
        if (!CanSwap) return; 

        if (!m_isHintShown)
        {
            m_timeAfterFill += Time.deltaTime;
            if (m_timeAfterFill > m_gameSettings.TimeForHint)
            {
                m_timeAfterFill = 0f;
                ShowHint();
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                m_isDragging = true;
                m_collider = hit.collider;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            ResetRayCast();
        }
        if (Input.GetMouseButton(0) && m_isDragging)
        {
            var hit = Physics2D.Raycast(m_camera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                if (m_collider != null && m_collider != hit.collider)
                {
                    StopHint();

                    Cell c1 = m_collider.GetComponent<Cell>();
                    Cell c2 = hit.collider.GetComponent<Cell>();
                    if (AreNeighbour(c1, c2))
                    {
                        IsBusy = true;
                        SetSortingLayer(c1, c2);

                        m_gameManager.SwitchSound(); //Read again
                        m_board.Swap(c1, c2, () =>
                        {
                            FindMatchesAndCollapse(c1, c2);
                        });
                        ResetRayCast();
                    }
                }
            }
            else ResetRayCast();
        }
    }
    private void FindMatchesAndCollapse()
    {
        List<Cell> firstMatches = m_board.FindFirstMatch();
        if (firstMatches.Count > 0)
        {
            m_gameManager.CollapseSound();
            CollapseMatches(firstMatches, null);
        }
        else
        {
            m_potentialMatches = m_board.GetPotentialMatches();
            if (m_potentialMatches.Count > 0)
            {
                IsBusy = false;
                m_timeAfterFill = 0;
            }
            else StartCoroutine(ShuffleBoard());
        }
    }
    private void FindMatchesAndCollapse(Cell cell1, Cell cell2)
    {
        if (cell1.Item is BonusItem)
        {
            if (cell1.BoardX == cell2.BoardX) cell1.ExplodeItem("Vertical");//
            else if (cell1.BoardY == cell2.BoardY) cell1.ExplodeItem("Horizontal");//
            StartCoroutine(ShiftDownItems());
        }
        else if (cell2.Item is BonusItem)
        {
            if (cell1.BoardX == cell2.BoardX) cell2.ExplodeItem("Vertical");//
            else if (cell1.BoardY == cell2.BoardY) cell2.ExplodeItem("Horizontal");//
            StartCoroutine(ShiftDownItems());
        }
        else
        {
            List<Cell> cells1 = GetMatches(cell1);
            List<Cell> cells2 = GetMatches(cell2);

            List<Cell> matches = new List<Cell>();
            matches.AddRange(cells1);
            matches.AddRange(cells2);

            if (matches.Count < m_gameSettings.MatchesMin)
            {
                m_gameManager.SwitchSound();
                m_board.Swap(cell1, cell2, () =>
                {
                    IsBusy = false;
                });
            }
            else
            {
                OnMoveEvent();

                int ranIndex = UnityEngine.Random.Range(0, matches.Count);
                Cell bonusCell =  matches[ranIndex];
                CollapseMatches(matches, bonusCell);//Read again
            }
        }
    }

    //
    private void ResetRayCast()
    {
        m_isDragging = false;
        m_collider = null;
    }
    private void SetSortingLayer(Cell cell1, Cell cell2)
    {
        if (cell1.Item != null) cell1.Item.SetHigherOrderLayer();
        if (cell2.Item != null) cell2.Item.SetLowerOrderLayer();
    }
    private bool AreNeighbour(Cell cell1, Cell cell2)
    {
        return cell1.IsNeighbour(cell2);
    }
    //Hint
    private void ShowHint()
    {
        m_isHintShown = true;
        foreach (var cell in m_potentialMatches)
        {
            cell.AnimateItemForHint();
        }
    }
    private void StopHint()
    {
        m_isHintShown = false;
        foreach (var cell in m_potentialMatches)
        {
            cell.StopHintAnimation();
        }
        m_potentialMatches.Clear();
    }
    //
    private void CollapseMatches(List<Cell> matches, Cell bonusCell)
    {
        m_gameManager.CollapseSound();
        for (int i = 0; i < matches.Count; i++)
        {
            matches[i].ExplodeItem();
        }
        if (matches.Count > m_gameSettings.MatchesMin)
        {
            m_board.ConvertMatchesToBonusItem(matches, bonusCell);
        }
        //currentPoint += m_board.boardPointPerCandy * matches.Count;
        //ChangedScoreBar(currentPoint, currentPoint / m_board.boardPointForThreeStar);
        //ChangedStar(Score());
        StartCoroutine(ShiftDownItems());
    }
    private IEnumerator ShuffleBoard()
    {
        m_board.Shuffle();
        yield return new WaitForSeconds(0.3f);
        FindMatchesAndCollapse();
    }
    private IEnumerator ShiftDownItems()
    {
        currentPoint += m_board.ShiftDownItems() * m_board.boardPointPerCandy;
        ChangedScoreBar(currentPoint, currentPoint / m_board.boardPointForThreeStar);
        ChangedStar(Score());
        yield return new WaitForSeconds(0.2f);
        m_board.FillGapsWithNewItems();
        yield return new WaitForSeconds(0.2f);
        FindMatchesAndCollapse();

    }
    private List<Cell> GetMatches(Cell cell)
    {
        List<Cell> verticalMatches = m_board.GetVerticalMatches(cell);
        List<Cell> horizontalMatches = m_board.GetHorizontalMatches(cell);

        if (verticalMatches.Count < m_gameSettings.MatchesMin) verticalMatches.Clear();
        if (horizontalMatches.Count < m_gameSettings.MatchesMin) horizontalMatches.Clear();
        return verticalMatches.Concat(horizontalMatches).Distinct().ToList();
    }
    //Score
    public int Score()
    {
        if (currentPoint < m_board.boardPointForOneStar) return 0;
        else if (currentPoint < m_board.boardPointForTwoStar) return 1;
        else if (currentPoint < m_board.boardPointForThreeStar) return 2;
        return 3;
    }


    //
    internal void Clear()
    {
        m_board.Clear();
    }
}
