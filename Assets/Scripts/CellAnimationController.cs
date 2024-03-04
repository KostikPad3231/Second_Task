using DG.Tweening;
using UnityEngine;
using Zenject;

public class CellAnimationController : MonoBehaviour
{
    [SerializeField] private CellAnimation animationPrefab;
    
    [Inject] private DiContainer diContainer;
    
    private void Awake()
    {
        DOTween.Init();
    }

    public void SmoothTransition(Cell from, Cell to, bool isMerging)
    {
        var cellAnimation = Instantiate(animationPrefab, transform, false);
        diContainer.Inject(cellAnimation);
        cellAnimation.Move(from, to, isMerging);
    }

    public void SmoothAppear(Cell cell)
    {
        var cellAnimation = Instantiate(animationPrefab, transform, false);
        diContainer.Inject(cellAnimation);
        cellAnimation.Appear(cell);
    }
}
