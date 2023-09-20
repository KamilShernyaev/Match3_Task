
using UnityEngine;

public class UIFactory : IUIFactory
{
    private IAssetProvider _asset;

    public UIFactory(IAssetProvider asset)
    {
        _asset = asset;
    }
    public LoseWindow CreateLoseScreen() => 
        _asset.Instantiate(ConstantsAssetPath.LoseScreen, GameObject.FindWithTag("UI").transform).GetComponent<LoseWindow>();

    public VictoryWindow CreateVictoryScreen() => 
        _asset.Instantiate(ConstantsAssetPath.VictoryScreen, GameObject.FindWithTag("UI").transform).GetComponent<VictoryWindow>();
}