using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Cell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public int Number { get; private set; }
    public byte NumberPower { get; private set; }
    public bool HasMerged { get; private set; }

    public bool isEmpty => Number == 0;
    public const int maxNumber = 512;

    [SerializeField] private Image image;
    [SerializeField] private TMP_Text textNumber;

    private CellAnimation currentAnimation;
    
    [Inject] private ColorManager colorManager;
    [Inject] private GameController gameController;
    [Inject] private CellAnimationController cellAnimationController;
    
    public void SetValues(int x, int y, int number, byte numberPower, bool updateUI = true)
    {
        X = x;
        Y = y;
        Number = number;
        NumberPower = numberPower;

        if (updateUI)
        {
            UpdateCell();
        }
    }

    private void Merge()
    {
        HasMerged = true;
        ++NumberPower;
        Number *= 2;
        
        gameController.AddPoints();
    }

    public void ResetStatus()
    {
        HasMerged = false;
    }

    public void MergeWithCell(Cell otherCell)
    {
        cellAnimationController.SmoothTransition(this, otherCell, true);
        
        otherCell.Merge();
        SetValues(X, Y, 0, 0);
    }

    public void MoveToEmptyCell(Cell otherCell)
    {
        cellAnimationController.SmoothTransition(this, otherCell, false);
        
        otherCell.SetValues(otherCell.X, otherCell.Y, Number, NumberPower, false);
        SetValues(X, Y, 0, 0);
    }

    public void UpdateCell()
    {
        textNumber.text = isEmpty ? "" : Number.ToString();
        image.color = colorManager.getColor(NumberPower);
    }

    public void SetAnimation(CellAnimation cellAnimation)
    {
        currentAnimation = cellAnimation;
    }

    public void CancelAnimation()
    {
        if (currentAnimation != null)
        {
            currentAnimation.Destroy();
        }
    }
}
