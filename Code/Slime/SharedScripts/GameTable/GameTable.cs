using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

public class NonPublicDefaultContractResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        var property = base.CreateProperty(member, memberSerialization);
        if (!property.Writable)
        {
            var propertyInfo = member as PropertyInfo;
            if (propertyInfo != null)
            {
                var setter = propertyInfo.GetSetMethod(true);
                if (setter != null)
                {
                    property.Writable = true;
                }
            }
        }

        return property;
    }
}

public enum eTableType
{
    BaseConfig,
    AccountLv,
    AccountProfileIcon,
    ContentsLock,
    Tutorial,
    ChapInfo,
    ChapProfile,
    ChapClearReward,
    ChapPlace,
    CharInfo,
    CharProfile,
    CharLvUpStat,
    CharLvUpExp,
    CharPromoteInfo,
    CharPromoteUpStat,
    EventInfo,
    EventReward,
    ItemInfo,
    ItemProfile,
    ItemParam,
    ItemSummonRandom,
    ItemSummonSelect,
    ItemSummonConfirm,
    ItemLuckyBag,
    ItemProfileIcon,
    MissionUiMove,
    MissionKind,
    MissionInfo,
    MissionScoreReward,
    AttendRepeat,
    AttendCumulate,
    MonInfo,
    MonProfile,
    MonLvUpStat,
    ObjInfo,
    ObjProfile,
    ShopInfo,
    SkillType,
    SkillEffect,
    SkillInfo,
    SkillProfile,
    SummonInfo,
    SummonDailyFree,
    SummonRateGroup,
    SummonCharNormalRate,
    SummonCharPickRate,
    SummonCharDuplicateReward,
    End
}

public enum eLanguage
{
    Kor,
    Eng,
    Jpn,
    Cns,
    Cnt,
    End
}

