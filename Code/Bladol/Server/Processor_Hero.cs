using GameServer.Module.ServerManager.Contents;
using GameServer.Module.LogManager;

namespace GameServer.Module.ServerManager.Processors
{
    public partial class Processor
    {
        public static string ProcessPacket_Hero(string AccountCode, GamePacket PacketData, out PacketState packetState)
        {
            packetState = PacketState.None;

            if (PacketData.contentsType != ContentsType.Hero)
            {
                packetState = PacketState.UnknownPacket;
                return ServerUtil.MakeUnkownErrorData(PacketType.ContentsPacket, ContentsType.Hero, PacketData.contentsType, PacketData.HeaderData.ContentsIndex);
            }

            PacketHeader outHeaderData;
            List<PacketBody> outBodyData = new List<PacketBody>();
            List<LogData> outLogData = new List<LogData>();

            HeroContents Type = (HeroContents)PacketData.HeaderData.ContentsIndex;
            switch (Type)
            {
                case HeroContents.ExclusiveWeaponGradeUp:
                    {
                        int HeroIndex = int.Parse(PacketData.HeaderData.Data);

                        if(HeroIndex == 0)
                        {
                            // 잘못된 데이터
                            outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponGradeUp, false);
                            outBodyData.Add(ServerUtil.MakeMessage(MessageType.Message, "ServerMessage_034"));
                            return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                        }

                        var UserHeroList = HeroMethod.GetUserHeroList(AccountCode);
                        var HeroData = UserHeroList[$"{HeroIndex}"];

                        if (HeroData.ExclusiveWeapon.StarGrade == GetHeroExclusiveWeaponMaxGrade(HeroIndex))
                        {
                            // 이미 만랩이다.
                            outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponGradeUp, false);
                            outBodyData.Add(ServerUtil.MakeMessage(MessageType.Toast, "notice_0180"));
                            return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                        }

                        var MaterialList = GetHeroExclusiveWeaponMaterial(AccountCode, HeroIndex);

                        if(MaterialList == null)
                        {
                            // 잘못된 데이터
                            outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponGradeUp, false);
                            outBodyData.Add(ServerUtil.MakeMessage(MessageType.Message, "ServerMessage_034"));
                            return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                        }

                        var UserInventoryData = InventoryMethod.GetUserInventoryData(AccountCode);

                        for(int count = 0; count < MaterialList.Count; count++)
                        {
                            switch (MaterialList[count].Type)
                            {
                                case eItemType.Currency:
                                    {
                                        switch ((eGoodsType)MaterialList[count].Index)
                                        {
                                            case eGoodsType.Gold:
                                                {
                                                    int CurrentGold = GoodsMethod.GetUserGoodsData<int>(AccountCode, eUserData_Goods.Gold);

                                                    if (CurrentGold < MaterialList[count].Count)
                                                    {
                                                        outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponGradeUp, false);
                                                        outBodyData.Add(ServerUtil.MakeMessage(MessageType.Toast, "ServerMessage_007"));
                                                        return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                                                    }
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case eItemType.ExpPotion:
                                case eItemType.ReinforceStone:
                                case eItemType.TimeDimensionPostion:
                                case eItemType.ClearCard:
                                case eItemType.SummonTicket:
                                case eItemType.NotUse:
                                case eItemType.PreferenceItem:
                                case eItemType.Consensus:
                                case eItemType.Exclusive:
                                case eItemType.EventItem:
                                    {
                                        string ItemKey = GetItemKey(MaterialList[count].Type, MaterialList[count].Index);
                                        if (UserInventoryData.ConsumeItemList.ContainsKey(ItemKey))
                                        {
                                            if (UserInventoryData.ConsumeItemList[ItemKey] < MaterialList[count].Count)
                                            {
                                                // 재료 부족
                                                outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponGradeUp, false);
                                                outBodyData.Add(ServerUtil.MakeMessage(MessageType.Toast, "notice_0178"));
                                                return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                                            }
                                        }
                                        else
                                        {
                                            // 재료부족
                                            outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponGradeUp, false);
                                            outBodyData.Add(ServerUtil.MakeMessage(MessageType.Toast, "message_item_0004"));
                                            return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                                        }
                                    }
                                    break;
                            }
                        }

                        int PrevStar = HeroData.ExclusiveWeapon.StarGrade;
                        HeroData.ExclusiveWeapon.StarGrade++;
                        if(HeroData.ExclusiveWeapon.StarGrade == 1)
                        {
                            HeroData.ExclusiveWeapon.Level = 1;
                        }

