using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    #region Cashed Object
    [SerializeField] private Image Img_Character = null;
    #endregion

    #region Member Property
    private int m_MaxHealth = 100;
    public int MaxHealth
    {
        get { return m_MaxHealth; }
    }

    private int m_Health = 100;
    public int Health
    {
        get { return m_Health; }
    }

    private float m_Speed = 0.05f;
    public float Speed
    {
        get { return m_Speed; }
    }
    #endregion

    #region Member Method
    public bool IsAlive()
    {
        return m_Health > 0;
    }

    public void Damage(int value)
    {
        m_Health -= value;
        if (m_Health < 0)
            m_Health = 0;
    }

    public void Heal(int value)
    {
        m_Health += value;
        if (m_Health > 100)
            m_Health = 100;
    }
    #endregion
}
