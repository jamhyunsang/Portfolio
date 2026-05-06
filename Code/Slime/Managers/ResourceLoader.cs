using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public static class ResourceLoader
{
    #region Member Method
    public static async UniTask<T> LoadResourceAsync<T>(string path, bool isAddressable) where T : Object
    {
        if (isAddressable)
        {
            string ResourcePath = $"Assets/AddressableResources/{path}";
            var Handle = Addressables.LoadAssetAsync<T>(ResourcePath);
            await Handle.Task;
            return Handle.Status == AsyncOperationStatus.Succeeded ? Handle.Result : null;
        }
        else
        {
            return await Resources.LoadAsync(path) as T;
        }
    }

    public static async UniTask<Sprite> LoadSpriteAsync(string atlasName, string spriteName, bool isAddressable) 
    {
        if (isAddressable)
        {
            string ResourcePath = $"Assets/AddressableResources/Atlas/{atlasName}.spriteatlas";
            var Handle = Addressables.LoadAssetAsync<SpriteAtlas>(ResourcePath);
            await Handle.Task;
            if (Handle.Status == AsyncOperationStatus.Succeeded)
            {
                var Atlas = Handle.Result;
                var Sprite = Atlas.GetSprite(spriteName);
                if (Sprite == null)
                {
                    Debug.LogError($"Sprite {spriteName} not found in atlas {atlasName}");
                    return null;
                }
                return Sprite;
            }
            else
            {
                Debug.LogError($"Failed to load atlas {atlasName} from addressables");
                return null;
            }
        }
        else
        {
            var Atlas = await Resources.LoadAsync<SpriteAtlas>($"Atlas/{atlasName}") as SpriteAtlas;
            var Sprite = Atlas.GetSprite(spriteName);
            if(Sprite == null)
            {
                Debug.LogError($"Sprite {spriteName} not found in atlas {atlasName}");
                return null;
            }
            return Sprite;
        }
    }

    public static void ReleaseResource<T>(T resource, bool isAddressable) where T : Object
    {
        if (isAddressable)
        {
            Addressables.Release(resource);
        }
    }
    #endregion
}
