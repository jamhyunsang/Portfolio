using EnhancedUI.EnhancedScroller;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GachaProbabilityCellView : EnhancedScrollerCellView
{
    #region Cashed Object
    [SerializeField] private GameObject Obj_Title = null;
    [SerializeField] private GameObject Obj_SlotPos = null;
    [SerializeField] private List<Element_Slot> Slots = null;

    [SerializeField] private Text Text_ClassProbability = null;
    [SerializeField] private List<GameObject> Obj_Grades = null;
    #endregion

    #region Member Property
    public float TitleHeight
    {
        get { return Obj_Title.GetComponent<RectTransform>().rect.height; }
    }

    public float ListHeight
    {
        get
        { 
            var Grid = Obj_SlotPos.GetComponent<GridLayoutGroup>();
            return Grid.cellSize.y + Grid.spacing.y;
        }
    }
    #endregion

    #region Member Method
    private void Init()
    {
        for (int count = 0; count < Slots.Count; count++)
        {
            Slots[count].SetActive(false);
        }

        for(int count = 0; count < Obj_Grades.Count; count++)
        {
            Obj_Grades[count].SetActive(false);
        }
    }

    public void SetData(int Grade, int TotalRate, List<GachaProbabilityData> ProbabilityList)
    {
        if (ProbabilityList.Count == 0)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            Init();

            Obj_Grades[Grade - 1].SetActive(true);

            var GroupRate = Util.UniformVelocity_float(0f, 100f, ProbabilityList.Sum(Data => Data.Probability), TotalRate);
            Text_ClassProbability.text = $"{GroupRate:0.0#}%";

            for (int count = 0; count < ProbabilityList.Count; count++)
            {
                Element_Slot_GachaProbability_Data Data = new Element_Slot_GachaProbability_Data();
                Data.HeroKey = ProbabilityList[count].CharIdx;
                Data.Type = ProbabilityList[count].Type;
                Data.Grade = ProbabilityList[count].Grade;
                Data.Rate = Util.UniformVelocity_float(0f, 100f, ProbabilityList[count].Probability, TotalRate);

                Slots[count].SetActive(true);
                Slots[count].SetGachaProbability(Data);
            }
        }
    }
    #endregion
}
