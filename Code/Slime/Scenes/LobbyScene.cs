using Unity.VisualScripting;
using UnityEngine;

public class LobbyScene : MonoBehaviour
{
    [SerializeField] private GameObject m_Root = null;

    #region Unity Method
    private async void Start()
    {
        UIManager.Instance.SetRoot(m_Root);
        UIManager.Instance.SwitchRoot();

        await UIManager.Instance.Open<LobbyWindow>(eUI.Main, "Assets/AddressableResources/UI/Scene/LobbyWindow.prefab", true);

        await UIManager.Instance.Close<LoadingWindow>();
    }
    #endregion
}
