using System;
using System.Collections.Generic;

public enum eUserData_Hero
{
    HeroCollection,
    HeroList,
    HeroUpgradeItemList,

    Max
}

public class UserData_Hero
{
    public string _id = string.Empty;
    public string AccountCode = string.Empty;

    public Dictionary<string, UserHeroCollection> HeroCollection;
    public List<UserHero> HeroList;
    public List<UserHero> HeroUpgradeItemList;
}

public class UserHero
{
    public HeroUniqueData Unique;
    public int Type;
    public int Exp;
    public int Grade;
    public int Group;
    public int Lv;
    public int CP;
    public int IdleSkillLv;
    public bool IsLock;

    public static UserHero Clone(UserHero HeroData)
    {
        UserHero CloneData = new UserHero();
        CloneData.Unique = HeroData.Unique;
        CloneData.Group = HeroData.Group;
        CloneData.Type = HeroData.Type;
        CloneData.Exp = HeroData.Exp;
        CloneData.Grade = HeroData.Grade;
        CloneData.Lv = HeroData.Lv;
        CloneData.CP = HeroData.CP;
        CloneData.IdleSkillLv = HeroData.IdleSkillLv;
        CloneData.IsLock = HeroData.IsLock;
        return CloneData;
    }
}

public class UserHeroCollection
{
    public int HeroKey;
    public int Type;
    public bool IsOpen;
    public DateTime IsOpenTime;
    public bool IsReward;
    public int GetCount;
}

public class HeroUniqueData
{
    public int HeroKey;
    public int Count;

    public HeroUniqueData(int HeroKey, int Count)
    {
        this.HeroKey = HeroKey;
        this.Count = Count;
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        HeroUniqueData other = (HeroUniqueData)obj;
        return HeroKey == other.HeroKey && Count == other.Count;
    }

    public override int GetHashCode()
    {
        return (HeroKey, Count).GetHashCode();
    }
}
