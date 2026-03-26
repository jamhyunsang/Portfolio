using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class UIElement : MonoBehaviour
{
    public eUI UI;

    public abstract void Init();

    public abstract void Open();

    public abstract UniTask OpenAsync();

    public abstract void Refresh();

    public abstract void Close();

    public abstract UniTask CloseAsync();
}
