using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PlayerController;

public class ElevatorPanelInitializer : MonoBehaviour
{
    [SerializeField] private GameObject panelObj;
    [SerializeField] private GameObject buttonObj;
    [SerializeField] private GameObject youAreHereObj;
    private List<ElevatorPortalData> levels;
    private int thisLevel;

    private bool hasData = false;

    public void PassData(List<ElevatorPortalData> levels, int thisLevel)
    {
        this.levels = levels;
        this.thisLevel = thisLevel;
        hasData = true;
    }

    public void Init(ElevatorController controller, PlayerController2D cc)
    {
        if (!hasData)
        {
            return;
        }
        this.cc = cc;
        this.cc.LockInputs();

        var hereRect = youAreHereObj.GetComponent<RectTransform>();

        void makeButton(int i)
        {
            var newButtonObj = Instantiate(buttonObj, panelObj.transform);
            var newRect = newButtonObj.GetComponent<RectTransform>();

            var newX = hereRect.position.x;
            var newY = hereRect.position.y + (hereRect.sizeDelta.y * (thisLevel - i));
            var newZ = hereRect.position.z;
            newRect.position = new Vector3(newX, newY, newZ);

            // button properties
            var newButton = newButtonObj.GetComponent<UnityEngine.UI.Button>();
            var text = newButton.GetComponentInChildren<TextMeshProUGUI>();

            text.text = levels[i].PortalDisplayName;

            newButton.onClick.AddListener(() =>
            {
                Debug.Log("click");
                if (controller != null)
                {
                    Debug.Log("here");
                    controller.GoToLevel(i);
                    controller.UIUp = false;
                }
                Close(false);
            });
        }

        // draw upper buttons
        for (var i = thisLevel - 1; i > -1; i--)
        {
            Debug.Log(i);
            makeButton(i);
        }

        // draw lower buttons
        for (var i = thisLevel + 1; i < levels.Count; i++)
        {
            Debug.Log(i + "gooy");
            makeButton(i);
        }
    }

    private PlayerController2D cc;
    public void Close(bool unlock = true)
    {
        if (cc != null && unlock)
        {
            cc.UnlockInputs();
        }
        Destroy(gameObject);
    }
}
