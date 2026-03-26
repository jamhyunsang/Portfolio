using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class InGameWindow_Background : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private Image Img_Background = null;
    #endregion

    #region Member Method
    public async UniTask Init(string StageKey)
    {
        //TODO : StageKey에 따른 배경 이미지 변경
    }
    #endregion
}
