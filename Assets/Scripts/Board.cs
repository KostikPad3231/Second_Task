using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class Board : MonoBehaviour
{
    public float cellSize;
    public float spacing;
    public int boardSize;
    public (int, int) generatedCellsCount = (2, 3);

    [SerializeField] private Cell cellPrefab;
    [SerializeField] private RectTransform _rectTransform;

    private Cell[,] board;

    private bool anyCellMoved;

    [Inject] private DiContainer diContainer;
    [Inject] private GameController gameController;
    [Inject] private InputController inputController;
    [Inject] private CellAnimationController cellAnimationController;

    private void Start()
    {
        inputController.InputEvent += OnInput;
    }

    private void OnInput(Vector2 direction)
    {
        if (!gameController.GameStarted)
        {
            return;
        }

        anyCellMoved = false;
        ResetCellsStatuses();

        Move(direction);

        if (anyCellMoved)
        {
            GenerateRandomCell();
            CheckGameResult();
        }
    }

    private void Move(Vector2 direction)
    {
        int startXY = direction.x > 0 || direction.y < 0 ? boardSize - 1 : 0;
        int dir = direction.x != 0 ? (int)direction.x : -(int)direction.y;
        for (int i = 0; i < boardSize; ++i)
        {
            for (int j = startXY; j >= 0 && j < boardSize; j -= dir)
            {
                var cell = direction.x != 0 ? board[j, i] : board[i, j];

                if (cell.isEmpty)
                {
                    continue;
                }

                var cellToMerge = FindCellToMerge(cell, direction);
                if (cellToMerge != null)
                {
                    cell.MergeWithCell(cellToMerge);
                    anyCellMoved = true;
                    continue;
                }

                var emptyCell = FindEmptyCell(cell, direction);
                if (emptyCell != null)
                {
                    cell.MoveToEmptyCell(emptyCell);
                    anyCellMoved = true;
                }
            }
        }
    }

    private Cell FindCellToMerge(Cell cell, Vector2 direction)
    {
        int startX = cell.X + (int)direction.x;
        int startY = cell.Y - (int)direction.y;

        for (int x = startX, y = startY;
             x >= 0 && x < boardSize && y >= 0 && y < boardSize;
             x += (int)direction.x, y -= (int)direction.y)
        {
            if (board[x, y].isEmpty)
            {
                continue;
            }

            if (board[x, y].Number == cell.Number && !board[x, y].HasMerged)
            {
                return board[x, y];
            }

            break;
        }

        return null;
    }

    private Cell FindEmptyCell(Cell cell, Vector2 direction)
    {
        Cell emptyCell = null;
        int startX = cell.X + (int)direction.x;
        int startY = cell.Y - (int)direction.y;

        for (int x = startX, y = startY;
             x >= 0 && x < boardSize && y >= 0 && y < boardSize;
             x += (int)direction.x, y -= (int)direction.y)
        {
            if (board[x, y].isEmpty)
            {
                emptyCell = board[x, y];
            }
            else
            {
                break;
            }
        }

        return emptyCell;
    }

    private void CheckGameResult()
    {
        bool lose = true;

        for (int x = 0; x < boardSize; ++x)
        {
            for (int y = 0; y < boardSize; ++y)
            {
                if (board[x, y].Number == Cell.maxNumber)
                {
                    gameController.Win();
                    return;
                }

                if (lose && (
                        board[x, y].isEmpty ||
                        FindCellToMerge(board[x, y], Vector2.up) ||
                        FindCellToMerge(board[x, y], Vector2.left) ||
                        FindCellToMerge(board[x, y], Vector2.down) ||
                        FindCellToMerge(board[x, y], Vector2.right)))
                {
                    lose = false;
                }
            }
        }

        if (lose)
        {
            gameController.Lose();
        }
    }

    private void CreateBoard()
    {
        board = new Cell[boardSize, boardSize];

        float boardWidth = boardSize * (cellSize + spacing) + spacing;
        _rectTransform.sizeDelta = new Vector2(boardWidth, boardWidth);

        float startX = -(boardWidth / 2) + (cellSize / 2) + spacing;
        float startY = (boardWidth / 2) - (cellSize / 2) - spacing;

        for (int x = 0; x < boardSize; ++x)
        {
            for (int y = 0; y < boardSize; ++y)
            {
                var cell = Instantiate(cellPrefab, transform, false);
                diContainer.Inject(cell);
                var position = new Vector2(startX + x * (cellSize + spacing), startY - y * (cellSize + spacing));
                cell.transform.localPosition = position;

                board[x, y] = cell;

                cell.GetComponent<RectTransform>().sizeDelta = new Vector2(cellSize, cellSize);

                cell.SetValues(x, y, 0, 0);
            }
        }
    }

    public void GenerateBoard()
    {
        if (board == null)
        {
            CreateBoard();
        }

        for (int x = 0; x < boardSize; ++x)
        {
            for (int y = 0; y < boardSize; ++y)
            {
                board[x, y].SetValues(x, y, 0, 0);
            }
        }

        for (int i = 0; i < Random.Range(generatedCellsCount.Item1, generatedCellsCount.Item2 + 1); ++i)
        {
            GenerateRandomCell();
        }
    }

    private void GenerateRandomCell()
    {
        List<Cell> emptyCells = board.Cast<Cell>().Where(cell => cell.isEmpty).ToList();

        int number = 2;

        var cell = emptyCells[Random.Range(0, emptyCells.Count)];
        cell.SetValues(cell.X, cell.Y, number, 1, false);
        
        cellAnimationController.SmoothAppear(cell);
    }

    private void ResetCellsStatuses()
    {
        for (int x = 0; x < boardSize; ++x)
        {
            for (int y = 0; y < boardSize; ++y)
            {
                board[x, y].ResetStatus();
            }
        }
    }
}