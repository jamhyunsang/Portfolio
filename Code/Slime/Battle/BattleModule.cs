using Cysharp.Threading.Tasks;
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
    protected List<Floor> m_Floors;

    protected GameObject Obj_Left = null;
    protected GameObject Obj_Mid = null;
    protected GameObject Obj_Right = null;

    protected GameObject Obj_Character = null;
    protected GameObject Obj_Floor = null;

    protected ePosition m_Position = ePosition.Mid;

    protected int m_FloorCount = 1;
    protected int m_MaxFloor = -1;

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
        m_Floors = new List<Floor>();
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

    public async UniTask Jump(Vector3 start, Vector3 end)
    {
        var Direction = GetDirection(start, end);

        if (Direction == eDirection.None)
            return;

        m_IsJumping = true;

        switch (Direction)
        {
            case eDirection.Right:
                {
                    m_Position++;
                    if (m_Position >= ePosition.RightDrop)
                    {
                        m_Position = ePosition.RightDrop;
                    }
                }
                break;
            case eDirection.Left:
                {
                    m_Position--;
                    if (m_Position <= ePosition.LeftDrop)
                    {
                        m_Position = ePosition.LeftDrop;
                    }
                }
                break;
        }

        if (m_Position == ePosition.LeftDrop || m_Position == ePosition.RightDrop)
        {
            Debug.Log("GameOver");
        }
        else
        {
            if (IsMoveable(m_Position))
            {
                // 점프
                await JumpActionAsync();
                // 아이템 사용
            }
            else
            {
                Debug.Log("NoJump");
                // 이동 불가능한 위치로 점프 시도 시 처리 애니메이션
            }
        }

        m_IsJumping = false;
    }

    // 점프 로직 다음 발판 설정까지 해주기
    protected virtual async UniTask JumpActionAsync()
    {
        var StartX = m_Player.transform.position.x;
        var EndX = GetPositionX(m_Position);

        float Elapsed = 0f;
        float Duration = 1f * (1f - m_Player.Speed);
        float UpDuration = Duration * 0.75f;
        float DownDuration = Duration * 0.25f;
        List<float> StartYs = m_Floors.Select((f, i) => i * 300f).ToList();

        while (Elapsed < Duration)
        {
            Elapsed += Time.deltaTime;
            
            // 발판 이동
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

            for (int Count = 0; Count < m_Floors.Count; Count++)
            {
                m_Floors[Count].transform.position = new Vector3(0, StartYs[Count] + currentY, 0);
            }

            // 플레이어 이동
            m_Player.transform.position = new Vector3(Mathf.Lerp(StartX, EndX, Elapsed / Duration), m_Player.transform.position.y, 0);

            await UniTask.Yield();
        }

        // 이동 완료 후 처리
        // 발판
        var Floor = m_Floors.First();
        m_Floors.RemoveAt(0);
        m_Floors.Add(Floor);

        for (int Count = 0; Count < m_Floors.Count; Count++)
        {
            m_Floors[Count].transform.position = new Vector3(0, Count * 300, 0);
        }

        // 플레이어
        m_Player.transform.position = new Vector3(EndX, m_Player.transform.position.y, 0);

        m_FloorCount++;
    }

    private eDirection GetDirection(Vector2 start, Vector2 end)
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

    private float GetPositionX(ePosition position)
    {
        switch (position)
        {
            case ePosition.LeftDrop:
            case ePosition.Left:
                return Obj_Left.transform.position.x;
            case ePosition.Mid:
                return Obj_Mid.transform.position.x;
            case ePosition.Right:
            case ePosition.RightDrop:
                return Obj_Right.transform.position.x;
        }
        return 0f;
    }

    public virtual bool IsMoveable(ePosition position)
    {
        return true;
    }
    #endregion
}
