using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    #region Override Method
    protected override void Init()
    {
        CreateManagers();
    }
    #endregion

    #region Member Method
    private void CreateManagers()
    {
        _ = DataManager.Instance;
        _ = SoundManager.Instance;
        _ = UIManager.Instance;
    }

    #region Scene
    public async UniTask LoadScene(eScene scene)
    {
        var LoadingWindow = await UIManager.Instance.Open<LoadingWindow>(eUI.Loading, "UI/OutGame/LoadingWindow", false);

        await UIManager.Instance.CloseAll();

        var CurrentScene = SceneManager.GetActiveScene();

        var LoadTask = SceneManager.LoadSceneAsync($"{scene}", LoadSceneMode.Additive);

        while (!LoadTask.isDone)
        {
            //loadTask.progress;
            await UniTask.Yield();
            LoadingWindow.SetProgress(Util.Lerp(0f, 50f, LoadTask.progress, 1f));
        }

        await LoadTask.ToUniTask();

        var unloadTask = SceneManager.UnloadSceneAsync(CurrentScene);

        while (!unloadTask.isDone)
        {
            //unloadTask.progress;
            await UniTask.Yield();
            LoadingWindow.SetProgress(Util.Lerp(50f, 100f, unloadTask.progress, 1f));
        }

        await unloadTask.ToUniTask();
    }
    #endregion
    #endregion
}
