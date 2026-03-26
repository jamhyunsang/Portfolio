using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T m_Instance = null;
    public static T Instance
    {
        get
        {
            if (m_Instance == null)
            {
                var ManagersObj = GameObject.Find("[Managers]");
                if (ManagersObj == null)
                {
                    ManagersObj = new GameObject("[Managers]");
                    DontDestroyOnLoad(ManagersObj);
                }

                var Manager = typeof(T);
                var ManagerObj = ManagersObj.transform.Find($"{Manager.Name}");
                if (ManagerObj == null)
                {
                    ManagerObj = new GameObject(Manager.Name).transform;
                    ManagerObj.transform.SetParent(ManagersObj.transform);
                }

                m_Instance = ManagerObj.GetComponent<T>();
                if (m_Instance == null)
                {
                    m_Instance = ManagerObj.gameObject.AddComponent<T>();
                }

                m_Instance.Init();
            }

            return m_Instance;
        }
    }

    protected abstract void Init();
}
