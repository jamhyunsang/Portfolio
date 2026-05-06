using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Floor : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private FloorElement Left = null;
    [SerializeField] private FloorElement Mid = null;
    [SerializeField] private FloorElement Right = null;
    #endregion

    #region Member Method
    private void Init()
    {
        Left.SetActive(false);
        Mid.SetActive(false);
        Right.SetActive(false);
    }

    public async UniTask SetFloor(List<ChapPlace> chapPlaces)
    {
        Init();

        if (chapPlaces == null)
        {
            return;
        }

        foreach(var ChapPlace in chapPlaces)
        {
            if (ChapPlace.HrPos == "Left")
            {
                Left.SetActive(true);
                await Left.SetElement(ChapPlace);
            }
            else if (ChapPlace.HrPos == "Mid")
            {
                Mid.SetActive(true);
                await Mid.SetElement(ChapPlace);
            }
            else if (ChapPlace.HrPos == "Right")
            {
                Right.SetActive(true);
                await Right.SetElement(ChapPlace);
            }
        }
    }
    #endregion
}
