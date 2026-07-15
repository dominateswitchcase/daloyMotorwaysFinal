using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    [Header("Assign the small square panel here")]
    public GameObject popupPanel;

    [Header("Optional: close button inside the popup")]
    public Button closeButton;

    void Start()
    {
        // Make sure popup starts hidden
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (closeButton != null)
            closeButton.onClick.AddListener(HidePopup);
    }

    public void ShowPopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(true);
    }

    public void HidePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }
}