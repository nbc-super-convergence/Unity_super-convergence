using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum eUIPosition
{
    /// <summary> 0 : ���� �ؿ� �򸮴� ���ȭ��</summary>
    Background,

    /// <summary> 1 : ȭ�� ��/�ϴ��� �ȳ� ��</summary>
    Navigator,

    /// <summary> 2 : �˾�â</summary>
    Popup,

    /// <summary> 3 : ���� ������ ���� �����ִ� UI.</summary>
    Overlap
}

public class UIManager : Singleton<UIManager>
{
    private List<Transform> parents;
    private List<UIBase> uiList = new List<UIBase>();

    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private GameObject prefabLoadingScreen;
    private GameObject loadingScreen;

    //GameManager�ؼ� ȣ�������ν� Manager�� �ʱ�ȭ ���� ��Ű��.
    public async void Init()
    {
        await Show<UILogin>();
        isInitialized = true;
    }

    public static void SetParents(List<Transform> parents)
    {
        Instance.parents = parents;
    }

    /// <typeparam name="T">UIBase�� ��ӹ��� Ŭ���� �̸�</typeparam>
    /// <param name="param">���ϴ� ��� ������ ���� ���� ����</param>
    /// <returns>T ��ȯ</returns>
    public async static Task<T> Show<T>(params object[] param) where T : UIBase
    {
        UIManager.Instance.uiList.RemoveAll(obj => obj == null);

        if (UIManager.Instance.loadingScreen == null)
        {
            Transform targetCanvas = Instance.loadingCanvas.transform;
            UIManager.Instance.loadingScreen = Instantiate(UIManager.Instance.prefabLoadingScreen, targetCanvas);
            UIManager.Instance.loadingScreen.transform.SetAsLastSibling();
        }
        else
        {
            UIManager.Instance.loadingScreen.transform.SetAsLastSibling();
            UIManager.Instance.loadingScreen.SetActive(true);
        }

        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());

        if (ui == null)
        {
            var prefab = await ResourceManager.Instance.LoadAsset<T>(typeof(T).ToString(), eAddressableType.UI);
            ui = Instantiate(prefab, Instance.parents[(int)prefab.uiPosition]);
            ui.name = ui.name.Replace("(Clone)", "");

            Instance.uiList.Add(ui);
        }
        ui.opened?.Invoke(param);
        ui.gameObject.SetActive(ui.isActiveInCreated);
        ui.isActiveInCreated = true;
        UIManager.Instance.loadingScreen.SetActive(false);
        return (T)ui;
    }

    /// <summary>
    /// UIBase�� bool���� ���� setactive false �Ǵ� �ı�
    /// </summary>
    /// <typeparam name="T">UIBase�� ��ӹ��� Ŭ���� �̸�</typeparam>
    /// <param name="param">���ϴ� ��� ������ ���� ���� ����</param>
    public static void Hide<T>(params object[] param) where T : UIBase
    {
        var ui = Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
        if (ui != null)
        {
            ui.closed.Invoke(param);
            if (ui.isDestroyAtClosed)
            {
                Instance.uiList.Remove(ui);
                Destroy(ui.gameObject);
            }
            else
            {
                ui.SetActive(false);
            }
        }
    }

    /// <summary>
    /// ���� ������ UI�� �������� �޼���
    /// </summary>
    /// <typeparam name="T">UI ��ũ��Ʈ �̸�</typeparam>
    /// <returns>UI ��ũ��Ʈ</returns>
    public static T Get<T>() where T : UIBase
    {
        return (T)Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
    }

    /// <summary>
    /// UI ���� ����
    /// </summary>
    /// <typeparam name="T">UI ��ũ��Ʈ �̸�</typeparam>
    /// <returns></returns>
    public static bool IsOpened<T>() where T : UIBase
    {
        return Instance.uiList.Exists(obj => obj.name == typeof(T).ToString());
    }

    public static void LoadBoardScene()
    {
        if (UIManager.Instance.loadingScreen == null)
        {
            Transform targetCanvas = Instance.loadingCanvas.transform;
            UIManager.Instance.loadingScreen = Instantiate(UIManager.Instance.prefabLoadingScreen, targetCanvas);
            UIManager.Instance.loadingScreen.transform.SetAsLastSibling();
        }
        else
        {
            UIManager.Instance.loadingScreen.transform.SetAsLastSibling();
            UIManager.Instance.loadingScreen.SetActive(true);
        }
        //AsyncOperation asyncOper = SceneManager.LoadSceneAsync(2);
        //asyncOper.allowSceneActivation = true;

        //if (asyncOper.isDone)
        //{
        //    UIManager.Instance.loadingScreen.SetActive(false);
        //    asyncOper.allowSceneActivation = true;

        //}
        SceneManager.LoadScene(2);

        if (SceneManager.GetSceneAt(2) != null)
        {
            UIManager.Instance.loadingScreen.SetActive(false);
        }
    }
}