using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using UnityEngine;

public class RewardSummaryListView : EnhancedScrollerCellView
{
    [SerializeField] private List<Element_Slot> Slots = null;
    [SerializeField] private List<DOTweenAnimation> Animations = null;

    public void SetItemData(List<ItemData> RewardItemList)
    {
        Init();

        for (int Count = 0; Count < RewardItemList.Count; Count++)
        {
            Slots[Count].gameObject.SetActive(true);
            Element_Slot_ItemData ItemData = new Element_Slot_ItemData();
            ItemData.ItemDataType = RewardItemList[Count];
            ItemData.IsShowCount = true;
            ItemData.IsButtonActive = false;
            ItemData.IsLongPressActive = true;

            Slots[Count].SetItem(ItemData);
        }
    }

    public void SetHeroData(List<HeroUniqueData> HeroKeyList)
    {
        Init();
        for (int Count = 0; Count < HeroKeyList.Count; Count++)
        {
            Slots[Count].gameObject.SetActive(true);
            Element_Slot_HeroData Data = new Element_Slot_HeroData();
            var HeroData = User.GetHero(HeroKeyList[Count]);
            Data.HeroKey = HeroData.Unique.HeroKey;
            Data.Grade = HeroData.Grade;
            Data.Type = HeroData.Type;
            Data.Lv = HeroData.Lv;
            Data.Grade = HeroData.Grade;
            Data.IsShowInfo = true;
            Slots[Count].SetHero_OutGame(Data);
        }
    }

    private void Init()
    {
        for (int count = 0; count < Slots.Count; count++)
        {
            Slots[count].gameObject.SetActive(false);
        }
    }

    public void PlayAnimation()
    {
        for (int Count = 0; Count < Animations.Count; Count++)
        {
            Animations[Count].DORestart();
        }
    }
}