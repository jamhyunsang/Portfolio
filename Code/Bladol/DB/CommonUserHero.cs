using System;
using System.Collections.Generic;

public enum eUserData_Hero
{
    HeroList,

    Max
}

public enum eUserHero
{
    Index,
    HeroLv,
    HeroExp,
    RelationLv,
    RelationExp,
    StarGrade,
    Rank,
    CombatPower,
    StarGradeExp,
    IsOpen,
    Equipment,
    SkillLv,
    Skin,
    IsReward,
    ConsensusRate,
    IsConsensus,
    ExclusiveWeapon,

    Max
}

public enum eUserHeroEquip
{
    EquipKey,
    Exp,
    StarGrade,

    Max
}

public class UserData_Hero
{
    public string _id = string.Empty;
    public string AccountCode = string.Empty;
    public Dictionary<string, UserHero> HeroList;
}

public class UserHero
{
    public int Index = 0;

    public int HeroLv = 1;
    public int HeroExp = 0;

    public int RelationLv = 1;
    public int RelationExp = 0;

    public int StarGrade = 1;
    public int Rank = 1;
    public long CombatPower = 0;

    public int StarGradeExp = 0;

    public bool IsOpen = false;
    public bool IsReward = false;

    public int ConsensusRate = 0;
    public bool IsConsensus = false;

    public Dictionary<string, UserHeroEquip> Equipment;
    public Dictionary<string, int> SkillLv;
    public List<UserHeroSkin> Skin;

    public UserHeroExclusiveWeapon ExclusiveWeapon;

    public static UserHero Clone(UserHero Data)
    {
        UserHero Clone = new UserHero();
        Clone.Index = Data.Index;
        Clone.HeroLv = Data.HeroLv;
        Clone.HeroExp = Data.HeroExp;
        Clone.RelationLv = Data.RelationLv;
        Clone.RelationExp = Data.RelationExp;
        Clone.StarGrade = Data.StarGrade;
        Clone.Rank = Data.Rank;
        Clone.CombatPower = Data.CombatPower;
        Clone.StarGradeExp = Data.StarGradeExp;
        Clone.IsOpen = Data.IsOpen;
        Clone.Equipment = new Dictionary<string, UserHeroEquip>(Data.Equipment);
        Clone.SkillLv = new Dictionary<string, int>(Data.SkillLv);
        Clone.Skin = new List<UserHeroSkin>(Data.Skin);
        Clone.ConsensusRate = Data.ConsensusRate;
        Clone.IsConsensus = Data.IsConsensus;
        Clone.ExclusiveWeapon = UserHeroExclusiveWeapon.Clone(Data.ExclusiveWeapon);
        return Clone;
    }
}

public class UserHeroEquip
{
    public string EquipKey = string.Empty;
    public int Exp = 0;
    public int StarGrade = 0;

    public static UserHeroEquip Clone(UserHeroEquip Data)
    {
        UserHeroEquip Clone = new UserHeroEquip();
        Clone.EquipKey = Data.EquipKey;
        Clone.Exp = Data.Exp;
        Clone.StarGrade = Data.StarGrade;

        return Clone;
    }
}

public class UserHeroSkin
{
    public string SkinKey = string.Empty;
    public bool IsOpen = false;

    public static UserHeroSkin Clone(UserHeroSkin Data)
    {
        UserHeroSkin Clone = new UserHeroSkin();
        Clone.SkinKey = Data.SkinKey;
        Clone.IsOpen = Data.IsOpen;

        return Clone;
    }
}

public class UserHeroExclusiveWeapon
{
    public int Exp = 0;
    public int Level = 0;
    public int StarGrade = 0;

    public static UserHeroExclusiveWeapon Clone(UserHeroExclusiveWeapon Data)
    {
        UserHeroExclusiveWeapon Clone = new UserHeroExclusiveWeapon();
        Clone.Exp = Data.Exp;
        Clone.Level = Data.Level;
        Clone.StarGrade += Data.StarGrade;

        return Clone;
    }
}