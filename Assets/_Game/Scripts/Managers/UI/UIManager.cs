using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    Dictionary<System.Type, UICanvas> canvasActives = new Dictionary<System.Type, UICanvas>();
    Dictionary<System.Type, UICanvas> canvasPrefabs = new Dictionary<System.Type, UICanvas>();
    [SerializeField] Transform parent;

    private void Awake()
    {
        // Load UI prefabs from Resources
        UICanvas[] prefabs = Resources.LoadAll<UICanvas>("UI/");
        for (int i = 0; i < prefabs.Length; i++)
        {
            canvasPrefabs.Add(prefabs[i].GetType(), prefabs[i]);
        }
    }
    //OPEN canvas
    public T OpenUI<T>() where T : UICanvas
    {
        T canvas = GetUI<T>();
        canvas.Setup();
        canvas.Open();
        
        return canvas;
    }
    // CLOSE canvas after time(s)
    public void CloseUI<T>(float time) where T : UICanvas
    {
        if (IsUIOpened<T>())
        {
            canvasActives[typeof(T)].Close(time);
        }
    }
    // CLOSE canvas directly
    public void CloseUIDirectly<T>() where T : UICanvas
    {
        if (IsUIOpened<T>())
        {
            canvasActives[typeof(T)].CloseDirectly();
        }
    }
    // Check canvas is created?
    public bool IsUILoaded<T>() where T : UICanvas // Check UI is Init?
    {
        return canvasActives.ContainsKey(typeof(T)) && canvasActives[typeof(T)] != null;
    }
    // check canvas is ACTIVE
    public bool IsUIOpened<T>()  where T : UICanvas
    {
        return IsUILoaded<T>() && canvasActives[typeof(T)].gameObject.activeSelf;
    }
    // GET Active Canvas
    public T GetUI<T>() where T : UICanvas
    {
        if (!IsUILoaded<T>())
        {
            T prefab = GetUIPrefab<T>();
            T canvas = Instantiate(prefab, parent);
            canvasActives[typeof(T)] = canvas;
        }
        return canvasActives[typeof(T)] as T;
    }
    // Get prefabs
    private T GetUIPrefab<T>() where T : UICanvas
    {
        return canvasPrefabs[typeof(T)] as T;
    }
    // CLOSE ALL canvas
    public void CloseAllUI()
    {
        foreach (var canvas in canvasActives.Values)
        {
            if (canvas != null && canvas.gameObject.activeSelf)
            {
                canvas.Close(0); // Prio use CLose instead of directly
            }
        }
    }
}
