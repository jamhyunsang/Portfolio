using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class InGameWindow : UIElement
{
    #region Cashed Object
    [SerializeField] private InGameWindow_Background Background = null;
    [SerializeField] private InGameWindow_Top Top = null;
    [SerializeField] private InGameWindow_Game Game = null;
    #endregion

    #region Override Method
    public override void Close()
    {
    }

    public override async UniTask CloseAsync()
    {
        await UniTask.Yield();
    }

    public override void Init()
    {

    }

    public override void Open()
    {
    }

    public override async UniTask OpenAsync()
    {
        await UniTask.Yield();
    }

    public override void Refresh()
    {
    }
    #endregion

    #region Member Method
    public async UniTask InitWindow(string stageKey)
    {
        await Background.Init(stageKey);
        await Top.Init(stageKey);
        await Game.Init(stageKey);
    }
    #endregion
}
