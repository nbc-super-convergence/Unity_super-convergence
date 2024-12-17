using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ChangeInputTab : MonoBehaviour
{
    private EventSystem system;
    [SerializeField] private Selectable firstInput;
    [SerializeField] private Button goBtn;

    private void Start()
    {
        system = EventSystem.current;
        firstInput.Select();
    }

    private void Update()
    {
        if (system.currentSelectedGameObject != null)
        {
            if (Input.GetKeyDown(KeyCode.Tab) && Input.GetKey(KeyCode.LeftShift))
            {

                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                if (next != null)
                {
                    next.Select();
                }

            }
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if (next != null)
                {
                    next.Select();
                }
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                if (!UIManager.IsOpened<UIRegister>() && !UIManager.IsOpened<UIError>())
                {
                    goBtn.onClick.Invoke();
                }
            }
        }
    }
}