                        HeroData.CombatPower = Calculator.GetLongTotalCombatPower(HeroData);
                        UserHeroList[$"{HeroIndex}"].CombatPower = HeroData.CombatPower;

                        long BestCombatPower = UserMethod.GetUserCommonData<long>(AccountCode, eUserData_Common.BestCombatPower);
                        long TotalCombatPower = 0;
                        foreach (var Hero in UserHeroList)
                        {
                            if (Hero.Value.IsOpen)
                                TotalCombatPower += Hero.Value.CombatPower;
                        }

                        if (BestCombatPower < HeroData.CombatPower)
                            outBodyData.Add(UserMethod.ProcessUserCommonData(AccountCode, eUserData_Common.BestCombatPower, HeroData.CombatPower, ref outLogData));

                        outBodyData.Add(UserMethod.ProcessUserCommonData(AccountCode, eUserData_Common.TotalCombatPower, TotalCombatPower, ref outLogData));
                        EventRankingMethod.ProcessUserEventRankingData(AccountCode, TotalCombatPower);

                        outBodyData.Add(HeroMethod.ProcessUserHeroItem(AccountCode, HeroData, ref outLogData));

                        for (int count = 0; count < MaterialList.Count; count++)
                        {
                            CommonMethod.ProcessAddItemData(AccountCode, MaterialList[count].Type, MaterialList[count].Index, MaterialList[count].Count * -1, PacketData.platformType, ref outBodyData, ref outLogData);
                        }

                        ServerLog.SetContentsLog(AccountCode, ServerUtil.GetEnumFullName(HeroContents.ExclusiveWeaponGradeUp), outLogData);

                        var Data = ServerUtil.MakeData($"{HeroData.Index}", $"{PrevStar}",$"{HeroData.ExclusiveWeapon.StarGrade}");
                        outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponGradeUp, true, Data);

