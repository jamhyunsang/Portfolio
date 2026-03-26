using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleModule : MonoBehaviour
{
    #region Singleton
    protected static BattleModule m_Instance;
    public static BattleModule Instance
    {
        get
        {
            return m_Instance;
        }
    }

    public static T CreateBattleModule<T>() where T : BattleModule
    {
        GameObject Obj = GameObject.Find("[BattleModule]");

        if (Obj == null)
        {
            Obj = new GameObject("[BattleModule]");
            DontDestroyOnLoad(Obj);
        }

        var Module = Obj.AddComponent<T>();
        m_Instance = Module;
        return Module;
    }
    #endregion

    #region Member Property
    protected Player m_Player = null;
    protected Queue<Floor> m_Floors;

    protected GameObject Obj_Left = null;
    protected GameObject Obj_Mid = null;
    protected GameObject Obj_Right = null;

    protected GameObject Obj_Character = null;
    protected GameObject Obj_Floor = null;

    protected bool m_IsJumping = false;
    public bool IsJumping
    {
        get { return m_IsJumping; }
    }

    protected bool m_IsPaused = false;
    #endregion

    #region Override Method
    public virtual async UniTask Init()
    {
        m_Floors = new Queue<Floor>();
    }

    public virtual async UniTask LoadResources()
    {
        await UniTask.Yield();
    }

    public virtual async UniTask StartGame()
    {
        await Init();

        await LoadResources();

        await UIManager.Instance.Close<LoadingWindow>();
    }
    #endregion

    #region Member Method
    public void SetPosObject(GameObject left, GameObject mid, GameObject right)
    {
        Obj_Left = left;
        Obj_Mid = mid;
        Obj_Right = right;
    }

    public void SetCharacterObject(GameObject character)
    {
        Obj_Character = character;
    }

    public void SetFloorObject(GameObject floor)
    {
        Obj_Floor = floor;
    }

    public void Jump(Vector3 start, Vector3 end)
    {
        var Direction = GetDirection(start, end);
        Debug.Log($"{Direction}");

        if (Direction == eDirection.None)
            return;

        StartCoroutine(Routine_Jump(Direction));
    }

    public IEnumerator Routine_Jump(eDirection direction)
    {
        m_IsJumping = true;

        var StartX = m_Player.transform.position.x;
        var EndX = 0f;

        switch (direction)
        {
            case eDirection.Up:
                {
                    EndX = Obj_Mid.transform.position.x;
                }
                break;
            case eDirection.Right:
                {
                    EndX = Obj_Right.transform.position.x;
                }
                break;
            case eDirection.Left:
                {
                    EndX = Obj_Left.transform.position.x;
                }
                break;
        }

        float Elapsed = 0f;
        float Duration = 1f * (1f - m_Player.Speed);
        float UpDuration = Duration * 0.75f;
        float DownDuration = Duration * 0.25f;
        var Floors = m_Floors.ToList();
        List<float> StartYs = Floors.Select((f, i) => i * 300f).ToList();

        while (Elapsed < Duration)
        {
            Elapsed += Time.deltaTime;
            float currentY;

            if (Elapsed < UpDuration)
            {
                float Time = Elapsed / UpDuration;
                currentY = Mathf.Lerp(0, -450f, Time);
            }
            else
            {
                float Time = (Elapsed - UpDuration) / DownDuration;
                currentY = Mathf.Lerp(-450f, -300f, Time);
            }

            for (int Count = 0; Count < Floors.Count; Count++)
            {
                Vector3 pos = Floors[Count].transform.position;
                pos.y = StartYs[Count] + currentY;
                Floors[Count].transform.position = new Vector3(pos.x, pos.y, pos.z);
            }

            Vector3 PlayerPos = m_Player.transform.position;
            PlayerPos.x = Mathf.Lerp(StartX, EndX, Elapsed / Duration);
            m_Player.transform.position = new Vector3(PlayerPos.x, PlayerPos.y, PlayerPos.z);

            yield return null;
        }

        m_IsJumping = false;
    }

    public eDirection GetDirection(Vector2 start, Vector2 end)
    {
        var Direction = end - start;
        
        if(Direction.sqrMagnitude < ClientDefine.MinDistance * ClientDefine.MinDistance)
            return eDirection.None;

        var X = Direction.x * 2;
        var Y = Direction.y;

        if(Mathf.Abs(X) > Mathf.Abs(Y))
        {
            return X > 0 ? eDirection.Right : eDirection.Left;
        }
        else
        {
            return Y > 0 ? eDirection.Up : eDirection.None;
        }
    }
    #endregion
}
