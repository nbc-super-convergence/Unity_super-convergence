using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum eUIPosition
{
    /// <summary> 0 : 가장 밑에 깔리는 배경화면</summary>
    Background,

    /// <summary> 1 : 화면 상/하단의 안내 바</summary>
    Navigator,

    /// <summary> 2 : 팝업창</summary>
    Popup,

    /// <summary> 3 : 가장 위에서 모든걸 가려주는 UI.</summary>
    Overlap
}

public class UIManager : Singleton<UIManager>
{
    private List<Transform> parents;
    private List<UIBase> uiList = new List<UIBase>();

    [SerializeField] private Canvas loadingCanvas;
    [SerializeField] private GameObject prefabLoadingScreen;
    private GameObject loadingScreen;

    //GameManager해서 호출함으로써 Manager간 초기화 서순 지키기.
    public async void Init()
    {
        await Show<UILogin>();
        isInitialized = true;
    }

    public static void SetParents(List<Transform> parents)
    {
        Instance.parents = parents;
    }

    /// <typeparam name="T">UIBase를 상속받은 클래스 이름</typeparam>
    /// <param name="param">원하는 모든 형태의 변수 전달 가능</param>
    /// <returns>T 반환</returns>
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
    /// UIBase의 bool값에 따라 setactive false 또는 파괴
    /// </summary>
    /// <typeparam name="T">UIBase를 상속받은 클래스 이름</typeparam>
    /// <param name="param">원하는 모든 형태의 변수 전달 가능</param>
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
    /// 동적 생성한 UI를 가져오는 메서드
    /// </summary>
    /// <typeparam name="T">UI 스크립트 이름</typeparam>
    /// <returns>UI 스크립트</returns>
    public static T Get<T>() where T : UIBase
    {
        return (T)Instance.uiList.Find(obj => obj.name == typeof(T).ToString());
    }

    /// <summary>
    /// UI 존재 여부
    /// </summary>
    /// <typeparam name="T">UI 스크립트 이름</typeparam>
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