                        return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                    }
                case HeroContents.ExclusiveWeaponLevelUp:
                    {
                        List<string> RecvData = ServerUtil.ToObjectJson<List<string>>(PacketData.HeaderData.Data);

                        int HeroIndex = int.Parse(RecvData[0]);

                        List<int[]> StoneInfo = null;
                        StoneInfo = ServerUtil.ToObjectJson<List<int[]>>(RecvData[1]);

                        var HeroData = HeroMethod.GetUserHeroItem(AccountCode, HeroIndex);
                        var MaxLevel = GetHeroExclusiveWeaponMaxLevel(AccountCode, HeroIndex);

                        if (HeroData.StarGrade < 5 | HeroData.ExclusiveWeapon.StarGrade == 0 | HeroData.ExclusiveWeapon.Level == 0)
                        {
                            // 무기가 오픈 안되었다.
                            outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponLevelUp, false);
                            outBodyData.Add(ServerUtil.MakeMessage(MessageType.Toast, "notice_0179"));
                            return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                        }

                        if (HeroData.ExclusiveWeapon.Level >= MaxLevel)
                        {
                            // 이미 만랩이다.
                            outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponLevelUp, false);
                            outBodyData.Add(ServerUtil.MakeMessage(MessageType.Toast, "notice_0165"));
                            return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                        }

                        int TotalPrice = 0;
                        int TotalGaneExp = 0;
                        List<ItemTypeData> ConsumeList = new List<ItemTypeData>();

                        // 강화석 정보가 없다
                        if (StoneInfo.Count == 0)
                        {
                            outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponLevelUp, false);
                            outBodyData.Add(ServerUtil.MakeMessage(MessageType.Message, "ServerMessage_018"));
                            return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                        }

                        bool IsError = false;
                        var UserInventory = InventoryMethod.GetUserInventoryData(AccountCode);
                        for (int count = 0; count < StoneInfo.Count; ++count)
                        {
                            eItemType ItemType = eItemType.TimeDimensionPostion;
                            int ItemIndex = StoneInfo[count][0];
                            int ItemCount = StoneInfo[count][1];
                            string ItemKey = GetItemKey(eItemType.TimeDimensionPostion, ItemIndex);

                            if (!UserInventory.ConsumeItemList.ContainsKey(ItemKey) || UserInventory.ConsumeItemList[ItemKey] < ItemCount)
                            {
                                IsError = true;
                                break;
                            }

                            var StoneTable = DataManager.GetTable<ItemConsume>(TableType.ItemConsume).Values.Where(Table => Table.ItemKey == ItemKey).SingleOrDefault();

                            TotalGaneExp += StoneTable.Parameter_1 * ItemCount;
                            TotalPrice += StoneTable.Parameter_2 * ItemCount;
                            ConsumeList.Add(ItemTypeData.Make(ItemType, ItemKey, ItemIndex, ItemCount));
                        }

                        // 클라이언트에서 보내온 갯수가 많음
                        if (IsError)
                        {
                            outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponLevelUp, false);
                            outBodyData.Add(ServerUtil.MakeMessage(MessageType.Message, "ServerMessage_019"));
                            return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                        }

                        int CurrentGold = GoodsMethod.GetUserGoodsData<int>(AccountCode, eUserData_Goods.Gold);

                        // 골드 부족
                        if (CurrentGold < TotalPrice)
                        {
                            outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponLevelUp, false);
                            outBodyData.Add(ServerUtil.MakeMessage(MessageType.Message, "ServerMessage_007"));
                            return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                        }
                        
                        int NewExp = HeroData.ExclusiveWeapon.Exp + TotalGaneExp;
                        int NewLevel = GetHeroExclusiveWeaponLevelByExp(NewExp, MaxLevel);
                        if(NewLevel >= MaxLevel)
                        {
                            NewLevel = MaxLevel;
                            NewExp = GetHeroExclusiveWeaponNeedExp(MaxLevel);
                        }
                        int AddNewLevel = NewLevel - HeroData.ExclusiveWeapon.Level;
                        int PrevStar = HeroData.ExclusiveWeapon.Level;
                        bool IsGradeUp = HeroData.ExclusiveWeapon.Level < NewLevel;

                        var UserHeroData = HeroMethod.GetUserHeroData(AccountCode);

                        HeroData.ExclusiveWeapon.Exp = NewExp;
                        HeroData.ExclusiveWeapon.Level = NewLevel;
                        HeroData.CombatPower = Calculator.GetLongTotalCombatPower(HeroData);
                        UserHeroData.HeroList[$"{HeroIndex}"] = HeroData;

                        // 최종 전투력 계산
                        long BestCombatPower = UserMethod.GetUserCommonData<long>(AccountCode, eUserData_Common.BestCombatPower);
                        long TotalCombatPower = 0;
                        foreach (var Hero in UserHeroData.HeroList)
                        {
                            if (Hero.Value.IsOpen)
                                TotalCombatPower += Hero.Value.CombatPower;
                        }

                        if (BestCombatPower < HeroData.CombatPower)
                            outBodyData.Add(UserMethod.ProcessUserCommonData(AccountCode, eUserData_Common.BestCombatPower, HeroData.CombatPower, ref outLogData));

                        outBodyData.Add(UserMethod.ProcessUserCommonData(AccountCode, eUserData_Common.TotalCombatPower, TotalCombatPower, ref outLogData));
                        EventRankingMethod.ProcessUserEventRankingData(AccountCode, TotalCombatPower);
                        
                        for (int count = 0; count < ConsumeList.Count; ++count)
                            CommonMethod.ProcessAddItemData(AccountCode, ConsumeList[count].Type, ConsumeList[count].Index, ConsumeList[count].Count * -1, PacketData.platformType, ref outBodyData, ref outLogData);

                        CommonMethod.ProcessAddItemData( AccountCode,eItemType.Currency,eGoodsType.Gold.GetHashCode(),TotalPrice * -1, PacketData.platformType, ref outBodyData, ref outLogData);

                        outBodyData.Add(HeroMethod.ProcessUserHeroItem(AccountCode, HeroData, ref outLogData));
                        outBodyData.Add(UserMethod.ProcessUserCommonData(AccountCode, eUserData_Common.LastConnectTime, ServerUtil.DateTimeNow, ref outLogData));

                        ServerLog.SetContentsLog(AccountCode, ServerUtil.GetEnumFullName(HeroContents.ExclusiveWeaponLevelUp), outLogData);

                        outHeaderData = ServerUtil.MakeHeaderData(HeroContents.ExclusiveWeaponLevelUp, true, $"{HeroIndex}#{IsGradeUp}#{PrevStar}#{NewLevel}");
                        return ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                    }
            }

            packetState = PacketState.UnknownPacket;
            return ServerUtil.MakeUnkownErrorData(PacketType.ContentsPacket, ContentsType.Hero, PacketData.contentsType, PacketData.HeaderData.ContentsIndex);
        }
    }
}