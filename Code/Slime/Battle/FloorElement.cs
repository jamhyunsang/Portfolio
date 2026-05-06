using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FloorElement : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private Image Img_Ground = null;
    [SerializeField] private Image Img_Object = null;
    #endregion

    #region Member Property
    private ChapPlace m_ChapPlace = null;
    #endregion

    #region Member Method
    private void Init()
    {
        Img_Object.gameObject.SetActive(false);
    }

    public async UniTask SetElement(ChapPlace chapPlace)
    {
        Init();

        m_ChapPlace = chapPlace;

        if(m_ChapPlace.ObjKey != "Empty")
        {
            var ObjProfile = DataManager.GetTable<ObjProfile>(eTableType.ObjProfile).Find(Data => Data.ObjKey == m_ChapPlace.ObjKey);
            Img_Object.gameObject.SetActive(true);
            Img_Object.sprite = await ResourceLoader.LoadSpriteAsync("Object", ObjProfile.ObjPrefab, true);
        }
    }

    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    #endregion
}