public class GameTable
{
    public static Dictionary<eTableType, object> Parse(JArray Obj)
    {
        Dictionary<eTableType, object> Result = new Dictionary<eTableType, object>();
        var settings = new JsonSerializerSettings
        {
            ContractResolver = new NonPublicDefaultContractResolver(),
            Formatting = Formatting.Indented
        };
        for (int Count = 0; Count < Obj.Count; ++Count)
        {
            var CurTableName = Obj[Count]["Key"].Value<string>();
            var CurTableData = Obj[Count]["Value"].Value<string>();
            switch (CurTableName)
            {
                case "BaseConfig":
                    Result.Add(eTableType.BaseConfig, JsonConvert.DeserializeObject<List<BaseConfig>>(CurTableData, settings));
                    break;
                case "AccountLv":
                    Result.Add(eTableType.AccountLv, JsonConvert.DeserializeObject<List<AccountLv>>(CurTableData, settings));
                    break;
                case "AccountProfileIcon":
                    Result.Add(eTableType.AccountProfileIcon, JsonConvert.DeserializeObject<List<AccountProfileIcon>>(CurTableData, settings));
                    break;
                case "ContentsLock":
                    Result.Add(eTableType.ContentsLock, JsonConvert.DeserializeObject<List<ContentsLock>>(CurTableData, settings));
                    break;
                case "Tutorial":
                    Result.Add(eTableType.Tutorial, JsonConvert.DeserializeObject<List<Tutorial>>(CurTableData, settings));
                    break;
                case "ChapInfo":
                    Result.Add(eTableType.ChapInfo, JsonConvert.DeserializeObject<List<ChapInfo>>(CurTableData, settings));
                    break;
                case "ChapProfile":
                    Result.Add(eTableType.ChapProfile, JsonConvert.DeserializeObject<List<ChapProfile>>(CurTableData, settings));
                    break;
                case "ChapClearReward":
                    Result.Add(eTableType.ChapClearReward, JsonConvert.DeserializeObject<List<ChapClearReward>>(CurTableData, settings));
                    break;
                case "ChapPlace":
                    Result.Add(eTableType.ChapPlace, JsonConvert.DeserializeObject<List<ChapPlace>>(CurTableData, settings));
                    break;
                case "CharInfo":
                    Result.Add(eTableType.CharInfo, JsonConvert.DeserializeObject<List<CharInfo>>(CurTableData, settings));
                    break;
                case "CharProfile":
                    Result.Add(eTableType.CharProfile, JsonConvert.DeserializeObject<List<CharProfile>>(CurTableData, settings));
                    break;
                case "CharLvUpStat":
                    Result.Add(eTableType.CharLvUpStat, JsonConvert.DeserializeObject<List<CharLvUpStat>>(CurTableData, settings));
                    break;
                case "CharLvUpExp":
                    Result.Add(eTableType.CharLvUpExp, JsonConvert.DeserializeObject<List<CharLvUpExp>>(CurTableData, settings));
                    break;
                case "CharPromoteInfo":
                    Result.Add(eTableType.CharPromoteInfo, JsonConvert.DeserializeObject<List<CharPromoteInfo>>(CurTableData, settings));
                    break;
                case "CharPromoteUpStat":
                    Result.Add(eTableType.CharPromoteUpStat, JsonConvert.DeserializeObject<List<CharPromoteUpStat>>(CurTableData, settings));
                    break;
                case "EventInfo":
                    Result.Add(eTableType.EventInfo, JsonConvert.DeserializeObject<List<EventInfo>>(CurTableData, settings));
                    break;
                case "EventReward":
                    Result.Add(eTableType.EventReward, JsonConvert.DeserializeObject<List<EventReward>>(CurTableData, settings));
                    break;
                case "ItemInfo":
                    Result.Add(eTableType.ItemInfo, JsonConvert.DeserializeObject<List<ItemInfo>>(CurTableData, settings));
                    break;
                case "ItemProfile":
                    Result.Add(eTableType.ItemProfile, JsonConvert.DeserializeObject<List<ItemProfile>>(CurTableData, settings));
                    break;
                case "ItemParam":
                    Result.Add(eTableType.ItemParam, JsonConvert.DeserializeObject<List<ItemParam>>(CurTableData, settings));
                    break;
                case "ItemSummonRandom":
                    Result.Add(eTableType.ItemSummonRandom, JsonConvert.DeserializeObject<List<ItemSummonRandom>>(CurTableData, settings));
                    break;
                case "ItemSummonSelect":
                    Result.Add(eTableType.ItemSummonSelect, JsonConvert.DeserializeObject<List<ItemSummonSelect>>(CurTableData, settings));
                    break;
                case "ItemSummonConfirm":
                    Result.Add(eTableType.ItemSummonConfirm, JsonConvert.DeserializeObject<List<ItemSummonConfirm>>(CurTableData, settings));
                    break;
                case "ItemLuckyBag":
                    Result.Add(eTableType.ItemLuckyBag, JsonConvert.DeserializeObject<List<ItemLuckyBag>>(CurTableData, settings));
                    break;
                case "ItemProfileIcon":
                    Result.Add(eTableType.ItemProfileIcon, JsonConvert.DeserializeObject<List<ItemProfileIcon>>(CurTableData, settings));
                    break;
                case "MissionUiMove":
                    Result.Add(eTableType.MissionUiMove, JsonConvert.DeserializeObject<List<MissionUiMove>>(CurTableData, settings));
                    break;
                case "MissionKind":
                    Result.Add(eTableType.MissionKind, JsonConvert.DeserializeObject<List<MissionKind>>(CurTableData, settings));
                    break;
                case "MissionInfo":
                    Result.Add(eTableType.MissionInfo, JsonConvert.DeserializeObject<List<MissionInfo>>(CurTableData, settings));
                    break;
                case "MissionScoreReward":
                    Result.Add(eTableType.MissionScoreReward, JsonConvert.DeserializeObject<List<MissionScoreReward>>(CurTableData, settings));
                    break;
                case "AttendRepeat":
                    Result.Add(eTableType.AttendRepeat, JsonConvert.DeserializeObject<List<AttendRepeat>>(CurTableData, settings));
                    break;
                case "AttendCumulate":
                    Result.Add(eTableType.AttendCumulate, JsonConvert.DeserializeObject<List<AttendCumulate>>(CurTableData, settings));
                    break;
                case "MonInfo":
                    Result.Add(eTableType.MonInfo, JsonConvert.DeserializeObject<List<MonInfo>>(CurTableData, settings));
                    break;
                case "MonProfile":
                    Result.Add(eTableType.MonProfile, JsonConvert.DeserializeObject<List<MonProfile>>(CurTableData, settings));
                    break;
                case "MonLvUpStat":
                    Result.Add(eTableType.MonLvUpStat, JsonConvert.DeserializeObject<List<MonLvUpStat>>(CurTableData, settings));
                    break;
                case "ObjInfo":
                    Result.Add(eTableType.ObjInfo, JsonConvert.DeserializeObject<List<ObjInfo>>(CurTableData, settings));
                    break;
                case "ObjProfile":
                    Result.Add(eTableType.ObjProfile, JsonConvert.DeserializeObject<List<ObjProfile>>(CurTableData, settings));
                    break;
                case "ShopInfo":
                    Result.Add(eTableType.ShopInfo, JsonConvert.DeserializeObject<List<ShopInfo>>(CurTableData, settings));
                    break;
                case "SkillType":
                    Result.Add(eTableType.SkillType, JsonConvert.DeserializeObject<List<SkillType>>(CurTableData, settings));
                    break;
                case "SkillEffect":
                    Result.Add(eTableType.SkillEffect, JsonConvert.DeserializeObject<List<SkillEffect>>(CurTableData, settings));
                    break;
                case "SkillInfo":
                    Result.Add(eTableType.SkillInfo, JsonConvert.DeserializeObject<List<SkillInfo>>(CurTableData, settings));
                    break;
                case "SkillProfile":
                    Result.Add(eTableType.SkillProfile, JsonConvert.DeserializeObject<List<SkillProfile>>(CurTableData, settings));
                    break;
                case "SummonInfo":
                    Result.Add(eTableType.SummonInfo, JsonConvert.DeserializeObject<List<SummonInfo>>(CurTableData, settings));
                    break;
                case "SummonDailyFree":
                    Result.Add(eTableType.SummonDailyFree, JsonConvert.DeserializeObject<List<SummonDailyFree>>(CurTableData, settings));
                    break;
                case "SummonRateGroup":
                    Result.Add(eTableType.SummonRateGroup, JsonConvert.DeserializeObject<List<SummonRateGroup>>(CurTableData, settings));
                    break;
                case "SummonCharNormalRate":
                    Result.Add(eTableType.SummonCharNormalRate, JsonConvert.DeserializeObject<List<SummonCharNormalRate>>(CurTableData, settings));
                    break;
                case "SummonCharPickRate":
                    Result.Add(eTableType.SummonCharPickRate, JsonConvert.DeserializeObject<List<SummonCharPickRate>>(CurTableData, settings));
                    break;
                case "SummonCharDuplicateReward":
                    Result.Add(eTableType.SummonCharDuplicateReward, JsonConvert.DeserializeObject<List<SummonCharDuplicateReward>>(CurTableData, settings));
                    break;
                default:
                    throw new ArgumentException("Invalid TableType");
            }
        }

        return Result;
    }
}

