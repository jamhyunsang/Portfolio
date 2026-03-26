using Cysharp.Threading.Tasks;

public class TitleWindow : UIElement
{
    #region Member Property
    private int m_Step = 0;
    #endregion

    #region Override Method
    public override void Init()
    {
    }

    public override void Open()
    {
        Step();
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
    private void NextStep()
    {
        m_Step++;
        Step();
    }

    private async void Step()
    {
        await UniTask.Yield();

        switch (m_Step)
        {
            case 0: await LoadGameTable(); break;
            case 1: await GameStart(); break;
        }
    }

    #region GameTable
    private async UniTask LoadGameTable()
    {
        await DataManager.Instance.Load();
        NextStep();
    }
    #endregion

    #region Game Start
    private async UniTask GameStart()
    {
        await GameManager.Instance.LoadScene(eScene.Lobby);
    }
    #endregion
    #endregion
}
