using System;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

public class GameController : MonoBehaviour
{
    public int Score { get; private set; }
    public bool GameStarted { get; private set; }

    private int lastFrameIndex = 0;
    private float[] frameDeltaTime;

    [SerializeField] private TMP_Text fpsText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text gameResult;
    [SerializeField] private GameObject gameOverPanel;

    [Inject] private Board board;

    private void Start()
    {
        frameDeltaTime = new float[50];
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        StartGame();
    }

    private void Update()
    {
        frameDeltaTime[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTime.Length;

        fpsText.text = Mathf.RoundToInt(CalculateFPS()).ToString();
    }

    private float CalculateFPS()
    {
        return frameDeltaTime.Length / frameDeltaTime.Sum();
    }

    private void GameOver()
    {
        gameOverPanel.SetActive(true);
        GameStarted = false;
    }

    public void Win()
    {
        GameOver();
        gameResult.text = "Victory!";
    }

    public void Lose()
    {
        GameOver();
        gameResult.text = "Defeat!";
    }

    public void StartGame()
    {
        gameOverPanel.SetActive(false);
        gameResult.text = "";
        GameStarted = true;
        Score = 0;

        board.GenerateBoard();
    }

    public void AddPoints(int points = 1)
    {
        Score += points;
        ChangeScoreText();
    }

    private void ChangeScoreText()
    {
        scoreText.text = Score.ToString();
    }
}