using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Floor : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private GameObject Obj_Left = null;
    [SerializeField] private GameObject Obj_Mid = null;
    [SerializeField] private GameObject Obj_Right = null;
    #endregion

    #region Member Property
    private Image Img_Left = null;
    private Image Img_Mid = null;
    private Image Img_Right = null;

    private List<ChapPlace> m_ChapPlaces = null;
    #endregion

    #region Member Method
    public void Init()
    {
        Obj_Left.SetActive(false);
        Obj_Mid.SetActive(false);
        Obj_Right.SetActive(false);
    }

    public void SetFloor(List<ChapPlace> chapPlaces)
    {
        m_ChapPlaces = chapPlaces;
        foreach(var ChapPlace in m_ChapPlaces)
        {
            if (ChapPlace.HrPos == "Left")
            {
                Obj_Left.SetActive(true);
            }
            else if (ChapPlace.HrPos == "Mid")
            {
                Obj_Mid.SetActive(true);
            }
            else if (ChapPlace.HrPos == "Right")
            {
                Obj_Right.SetActive(true);
            }
        }
    }

    #endregion

    #region Unity Method
    private void Start()
    {
        Img_Left = Obj_Left.GetComponent<Image>();
        Img_Mid = Obj_Mid.GetComponent<Image>();
        Img_Right = Obj_Right.GetComponent<Image>();
    }
    #endregion
}
