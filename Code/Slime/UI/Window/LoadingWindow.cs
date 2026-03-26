using Cysharp.Threading.Tasks;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class LoadingWindow : UIElement
{
    [SerializeField] private Text Text_Progress;

    #region Override Method
    public override void Init()
    {

    }

    public override void Open()
    {

    }

    public override void Refresh()
    {
        
    }

    public override async UniTask OpenAsync()
    {
        await UniTask.Yield();
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
    // 100% 기준으로 표시하기
    public void SetProgress(float progress)
    {
        Text_Progress.text = $"{progress}%";
    }
    #endregion
}
