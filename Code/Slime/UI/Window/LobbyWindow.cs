using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyWindow : UIElement
{
    #region Member Property
    [SerializeField] private Button Btn_GameStart = null;
    #endregion

    #region Override Method
    public override void Init()
    {
        Btn_GameStart.onClick.AddListener(async () => await OnClick_GameStartAsync());
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

    public override void Close()
    {


    }

    public override async UniTask CloseAsync()
    {
        await UniTask.Yield();
    }
    #endregion

    #region Member Method
    private async UniTask OnClick_GameStartAsync()
    {
        var Module = BattleModule.CreateBattleModule<ChapterBattleModule>();
        Module.SetChapterKey("ChapNor1");
        await GameManager.Instance.LoadScene(eScene.Game);
    }
    #endregion
}
