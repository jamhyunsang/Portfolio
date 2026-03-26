using System;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private GameObject m_Root = null;

    #region Unity Method
    private async void Start()
    {
        _ = GameManager.Instance;

        UIManager.Instance.SetRoot(m_Root);

        StringManager.Instance.SetLanguage(GetLanguage());

        await StringManager.Instance.LoadClientString();
       
        await UIManager.Instance.Open<TitleWindow>(eUI.Main, "UI/Scene/TitleWindow", false);
    }
    #endregion

    private eLanguage GetLanguage()
    {
        var SavedLanguage = PlayerPrefs.GetString(ClientDefine.LANGUAGE_KEY, string.Empty);
        if (string.IsNullOrEmpty(SavedLanguage))
        {
            switch(Application.systemLanguage)
            {
                case SystemLanguage.Korean:
                    return eLanguage.Kor;
                case SystemLanguage.English:
                    return eLanguage.Eng;
                case SystemLanguage.Japanese:
                    return eLanguage.Jpn;
                case SystemLanguage.ChineseSimplified:
                    return eLanguage.Cns;
                case SystemLanguage.ChineseTraditional:
                    return eLanguage.Cnt;
                default:
                    return eLanguage.Kor;
            }
        }
        else
        {
            return Enum.Parse<eLanguage>(SavedLanguage);
        }    
    }
}
