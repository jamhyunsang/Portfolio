using GameServer.Module.ServerManager.Contents;
using GameServer.Module.LogManager;

namespace GameServer.Module.ServerManager.Processors
{
    public partial class Processor
    {
        public static async Task<Tuple<PacketState, string>> ProcessPacket_Shop(string AccountCode, GamePacket PacketData)
        {
            PacketState packetState = PacketState.None;
            string Result = string.Empty;

            if (PacketData.contentsType != ContentsType.Shop)
            {
                packetState = PacketState.UnknownPacket;
                Result = ServerUtil.MakeUnkownErrorData(PacketType.ContentsPacket, ContentsType.Shop, PacketData.contentsType, PacketData.HeaderData.ContentsIndex);
                return new Tuple<PacketState, string>(packetState, Result);
            }

            PacketHeader outHeaderData;
            List<PacketBody> outBodyData = new List<PacketBody>();
            List<LogData> outLogData = new List<LogData>();

            ShopContents Type = (ShopContents)PacketData.HeaderData.ContentsIndex;
            switch (Type)
            {
                case ShopContents.GachaOne:
                    {
                        var Datas = PacketData.HeaderData.Data.Split("#");
                        int SummonType = int.Parse(Datas[0]);
                        int SummonUniqueKey = int.Parse(Datas[1]);
                        string MessageData = "UI_Message_0004";
                        try
                        {
                            // 컨텐츠 잠금
                            var IsContentsOpen = await CommonMethod.IsContentsOpen(AccountCode, eContentsType.Gacha);
                            if (!IsContentsOpen)
                                throw new Exception();
                            // 소환 테이블이 없다
                            var SummonInfoTable = await DataManager.GetTable<SummonInfo>(TableType.SummonInfo);
                            if (SummonInfoTable == null)
                                throw new Exception();

                            var SummonInfo = SummonInfoTable.SingleOrDefault(Table => Table.Value.Key == SummonType && Table.Value.Pickup_Unique_Key == SummonUniqueKey).Value;
                            if (SummonInfo == null)
                                throw new Exception();

                            var SummonDuplicateExpTable = await DataManager.GetTable<SummonDuplicateExp>(TableType.SummonDuplicateExp);
                            if (SummonDuplicateExpTable == null)
                                throw new Exception();

                            // 유저 히어로 데이터
                            var UserHeroData = await HeroMethod.GetUserHeroData(AccountCode);
                            if (UserHeroData == null)
                                throw new Exception();

                            // 소모 재화 체크하기, 무료 가챠라면 확인할 필요가 없다.
                            var UserDailyData = await RecordMethod.GetUserDailyRecordData(AccountCode);
                            int FreeGachaCount = 0;
                            UserDailyData.FreeGacha_One.TryGetValue($"{SummonType}", out FreeGachaCount);

                            var UserGemCount = await CommonMethod.GetItemCount(AccountCode, SummonInfo.Price_1_Key);
                            var UserCoinCount = await CommonMethod.GetItemCount(AccountCode, SummonInfo.Price_2_Key);

                            var GemCount = UserGemCount.OtherCount != -1 ? UserGemCount.Count + UserGemCount.OtherCount : UserGemCount.Count;
                            var CoinCount = UserCoinCount.OtherCount != -1 ? UserCoinCount.Count + UserCoinCount.OtherCount : UserCoinCount.Count;

                            if (FreeGachaCount <= 0)
                            {
                                if (CoinCount <= 0 && GemCount <= SummonInfo.Price_1_One)
                                {
                                    MessageData = "UI_Message_0003";
                                    throw new Exception();
                                }
                            }

                            var UserCommonData = await UserMethod.GetUserCommonData(AccountCode);
                            if (UserCommonData == null)
                            {
                                MessageData = "UI_Message_0003";
                                throw new Exception();
                            }

                            var HeroInfoTable = await DataManager.GetTable<HeroInfo>(TableType.HeroInfo);
                            if (HeroInfoTable == null)
                                throw new Exception();

                            var SuperVillainInfoTable = await DataManager.GetTable<SuperVillainInfo>(TableType.SuperVillainInfo);
                            if (HeroInfoTable == null)
                                throw new Exception();

                            var NewPick = await GetGachaResult(SummonType, SummonUniqueKey);

                            eMissionKind MissionKind = eMissionKind.None;

                            if (UserHeroData.HeroCollection.ContainsKey($"{NewPick[0]}"))
                            {
                                if (!UserHeroData.HeroCollection[$"{NewPick[0]}"].IsOpen)
                                    UserHeroData.HeroCollection[$"{NewPick[0]}"].IsOpenTime = ServerUtil.DateTimeNow;

                                UserHeroData.HeroCollection[$"{NewPick[0]}"].IsOpen = true;
                                UserHeroData.HeroCollection[$"{NewPick[0]}"].GetCount++;
                            }
                            else
                            {
                                UserHeroCollection HeroCollection = new UserHeroCollection();
                                HeroCollection.HeroKey = NewPick[0];
                                HeroCollection.Type = NewPick[1];
                                HeroCollection.IsOpen = true;
                                HeroCollection.IsOpenTime = ServerUtil.DateTimeNow;
                                HeroCollection.IsReward = false;
                                HeroCollection.GetCount = 1;
                                UserHeroData.HeroCollection.Add($"{NewPick[0]}", HeroCollection);
                            }

                            UserHero heroData = new UserHero();

                            switch ((eHeroType)NewPick[1])
                            {
                                case eHeroType.Hero:
                                    {
                                        var HeroInfo = HeroInfoTable.Values.Where(Data => Data.Key == NewPick[0]).SingleOrDefault();

                                        heroData.Unique = new HeroUniqueData(NewPick[0], UserHeroData.HeroCollection[$"{NewPick[0]}"].GetCount);
                                        heroData.Group = HeroInfo.Group;
                                        heroData.Type = eHeroType.Hero.GetHashCode();
                                        heroData.Exp = 0;
                                        heroData.Lv = 1;
                                        heroData.Grade = HeroInfo.Grade;
                                        var cp = await Calculator.GetLongTotalCombatPower(heroData);
                                        heroData.CP = (int)cp;
                                        heroData.IdleSkillLv = 1;

                                        MissionKind = eMissionKind.Get_Hero;
                                    }
                                    break;
                                case eHeroType.SuperVillain:
                                    {
                                        var SuperVillainInfo = SuperVillainInfoTable.Values.Where(Data => Data.Key == NewPick[0]).SingleOrDefault();

                                        heroData.Unique = new HeroUniqueData(NewPick[0], UserHeroData.HeroCollection[$"{NewPick[0]}"].GetCount);
                                        heroData.Group = SuperVillainInfo.Group;
                                        heroData.Type = eHeroType.SuperVillain.GetHashCode();
                                        heroData.Exp = 0;
                                        heroData.Lv = 1;
                                        heroData.Grade = SuperVillainInfo.Grade;
                                        var cp = await Calculator.GetLongTotalCombatPower(heroData);
                                        heroData.CP = (int)cp;
                                        heroData.IdleSkillLv = 1;

                                        MissionKind = eMissionKind.Get_SuperVillain;
                                    }
                                    break;
                            }

                            UserHeroData.HeroList.Add(heroData);

                            var MissionResult = await MissionMethod.ProcessCheckMission(AccountCode, MissionKind, NewPick[0]);
                            outBodyData.AddRange(MissionResult.Item1);
                            outLogData.AddRange(MissionResult.Item2);

                            GachaResultData GachaResult = GachaResultData.Make(heroData.Unique, NewPick[0], NewPick[1], heroData.Grade);

                            long TotalCombatPower = 0;

                            foreach (var Hero in UserHeroData.HeroList)
                            {
                                long CombatPower = await Calculator.GetLongTotalCombatPower(Hero);
                                TotalCombatPower += CombatPower;
                            }

                            UserCommonData.TotalCombatPower = TotalCombatPower;
                            UserCommonData.LastReinforceTime = ServerUtil.DateTimeNow;

                            var HeroResult = await HeroMethod.ProcessUserHeroData(AccountCode, UserHeroData);
                            outBodyData.Add(HeroResult.Item1);
                            outLogData.Add(HeroResult.Item2);

                            var UserResult = await UserMethod.ProcessUserCommonData(AccountCode, UserCommonData);
                            outBodyData.Add(UserResult.Item1);
                            outLogData.Add(UserResult.Item2);

                            // 무료 가챠가 아니라면 재화 소모
                            if (FreeGachaCount <= 0)
                            {
                                if (CoinCount > 0)
                                {
                                    var PriceResult = await CommonMethod.ProcessAddItemData(AccountCode, SummonInfo.Price_2_Key, SummonInfo.Price_2_One * -1);
                                    outBodyData.AddRange(PriceResult.Item1);
                                    outLogData.AddRange(PriceResult.Item2);
                                }
                                else
                                {
                                    var PriceResult = await CommonMethod.ProcessAddItemData(AccountCode, SummonInfo.Price_1_Key, SummonInfo.Price_1_One * -1);
                                    outBodyData.AddRange(PriceResult.Item1);
                                    outLogData.AddRange(PriceResult.Item2);
                                }
                            }
                            else // 무료 가챠라면 무료 가챠 회수 차감
                            {
                                UserDailyData.FreeGacha_One[$"{SummonType}"]--;
                                var UserDailyResult = await RecordMethod.ProcessUserDailyRecordData(AccountCode, UserDailyData);
                                outBodyData.Add(UserDailyResult.Item1);
                                outLogData.Add(UserDailyResult.Item2);
                            }

                            // 가챠 진행시 미션 카운트
                            var GuideMissionResult = await MissionMethod.ProcessCheckMission(AccountCode, eMissionKind.Gacha_Count);
                            outBodyData.AddRange(GuideMissionResult.Item1);
                            outLogData.AddRange(GuideMissionResult.Item2);

                            switch ((eGachaType)SummonType)
                            {
                                case eGachaType.Advanced:
                                    {
                                        var Gacha_Advanced_Count = await MissionMethod.ProcessCheckMission(AccountCode, eMissionKind.Gacha_Advanced_Count);
                                        outBodyData.AddRange(Gacha_Advanced_Count.Item1);
                                        outLogData.AddRange(Gacha_Advanced_Count.Item2);
                                    }
                                    break;
                                case eGachaType.Legend:
                                    {
                                        var Gacha_Legend_Count = await MissionMethod.ProcessCheckMission(AccountCode, eMissionKind.Gacha_Legend_Count);
                                        outBodyData.AddRange(Gacha_Legend_Count.Item1);
                                        outLogData.AddRange(Gacha_Legend_Count.Item2);
                                    }
                                    break;
                            }

                            // 가챠 횟수 누적

                            string Key = $"{SummonType}_{SummonUniqueKey}";
                            var UserAccumulateRecordData = await RecordMethod.GetUserAccumulateRecordData(AccountCode);
                            UserAccumulateRecordData.GachaCount[$"{Key}"]++;
                            var ResultAccumulateData =
                                await RecordMethod.ProcessUserAccumulateRecordData(AccountCode, UserAccumulateRecordData);
                            outBodyData.Add(ResultAccumulateData.Item1);
                            outLogData.Add(ResultAccumulateData.Item2);

                            // 컨텐츠 로그
                            await ServerLog.SetContentsLog(AccountCode, ServerUtil.GetEnumFullName(ShopContents.GachaOne), outLogData);

                            // 가챠 로그
                            List<string> GetHeros = new List<string>();
                            GetHeros.Add(ServerUtil.MakeData($"{NewPick[0]}", $"{NewPick[2]}", $"{NewPick[3]}"));
                            await ServerLog.SetGachaLog(AccountCode, SummonType, 1, GetHeros);

                            var Data = ServerUtil.MakeData(ServerUtil.ToJson(GachaResult), $"{SummonType}", $"{SummonUniqueKey}");
                            outHeaderData = ServerUtil.MakeHeaderData(ShopContents.GachaOne, true, Data);
                            Result = await ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                            return new Tuple<PacketState, string>(packetState, Result);
                        }
                        catch (Exception e)
                        {
                            outHeaderData = ServerUtil.MakeHeaderData(ShopContents.GachaOne, false);
                            outBodyData.Add(ServerUtil.MakeBodyData(ReceiveType.ShowMessage, MessageType.Message, MessageData));
                            Result = await ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                            return new Tuple<PacketState, string>(packetState, Result);
                        }
                    }
                case ShopContents.GachaTen:
                    {
                        var Datas = PacketData.HeaderData.Data.Split("#");
                        int SummonType = int.Parse(Datas[0]);
                        int SummonUniqueKey = int.Parse(Datas[1]);

                        string MessageData = "UI_Message_0004";
                        try
                        {
                            // 컨텐츠 잠금
                            var IsContentsOpen = await CommonMethod.IsContentsOpen(AccountCode, eContentsType.Gacha);
                            if (!IsContentsOpen)
                                throw new Exception();

                            var SummonInfoTable = await DataManager.GetTable<SummonInfo>(TableType.SummonInfo);
                            if (SummonInfoTable == null)
                                throw new Exception();

                            var SummonInfo = SummonInfoTable.SingleOrDefault(Table => Table.Value.Key == SummonType && Table.Value.Pickup_Unique_Key == SummonUniqueKey).Value;
                            if (SummonInfo == null)
                                throw new Exception();

                            var SummonDuplicateExpTable = await DataManager.GetTable<SummonDuplicateExp>(TableType.SummonDuplicateExp);
                            if (SummonDuplicateExpTable == null)
                                throw new Exception();

                            // 유저 히어로 데이터
                            var UserHeroData = await HeroMethod.GetUserHeroData(AccountCode);
                            if (UserHeroData == null)
                                throw new Exception();

                            // 소모 재화 체크하기
                            var UserDailyData = await RecordMethod.GetUserDailyRecordData(AccountCode);
                            // 일일 무료 가챠 체크
                            int FreeGachaCount = 0;
                            // 10회 무료
                            UserDailyData.FreeGacha_Ten.TryGetValue($"{SummonType}", out FreeGachaCount);

                            // 합산
                            var UserGemCount = await CommonMethod.GetItemCount(AccountCode, SummonInfo.Price_1_Key);
                            var UserCoinCount = await CommonMethod.GetItemCount(AccountCode, SummonInfo.Price_2_Key);

                            var GemCount = UserGemCount.OtherCount != -1 ? UserGemCount.Count + UserGemCount.OtherCount : UserGemCount.Count;
                            var CoinCount = UserCoinCount.OtherCount != -1 ? UserCoinCount.Count + UserCoinCount.OtherCount : UserCoinCount.Count;

                            if (FreeGachaCount <= 0)
                            {
                                if (CoinCount <= 0 && GemCount <= SummonInfo.Price_1_Ten)
                                {
                                    MessageData = "UI_Message_0003";
                                    throw new Exception();
                                }
                                else if (CoinCount > 0 && CoinCount < SummonInfo.Price_2_Ten && GemCount + (CoinCount * (SummonInfo.Price_1_Ten / 10)) < SummonInfo.Price_1_Ten)
                                {
                                    MessageData = "UI_Message_0003";
                                    throw new Exception();
                                }
                            }

                            var UserCommonData = await UserMethod.GetUserCommonData(AccountCode);
                            if (UserCommonData == null)
                            {
                                MessageData = "UI_Message_0003";
                                throw new Exception();
                            }

                            var HeroInfoTable = await DataManager.GetTable<HeroInfo>(TableType.HeroInfo);
                            if (HeroInfoTable == null)
                                throw new Exception();

                            var SuperVillainInfoTable = await DataManager.GetTable<SuperVillainInfo>(TableType.SuperVillainInfo);
                            if (HeroInfoTable == null)
                                throw new Exception();

                            List<int[]> HeroList = new List<int[]>();
                            List<GachaResultData> GachaReslutDataList = new List<GachaResultData>();
                            for (int count = 0; count < 10; count++)
                            {
                                var NewPick = await GetGachaResult(SummonType, SummonUniqueKey);

                                eMissionKind MissionKind = eMissionKind.None;

                                if (UserHeroData.HeroCollection.ContainsKey($"{NewPick[0]}"))
                                {
                                    if (!UserHeroData.HeroCollection[$"{NewPick[0]}"].IsOpen)
                                        UserHeroData.HeroCollection[$"{NewPick[0]}"].IsOpenTime = ServerUtil.DateTimeNow;

                                    UserHeroData.HeroCollection[$"{NewPick[0]}"].IsOpen = true;
                                    UserHeroData.HeroCollection[$"{NewPick[0]}"].GetCount++;
                                }
                                else
                                {
                                    UserHeroCollection HeroCollection = new UserHeroCollection();
                                    HeroCollection.HeroKey = NewPick[0];
                                    HeroCollection.Type = NewPick[1];
                                    HeroCollection.IsOpen = true;
                                    HeroCollection.IsOpenTime = ServerUtil.DateTimeNow;
                                    HeroCollection.GetCount = 1;
                                    HeroCollection.IsReward = false;
                                    UserHeroData.HeroCollection.Add($"{NewPick[0]}", HeroCollection);
                                }

                                UserHero heroData = new UserHero();

                                switch ((eHeroType)NewPick[1])
                                {
                                    case eHeroType.Hero:
                                        {
                                            var HeroInfo = HeroInfoTable.Values.Where(Data => Data.Key == NewPick[0]).SingleOrDefault();

                                            heroData.Unique = new HeroUniqueData(NewPick[0], UserHeroData.HeroCollection[$"{NewPick[0]}"].GetCount);
                                            heroData.Group = HeroInfo.Group;
                                            heroData.Type = eHeroType.Hero.GetHashCode();
                                            heroData.Exp = 0;
                                            heroData.Lv = 1;
                                            heroData.Grade = HeroInfo.Grade;
                                            var cp = await Calculator.GetLongTotalCombatPower(heroData);
                                            heroData.CP = (int)cp;
                                            heroData.IdleSkillLv = 1;

                                            MissionKind = eMissionKind.Get_Hero;
                                        }
                                        break;
                                    case eHeroType.SuperVillain:
                                        {
                                            var SuperVillainInfo = SuperVillainInfoTable.Values.Where(Data => Data.Key == NewPick[0]).SingleOrDefault();

                                            heroData.Unique = new HeroUniqueData(NewPick[0], UserHeroData.HeroCollection[$"{NewPick[0]}"].GetCount);
                                            heroData.Group = SuperVillainInfo.Group;
                                            heroData.Type = eHeroType.SuperVillain.GetHashCode();
                                            heroData.Exp = 0;
                                            heroData.Lv = 1;
                                            heroData.Grade = SuperVillainInfo.Grade;
                                            var cp = await Calculator.GetLongTotalCombatPower(heroData);
                                            heroData.CP = (int)cp;
                                            heroData.IdleSkillLv = 1;

                                            MissionKind = eMissionKind.Get_SuperVillain;
                                        }
                                        break;
                                }

                                UserHeroData.HeroList.Add(heroData);

                                var MissionResult = await MissionMethod.ProcessCheckMission(AccountCode, MissionKind, NewPick[0]);
                                outBodyData.AddRange(MissionResult.Item1);
                                outLogData.AddRange(MissionResult.Item2);

                                HeroList.Add(NewPick);
                                GachaReslutDataList.Add(GachaResultData.Make(heroData.Unique, NewPick[0], NewPick[1], heroData.Grade));
                            }

                            long TotalCombatPower = 0;

                            foreach (var Hero in UserHeroData.HeroList)
                            {
                                long CombatPower = await Calculator.GetLongTotalCombatPower(Hero);
                                TotalCombatPower += CombatPower;
                            }

                            UserCommonData.TotalCombatPower = TotalCombatPower;
                            UserCommonData.LastReinforceTime = ServerUtil.DateTimeNow;

                            var HeroResult = await HeroMethod.ProcessUserHeroData(AccountCode, UserHeroData);
                            outBodyData.Add(HeroResult.Item1);
                            outLogData.Add(HeroResult.Item2);

                            var UserResult = await UserMethod.ProcessUserCommonData(AccountCode, UserCommonData);
                            outBodyData.Add(UserResult.Item1);
                            outLogData.Add(UserResult.Item2);

                            if (FreeGachaCount <= 0)
                            {
                                long UseCoinCount = 0;
                                if (CoinCount > 0)
                                {
                                    UseCoinCount = Math.Min(CoinCount, SummonInfo.Price_2_Ten);
                                    var PriceResult = await CommonMethod.ProcessAddItemData(AccountCode, SummonInfo.Price_2_Key, -UseCoinCount);
                                    outBodyData.AddRange(PriceResult.Item1);
                                    outLogData.AddRange(PriceResult.Item2);
                                }

                                {
                                    var UseGemCount = SummonInfo.Price_1_Ten - (UseCoinCount * SummonInfo.Price_1_Ten / 10);
                                    if (UseGemCount > 0)
                                    {
                                        var PriceResult = await CommonMethod.ProcessAddItemData(AccountCode, SummonInfo.Price_1_Key, (UseGemCount * -1));
                                        outBodyData.AddRange(PriceResult.Item1);
                                        outLogData.AddRange(PriceResult.Item2);
                                    }
                                }
                            }
                            else // 무료 가챠라면 무료 가챠 회수 차감
                            {
                                // 일일 10회 무료 가챠 회수 차감
                                UserDailyData.FreeGacha_Ten[$"{SummonType}"]--;

                                var UserDailyResult = await RecordMethod.ProcessUserDailyRecordData(AccountCode, UserDailyData);
                                outBodyData.Add(UserDailyResult.Item1);
                                outLogData.Add(UserDailyResult.Item2);
                            }

                            // 가챠 진행시 미션 카운트
                            var Gacha_Count = await MissionMethod.ProcessCheckMission(AccountCode, eMissionKind.Gacha_Count, 0, 10);
                            outBodyData.AddRange(Gacha_Count.Item1);

                            switch ((eGachaType)SummonType)
                            {
                                case eGachaType.Advanced:
                                    {
                                        var Gacha_Advanced_Count = await MissionMethod.ProcessCheckMission(AccountCode, eMissionKind.Gacha_Advanced_Count, 0, 10);
                                        outLogData.AddRange(Gacha_Advanced_Count.Item2);
                                    }
                                    break;
                                case eGachaType.Legend:
                                    {
                                        var Gacha_Legend_Count = await MissionMethod.ProcessCheckMission(AccountCode, eMissionKind.Gacha_Legend_Count, 0, 10);
                                        outLogData.AddRange(Gacha_Legend_Count.Item2);
                                    }
                                    break;
                            }

                            // 가챠 횟수 누적
                            string Key = $"{SummonType}_{SummonUniqueKey}";
                            var UserAccumulateRecordData = await RecordMethod.GetUserAccumulateRecordData(AccountCode);
                            UserAccumulateRecordData.GachaCount[$"{Key}"] += 10;
                            var ResultAccumulateData =
                                await RecordMethod.ProcessUserAccumulateRecordData(AccountCode,
                                    UserAccumulateRecordData);
                            outBodyData.Add(ResultAccumulateData.Item1);
                            outLogData.Add(ResultAccumulateData.Item2);


                            // 컨텐츠 로그
                            await ServerLog.SetContentsLog(AccountCode, ServerUtil.GetEnumFullName(ShopContents.GachaTen), outLogData);

                            // 가챠 로그
                            List<string> GetHeros = new List<string>();
                            for (int count = 0; count < HeroList.Count; count++)
                                GetHeros.Add(ServerUtil.MakeData($"{HeroList[count][0]}", $"{HeroList[count][2]}", $"{HeroList[count][3]}"));

                            await ServerLog.SetGachaLog(AccountCode, SummonType, 1, GetHeros);

                            var Data = ServerUtil.MakeData(ServerUtil.ToJson(GachaReslutDataList), $"{SummonType}", $"{SummonUniqueKey}");
                            outHeaderData = ServerUtil.MakeHeaderData(ShopContents.GachaTen, true, Data);
                            Result = await ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                            return new Tuple<PacketState, string>(packetState, Result);
                        }
                        catch (Exception e)
                        {
                            outHeaderData = ServerUtil.MakeHeaderData(ShopContents.GachaTen, false);
                            outBodyData.Add(ServerUtil.MakeBodyData(ReceiveType.ShowMessage, MessageType.Message, MessageData));
                            Result = await ServerUtil.MakePacket(PacketData.contentsType, outHeaderData, outBodyData);
                            return new Tuple<PacketState, string>(packetState, Result);
                        }
                    }
            }

            packetState = PacketState.UnknownPacket;
            Result = ServerUtil.MakeUnkownErrorData(PacketType.ContentsPacket, ContentsType.Shop, PacketData.contentsType, PacketData.HeaderData.ContentsIndex);
            return new Tuple<PacketState, string>(packetState, Result);
        }
    }
}