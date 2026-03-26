using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public class InGameWindow_Game : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Cashed Object
    [SerializeField] private GameObject Obj_Pos = null;
    [SerializeField] private GameObject Obj_Floor = null;
    [SerializeField] private GameObject Obj_Character = null;
    #endregion

    #region Member Property
    private int m_CurrentFloor = 1;
    private int m_MaxFloor = -1;         

    private bool m_IsPointDown = false;
    private int m_PointerId = -1;
    private Vector2 m_StartPos;
    private Vector2 m_EndPos;
    #endregion

    #region Member Method
    public async UniTask Init(string stageKey)
    {
        GameObject Left = Obj_Pos.transform.Find("Left").gameObject;
        GameObject Mid = Obj_Pos.transform.Find("Mid").gameObject;
        GameObject Right = Obj_Pos.transform.Find("Right").gameObject;

        BattleModule.Instance.SetPosObject(Left, Mid, Right);

        BattleModule.Instance.SetCharacterObject(Obj_Character);

        BattleModule.Instance.SetFloorObject(Obj_Floor);

        await UniTask.Yield();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (BattleModule.Instance.IsJumping)
            return;

        if(m_IsPointDown)
            return;

        m_IsPointDown = true;

        m_StartPos = eventData.position;

        m_PointerId = eventData.pointerId;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (BattleModule.Instance.IsJumping)
            return;

        if (m_PointerId != eventData.pointerId)
            return;

        if (!m_IsPointDown)
            return;

        m_EndPos = eventData.position;

        BattleModule.Instance.Jump(m_StartPos, m_EndPos);

        m_IsPointDown = false;
    }
    #endregion

    #region Button Event
    private void OnClick_Controller()
    {

    }
    #endregion
}
