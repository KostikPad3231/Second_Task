using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class CellAnimation : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text numberText;

    private float moveTime = 0.1f;
    private float appearTime = 0.1f;
    
    private Sequence sequence;

    [Inject] private ColorManager colorManager;
    
    public void Move(Cell from, Cell to, bool isMerging)
    {
        from.CancelAnimation();
        to.SetAnimation(this);

        image.color = colorManager.getColor(from.NumberPower);
        numberText.text = from.Number.ToString();

        transform.position = from.transform.position;

        sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(to.transform.position, moveTime).SetEase(Ease.InOutQuad));

        if (isMerging)
        {
            sequence.AppendCallback(() =>
            {
                image.color = colorManager.getColor(to.NumberPower);
                numberText.text = to.Number.ToString();
            });

            sequence.Append(transform.DOScale(1.2f, appearTime));
            sequence.Append(transform.DOScale(1, appearTime));
        }

        sequence.AppendCallback(() =>
        {
            to.UpdateCell();
            Destroy();
        });
    }

    public void Appear(Cell cell)
    {
        cell.CancelAnimation();
        cell.SetAnimation(this);

        image.color = colorManager.getColor(cell.NumberPower);
        numberText.text = cell.Number.ToString();

        transform.position = cell.transform.position;
        transform.localScale = Vector2.zero;

        sequence = DOTween.Sequence();
        
        sequence.Append(transform.DOScale(1.2f, appearTime));
        sequence.Append(transform.DOScale(1, appearTime));
        sequence.AppendCallback(() =>
        {
            cell.UpdateCell();
            Destroy();
        });
    }

    public void Destroy()
    {
        sequence.Kill();
        Destroy(gameObject);
    }
}