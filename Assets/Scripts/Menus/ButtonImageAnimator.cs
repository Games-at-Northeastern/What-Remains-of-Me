using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonImageAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    [SerializeField]
    private Image buttonImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonImage.gameObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData data)
    {
        Debug.Log("Mouse entered");
        buttonImage.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData data)
    {
        Debug.Log("object selected");
        buttonImage.gameObject.SetActive(false);
        // temp stub for anim adds
    }

    public void OnSelect(BaseEventData data)
    {
        buttonImage.gameObject.SetActive(true);
    }

    public void OnDeselect(BaseEventData data)
    {
        buttonImage.gameObject.SetActive(false);
    }
}
