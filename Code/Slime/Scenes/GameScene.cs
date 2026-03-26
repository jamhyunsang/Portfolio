using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField] private GameObject m_Root = null;

    #region Unity Method
    private async void Start()
    {
        UIManager.Instance.SetRoot(m_Root);

        UIManager.Instance.SwitchRoot();

        await BattleModule.Instance.StartGame();
    }
    #endregion
}