public class BaseConfig : GameTable
{
    public string ConfigKey { get; private set; }
    public int ValueInt { get; private set; }
    public float ValueFloat { get; private set; }
    public string ValueStr { get; private set; }
}

public class AccountLv : GameTable
{
    public int AccountLvKey { get; private set; }
    public int LvUpExp { get; private set; }
    public int MaxStamina { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
}

public class AccountProfileIcon : GameTable
{
    public string AccountIconKey { get; private set; }
    public string AccountIcon { get; private set; }
    public string AccountIconName { get; private set; }
    public string AccountIconExpl { get; private set; }
}

public class ContentsLock : GameTable
{
    public string ContentsLockKey { get; private set; }
    public string Lock1Type { get; private set; }
    public string Lock1Count { get; private set; }
}

public class Tutorial : GameTable
{
    public string TutoKey { get; private set; }
    public int TutoGroup { get; private set; }
    public int SaveCheck { get; private set; }
    public string VrPos { get; private set; }
    public string HrPos { get; private set; }
    public string TutoStr { get; private set; }
}

public class ChapInfo : GameTable
{
    public string ChapKey { get; private set; }
    public string ChapType { get; private set; }
    public int ChapOrder { get; private set; }
    public int CousumeStamina { get; private set; }
    public int RevivalCoinConsume { get; private set; }
    public int PlatformTotalNumber { get; private set; }
}

public class ChapProfile : GameTable
{
    public string ChapKey { get; private set; }
    public string ChapName { get; private set; }
    public string ChapExpl { get; private set; }
    public string ChapPrefeb { get; private set; }
}

public class ChapClearReward : GameTable
{
    public string ChapKey { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
    public string Reward2 { get; private set; }
    public int Reward2Count { get; private set; }
    public string Mark1Reward { get; private set; }
    public int Mark1Count { get; private set; }
    public string Mark2Reward { get; private set; }
    public int Mark2Count { get; private set; }
    public string Mark3Reward { get; private set; }
    public int Mark3Count { get; private set; }
}

public class ChapPlace : GameTable
{
    public string ChapKey { get; private set; }
    public int VrPos { get; private set; }
    public string HrPos { get; private set; }
    public string ObjKey { get; private set; }
    public int ObjCount { get; private set; }
    public string MonKey { get; private set; }
    public int MonLv { get; private set; }
    public float SkillDamageLv { get; private set; }
    public int IsBoss { get; private set; }
    public int MonSummonCount { get; private set; }
}

public class CharInfo : GameTable
{
    public string CharKey { get; private set; }
    public int CharGroup { get; private set; }
    public string CharGrade { get; private set; }
    public float CharSize { get; private set; }
    public string Skill1 { get; private set; }
    public string Skill2 { get; private set; }
    public int Cp { get; private set; }
    public int Hp { get; private set; }
    public int Atk { get; private set; }
    public int Def { get; private set; }
    public float Cri1Rate { get; private set; }
    public float Cri1Dam { get; private set; }
    public float Spd { get; private set; }
    public float ItemGetDis { get; private set; }
    public float HealPercent { get; private set; }
}

public class CharProfile : GameTable
{
    public string CharKey { get; private set; }
    public string CharName { get; private set; }
    public string CharExpl { get; private set; }
    public string CharPrefab { get; private set; }
    public string CharIcon { get; private set; }
    public string CharIllust { get; private set; }
}

public class CharLvUpStat : GameTable
{
    public string CharKey { get; private set; }
    public int Hp { get; private set; }
    public int Atk { get; private set; }
    public int Def { get; private set; }
    public float Cri1Rate { get; private set; }
    public float Cri1Dam { get; private set; }
    public float Spd { get; private set; }
    public float ItemGetDis { get; private set; }
    public float HealPercent { get; private set; }
}

public class CharLvUpExp : GameTable
{
    public int CharLvKey { get; private set; }
    public int CharLvExp { get; private set; }
    public string Use1 { get; private set; }
    public int Use1Count { get; private set; }
}

public class CharPromoteInfo : GameTable
{
    public string CharGradeKey { get; private set; }
    public int CharMaxLv { get; private set; }
    public int Promote1StarNeedCount { get; private set; }
    public string Use1 { get; private set; }
    public int Use1Count { get; private set; }
    public int Promote2StarNeedCount { get; private set; }
    public string Use2 { get; private set; }
    public int Use2Count { get; private set; }
    public int Promote3StarNeedCount { get; private set; }
    public string Use3 { get; private set; }
    public int Use3Count { get; private set; }
}

public class CharPromoteUpStat : GameTable
{
    public string CharKey { get; private set; }
    public int Hp { get; private set; }
    public int Atk { get; private set; }
    public int Def { get; private set; }
    public float Cri1Rate { get; private set; }
    public float Cri1Dam { get; private set; }
    public float Spd { get; private set; }
    public float ItemGetDis { get; private set; }
    public float HealPercent { get; private set; }
}

public class EventInfo : GameTable
{
    public string EventKey { get; private set; }
    public int IsView { get; private set; }
    public int EventOrder { get; private set; }
    public string StartDate { get; private set; }
    public string EndDate { get; private set; }
    public string EventPrefab { get; private set; }
    public string UiMoveKey { get; private set; }
}

public class EventReward : GameTable
{
    public string EventKey { get; private set; }
    public string EventType { get; private set; }
    public string Reward1 { get; private set; }
    public string Reward1Count { get; private set; }
    public string Reward2 { get; private set; }
    public string Reward2Count { get; private set; }
}

public class ItemInfo : GameTable
{
    public string ItemKey { get; private set; }
    public string GradeType { get; private set; }
    public int ItemMaxCount { get; private set; }
    public string ItemType { get; private set; }
    public int GroupIdx { get; private set; }
    public int InvenView { get; private set; }
    public int UseView { get; private set; }
    public int RateInfoView { get; private set; }
}

public class ItemProfile : GameTable
{
    public string ItemKey { get; private set; }
    public string ItemName { get; private set; }
    public string ItemExpl { get; private set; }
    public string ItemPrefab { get; private set; }
    public string ItemIcon { get; private set; }
}

public class ItemParam : GameTable
{
    public string ItemKey { get; private set; }
    public int ItemParam1 { get; private set; }
}

public class ItemSummonRandom : GameTable
{
    public string ItemKey { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
    public int Summon1Rate { get; private set; }
}

public class ItemSummonSelect : GameTable
{
    public string ItemKey { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
}

public class ItemSummonConfirm : GameTable
{
    public string ItemKey { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
}

public class ItemLuckyBag : GameTable
{
    public string ItemKey { get; private set; }
    public int LuckyBagRate { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
}

public class ItemProfileIcon : GameTable
{
    public string ItemKey { get; private set; }
    public string CharKey { get; private set; }
}

public class MissionUiMove : GameTable
{
    public string UiMoveKey { get; private set; }
}

public class MissionKind : GameTable
{
    public string MissionKey { get; private set; }
    public string MissionName { get; private set; }
    public string UiMoveKey { get; private set; }
}

public class MissionInfo : GameTable
{
    public string MissionInfoKey { get; private set; }
    public string ResetDay { get; private set; }
    public string MissionKey { get; private set; }
    public int Cond1 { get; private set; }
    public int MissionOrder { get; private set; }
    public int MissionView { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
}

public class MissionScoreReward : GameTable
{
    public string MissionScoreKey { get; private set; }
    public string ResetDay { get; private set; }
    public int MissionScore { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
    public string Reward2 { get; private set; }
    public int Reward2Count { get; private set; }
}

public class AttendRepeat : GameTable
{
    public string AttendRepeatKey { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
}

public class AttendCumulate : GameTable
{
    public string AttendCumulateKey { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
    public string Reward2 { get; private set; }
    public int Reward2Count { get; private set; }
}

public class MonInfo : GameTable
{
    public string MonKey { get; private set; }
    public int MonGroup { get; private set; }
    public string MonGrade { get; private set; }
    public float MonSize { get; private set; }
    public string MonMoveType { get; private set; }
    public string Skill1 { get; private set; }
    public string Skill2 { get; private set; }
    public string Skill3 { get; private set; }
    public string Skill4 { get; private set; }
    public int Skill1Rate { get; private set; }
    public int Skill2Rate { get; private set; }
    public int Skill3Rate { get; private set; }
    public int Skill4Rate { get; private set; }
    public int Cp { get; private set; }
    public int Hp { get; private set; }
    public int Atk { get; private set; }
    public int Def { get; private set; }
    public float Spd { get; private set; }
    public float Cri1Rate { get; private set; }
    public float Cri1Dam { get; private set; }
}

public class MonProfile : GameTable
{
    public string MonKey { get; private set; }
    public string MonName { get; private set; }
    public string MonExpl { get; private set; }
    public string MonPrefab { get; private set; }
    public string MonIcon { get; private set; }
    public string MonIllust { get; private set; }
}

public class MonLvUpStat : GameTable
{
    public string MonKey { get; private set; }
    public int Cp { get; private set; }
    public int Hp { get; private set; }
    public int Atk { get; private set; }
    public int Def { get; private set; }
    public float Spd { get; private set; }
    public float Cri1Rate { get; private set; }
    public float Cri1Dam { get; private set; }
}

public class ObjInfo : GameTable
{
    public string ObjKey { get; private set; }
    public string SkillEffect1 { get; private set; }
    public float SkillEffect1Count { get; private set; }
    public string SkillEffect2 { get; private set; }
    public float SkillEffect2Count { get; private set; }
    public string SkillEffect3 { get; private set; }
    public float SkillEffect3Count { get; private set; }
}

public class ObjProfile : GameTable
{
    public string ObjKey { get; private set; }
    public string ObjName { get; private set; }
    public string ObjExpl { get; private set; }
    public string ObjPrefab { get; private set; }
    public string ObjIcon { get; private set; }
}

public class ShopInfo : GameTable
{
    public string ShopKey { get; private set; }
    public string ShopType { get; private set; }
    public int ShopIdx { get; private set; }
    public string ResetDay { get; private set; }
    public int SoldCount { get; private set; }
    public string AdResetDay { get; private set; }
    public int AdCount { get; private set; }
    public string LockType { get; private set; }
    public string ChapKey { get; private set; }
    public string Use1 { get; private set; }
    public int Use1Count { get; private set; }
    public string Reward1 { get; private set; }
    public int Reward1Count { get; private set; }
    public string Reward2 { get; private set; }
    public int Reward2Count { get; private set; }
    public string Reward3 { get; private set; }
    public int Reward3Count { get; private set; }
    public string ProductName { get; private set; }
    public string ProductExpl { get; private set; }
    public string ProductPrefab { get; private set; }
}

public class SkillType : GameTable
{
    public string SkillTypeKey { get; private set; }
}

public class SkillEffect : GameTable
{
    public string SkillEffectKey { get; private set; }
}

public class SkillInfo : GameTable
{
    public string SkillKey { get; private set; }
    public int SkillGroup { get; private set; }
    public string SkillType { get; private set; }
    public float SkillAtk { get; private set; }
    public float SkillCooltime { get; private set; }
    public float SkillMaintain { get; private set; }
    public float SkillSize { get; private set; }
    public int SkillDirect { get; private set; }
    public float SkillSpd { get; private set; }
    public float SkillInterval { get; private set; }
    public int SkillCount { get; private set; }
    public string SkillEffect1 { get; private set; }
    public float Effect1Count { get; private set; }
    public string SkillEffect2 { get; private set; }
    public float Effect2Count { get; private set; }
    public string SkillEffect3 { get; private set; }
    public float Effect3Count { get; private set; }
    public string SkillEffect4 { get; private set; }
    public float Effect4Count { get; private set; }
    public string SeriesEffect { get; private set; }
}

public class SkillProfile : GameTable
{
    public string SkillKey { get; private set; }
    public string SkillName { get; private set; }
    public string SkillExpl { get; private set; }
    public string SkillPrefab { get; private set; }
    public string SkillIcon { get; private set; }
}

public class SummonInfo : GameTable
{
    public string SummonKey { get; private set; }
    public string SummonType { get; private set; }
    public int SummonOrder { get; private set; }
    public int SummonView { get; private set; }
    public string SummonName { get; private set; }
    public string SummonPrefab { get; private set; }
    public string StartDate { get; private set; }
    public string EndDate { get; private set; }
    public string Use1 { get; private set; }
    public int Use1One { get; private set; }
    public int Use1Ten { get; private set; }
    public string Use2 { get; private set; }
    public int Use2One { get; private set; }
    public int Use2Ten { get; private set; }
}

public class SummonDailyFree : GameTable
{
    public string SummonKey { get; private set; }
    public string ResetDay { get; private set; }
    public string ProvideType { get; private set; }
    public int ProvideCount { get; private set; }
}

public class SummonRateGroup : GameTable
{
    public string SummonGroupKey { get; private set; }
    public string SummonKey { get; private set; }
    public string GradeType { get; private set; }
    public int GroupRate { get; private set; }
}

public class SummonCharNormalRate : GameTable
{
    public string SummonKey { get; private set; }
    public string CharKey { get; private set; }
    public string ItemKey { get; private set; }
    public string GradeType { get; private set; }
    public int AppearCount { get; private set; }
    public int SummonRate { get; private set; }
}

public class SummonCharPickRate : GameTable
{
    public string SummonKey { get; private set; }
    public string CharKey { get; private set; }
    public string ItemKey { get; private set; }
    public string GradeType { get; private set; }
    public int AppearCount { get; private set; }
    public int SummonRate { get; private set; }
}

public class SummonCharDuplicateReward : GameTable
{
    public string CharKey { get; private set; }
    public int CharMaxHave { get; private set; }
    public string MaxReward1 { get; private set; }
    public int MaxReward1Count { get; private set; }
}