using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class StringManager : Singleton<StringManager>
{
    #region Member Property
    public Dictionary<string, string> m_Language = null;
    public eLanguage CurrentLanguage;
    #endregion

    #region Override Method
    protected override void Init()
    {
        m_Language = new Dictionary<string, string>();
    }
    #endregion

    #region Member Method
    public void SetLanguage(eLanguage language)
    {
        CurrentLanguage = language;
    }

    public async UniTask LoadClientString()
    {
        var STRText = await ResourceManager.Instance.LoadResourceAsync<TextAsset>($"Data/STR/STR_{CurrentLanguage}", false);

        if (STRText == null)
        {
            Debug.LogError($"Failed to load string data for language: {CurrentLanguage}");
            return;
        }

        var STRDecrypt = Util.Decrypt(STRText.bytes);
        var STRDeCompress = Util.DeCompress(STRDecrypt);

        m_Language.AddRange(Util.ToObject<Dictionary<string, string>>(STRDeCompress));
    }

    public async UniTask LoadAddressableString()
    {
        var STRText = await ResourceManager.Instance.LoadResourceAsync<TextAsset>($"Assets/AddressableResources/Data/STR/STR_{CurrentLanguage}", true);
        if (STRText == null)
        {
            Debug.LogError($"Failed to load addressable string data for language: {CurrentLanguage}");
            return;
        }
        var STRDecrypt = Util.Decrypt(STRText.bytes);
        var STRDeCompress = Util.DeCompress(STRDecrypt);

        m_Language.AddRange(Util.ToObject<Dictionary<string, string>>(Util.ToJson(STRDeCompress)));
    }

    public async UniTask LoadString()
    {
        m_Language.Clear();
        await LoadClientString();
        await LoadAddressableString();
    }

    public string GetString(string key)
    {
        if (m_Language.TryGetValue(key, out var value))
        {
            return value;
        }
        else
        {
            Debug.LogWarning($"String key '{key}' not found in language dictionary.");
            return key; // Return the key itself if not found
        }
    }
    #endregion
}
