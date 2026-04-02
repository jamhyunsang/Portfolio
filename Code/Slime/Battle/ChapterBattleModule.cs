using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChapterBattleModule : BattleModule
{
    #region Member Property
    private string m_ChapterKey = string.Empty;
    private Dictionary <int, List<ChapPlace>> m_ChapterPlace = new Dictionary<int, List<ChapPlace>>();
    #endregion

    #region Member Method
    public void SetChapterKey(string chapterKey)
    {
        m_ChapterKey = chapterKey;
    }
    #endregion

    #region Override Method
    public override async UniTask Init()
    {
        await base.Init();

        var ChapterPlaceTable = DataManager.Instance.GetTable<ChapPlace>(eTableType.ChapPlace).Where(Data => Data.ChapKey == m_ChapterKey).ToList();
        m_ChapterPlace = ChapterPlaceTable.GroupBy(Data => Data.VrPos).ToDictionary(Data => Data.Key, Data => Data.ToList());

        m_MaxFloor = m_ChapterPlace.Count;
    }

    public override async UniTask LoadResources()
    {
        // Window Load
        var Window = await UIManager.Instance.Open<InGameWindow>(eUI.Main, "Assets/AddressableResources/UI/Scene/GameWindow.prefab", true);
        await Window.InitWindow(m_ChapterKey);

        // Floor Load
        var Obj = await ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AddressableResources/UI/InGame/Floor.prefab", true);
        for (int Count = 0; Count < ClientDefine.FloorCount; Count++)
        {
            var FloorObj = GameObject.Instantiate(Obj, Vector3.zero, Quaternion.identity);
            FloorObj.transform.SetParent(Obj_Floor.transform);

            var RectTrans = FloorObj.GetComponent<RectTransform>();
            RectTrans.position = new Vector3(0, Count * 300, 0);

            var Floor = FloorObj.GetComponent<Floor>();
            Floor.SetFloor(m_ChapterPlace[Count + 1]);
            m_Floors.Add(Floor);
        }
        ResourceManager.Instance.ReleaseResource(Obj, true);

        // Player Load
        Obj = await ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AddressableResources/UI/InGame/Player.prefab", true);
        var PlayerObj = GameObject.Instantiate(Obj, Vector3.zero, Quaternion.identity);
        PlayerObj.transform.SetParent(Obj_Character.transform);
        m_Player = PlayerObj.GetComponent<Player>();
        m_Player.transform.position = Obj_Mid.transform.position;
        ResourceManager.Instance.ReleaseResource(Obj, true);
    }

    protected override async UniTask JumpActionAsync()
    {
        await base.JumpActionAsync();

        var Floor = m_Floors.Last();

        if (m_FloorCount + ClientDefine.FloorCount - 1 <= m_MaxFloor)
            Floor.SetFloor(m_ChapterPlace[m_FloorCount + ClientDefine.FloorCount - 1]);
        else
            Floor.SetFloor(null);
    }

    public override async UniTask StartGame()
    {
        await base.StartGame();
    }

    public override bool IsMoveable(ePosition position)
    {
        if (m_MaxFloor == m_FloorCount)
        {
            return false;
        }

        var NextFloor= m_ChapterPlace[m_FloorCount + 1];
        return NextFloor.Exists(Data => Data.HrPos == position.ToString());
    }
    #endregion
}
