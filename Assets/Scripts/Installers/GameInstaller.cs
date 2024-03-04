using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ColorManager>().FromNew().AsSingle();
        Container.Bind<Board>().FromComponentInHierarchy().AsSingle();
        Container.Bind<GameController>().FromComponentInHierarchy().AsSingle();
        Container.Bind<Swiping>().FromComponentInHierarchy().AsSingle();
        Container.Bind<CellAnimationController>().FromComponentInHierarchy().AsSingle();
    }
}