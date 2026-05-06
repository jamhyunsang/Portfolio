using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ResultPopup : UIElement
{
    #region Cashed Object
    [SerializeField] private Button Btn_OK = null;
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
        Btn_OK.onClick.AddListener(OnClick_OK);
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
    private void OnClick_OK()
    {
        _ = BattleModule.Instance.Exit();
    }
    #endregion
}
