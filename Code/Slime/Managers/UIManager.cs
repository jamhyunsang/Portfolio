using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    #region Member Property
    private Dictionary<eUI, GameObject> m_Panels = null;
    private Dictionary<string, UIElement> m_UIElements = null;
    #endregion

    #region Override Method
    protected override void Init()
    {
        m_Panels = new Dictionary<eUI, GameObject>();
        m_UIElements = new Dictionary<string, UIElement>();
    }
    #endregion

    #region Member Method
    public void SetRoot(GameObject root)
    {
        for(eUI UI = 0; UI < eUI.Max; UI++)
        {
            m_Panels[UI] = root.transform.Find($"{UI}").gameObject;
        }
    }

    public void SwitchRoot()
    {
        foreach(var UIElement in m_UIElements.Values)
        {
            var Panel = m_Panels[UIElement.UI];

            if (Panel == null)
            {
                Debug.LogError($"Panel for {UIElement.UI} is not set.");
                continue;
            }

            UIElement.transform.SetParent(Panel.transform, false);
        }
    }

    public async UniTask<T> Open<T>(eUI ui, string resourcePath, bool isAddressable) where T : UIElement
    {
        var Name = typeof(T).Name;
        if (m_UIElements.ContainsKey(Name))
        {
            Debug.LogWarning($"UI Element {Name} is already opened.");
            return null;
        }
        var Panel = m_Panels[ui];
        if (Panel == null)
        {
            Debug.LogError($"Panel for {ui} is not set.");
            return null;
        }

        var Element = await ResourceManager.Instance.LoadResourceAsync<GameObject>(resourcePath, isAddressable);
        if (Element == null)
        {
            Debug.LogError($"Failed to load UI Element from {resourcePath}.");
            return null;
        }

        var Obj = Instantiate(Element, Panel.transform);
        if (Obj == null)
        {
            Debug.LogError($"Failed to instantiate UI Element from {resourcePath}.");
            return null;
        }

        ResourceManager.Instance.ReleaseResource(Element, isAddressable);

        var UIElement = Obj.GetComponent<T>();
        if (UIElement == null)
        {
            Debug.LogError($"UI Element {Name} does not have a component of type {typeof(T).Name}.");
            return null;
        }

        UIElement.UI = ui;
        m_UIElements.Add(Name, UIElement);

        UIElement.Init();

        await UIElement.OpenAsync();
        
        UIElement.Open();

        return UIElement;
    }

    public async UniTask Close<T>() where T : UIElement
    {
        var Name = typeof(T).Name;
        if (!m_UIElements.TryGetValue(Name, out var Element))
        {
            Debug.LogWarning($"UI Element {Name} is not opened.");
            return;
        }
        await Element.CloseAsync();
        Element.Close();
        m_UIElements.Remove(Name);
        Destroy(Element.gameObject);
    }

    public async UniTask CloseAll()
    {
        var RemoveList = new List<UIElement>();
        foreach (var Element in m_UIElements)
        {
            if(Element.Key == "LoadingWindow")
            {
                continue;
            }

            RemoveList.Add(Element.Value);
        }

        foreach (var Remove in RemoveList)
        {
            await Remove.CloseAsync();
            Remove.Close();
            m_UIElements.Remove(Remove.GetType().Name);
            Destroy(Remove.gameObject);
        }
    }

    public T GetOpened<T>() where T : UIElement
    {
        var Name = typeof(T).Name;
        if (m_UIElements.TryGetValue(Name, out var element))
        {
            return element as T;
        }

        Debug.LogWarning($"UI Element {Name} is not opened.");

        return null;
    }
    #endregion
}
