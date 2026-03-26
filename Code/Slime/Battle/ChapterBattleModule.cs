using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChapterBattleModule : BattleModule
{
    #region Member Property
    private string m_ChapterKey = string.Empty;
    private int m_CurrentFloor = 1;
    private int m_MaxFloor = -1;
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
        var Window = await UIManager.Instance.Open<InGameWindow>(eUI.Main, "Assets/AddressableResources/UI/Scene/GameWindow.prefab", true);
        await Window.InitWindow(m_ChapterKey);

        var Obj = await ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AddressableResources/UI/InGame/Floor.prefab", true);
        for (int Count = 0; Count < 7; Count++)
        {
            var FloorObj = GameObject.Instantiate(Obj, Vector3.zero, Quaternion.identity);
            FloorObj.transform.SetParent(Obj_Floor.transform);

            var RectTrans = FloorObj.GetComponent<RectTransform>();
            RectTrans.offsetMin = Vector2.zero;
            RectTrans.offsetMax = Vector2.zero;
            RectTrans.anchoredPosition = new Vector3(0, Count * 300, 0);
            RectTrans.sizeDelta = new Vector2(RectTrans.sizeDelta.x, 300f);

            var Floor = FloorObj.GetComponent<Floor>();
            m_Floors.Enqueue(Floor);
        }
        ResourceManager.Instance.ReleaseResource(Obj, true);

        Obj = await ResourceManager.Instance.LoadResourceAsync<GameObject>("Assets/AddressableResources/UI/InGame/Player.prefab", true);
        var PlayerObj = GameObject.Instantiate(Obj, Vector3.zero, Quaternion.identity);
        PlayerObj.transform.SetParent(Obj_Character.transform);
        m_Player = PlayerObj.GetComponent<Player>();
        m_Player.Position = ePosition.Mid;
        m_Player.transform.position = Obj_Mid.transform.position;
        ResourceManager.Instance.ReleaseResource(Obj, true);
    }

    public override async UniTask StartGame()
    {
        await base.StartGame();
    }
    #endregion
}
