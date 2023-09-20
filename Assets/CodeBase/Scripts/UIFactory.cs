
using UnityEngine;

public class UIFactory : IUIFactory
{
    private IAssetProvider _asset;

    public UIFactory(IAssetProvider asset)
    {
        _asset = asset;
    }
    public LoseWindow CreateLoseScreen() => 
        _asset.Instantiate(ContantsAssetPath.LoseScreen, GameObject.FindWithTag("UI").transform).GetComponent<LoseWindow>();

    public VictoryWindow CreateVictoryScreen() => 
        _asset.Instantiate(ContantsAssetPath.VictoryScreen, GameObject.FindWithTag("UI").transform).GetComponent<VictoryWindow>();
}