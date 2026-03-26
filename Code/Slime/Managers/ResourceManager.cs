using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : Singleton<ResourceManager>
{
    #region Override Method
    protected override void Init()
    {
    }
    #endregion

    #region Member Method
    public T LoadResource<T>(string path) where T : Object
    {
        return Resources.LoadAsync(path) as T;
    }

    public async UniTask<T> LoadResourceAsync<T>(string path, bool isAddressable) where T : Object
    {
        if (isAddressable)
        {
            string resourcePath = $"{path}";
            var handle = Addressables.LoadAssetAsync<T>(resourcePath);
            await handle.Task;
            return handle.Status == AsyncOperationStatus.Succeeded ? handle.Result : null;
        }
        else
        {
            return await Resources.LoadAsync(path) as T;
        }
    }

    public void ReleaseResource<T>(T resource, bool isAddressable) where T : Object
    {
        if (isAddressable)
        {
            Addressables.Release(resource);
        }
    }
    #endregion
}
