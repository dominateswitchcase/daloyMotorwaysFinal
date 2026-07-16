using UnityEngine;
using UnityEngine.UI;

public class MenuHowToPlaySwitchPages : MonoBehaviour
{
    [Header("Drag pages 1-10 here in order")]
    public GameObject[] pages;

    [Header("Buttons")]
    public Button nextButton;
    public Button prevButton; // optional, leave empty if not used

    private int currentPage = 0;

    void OnEnable()
    {
        // Reset to page 1 every time the popup is opened
        currentPage = 0;
        ShowCurrentPage();
    }

    void Start()
    {
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        if (prevButton != null)
            prevButton.onClick.AddListener(PreviousPage);

        ShowCurrentPage();
    }

    void ShowCurrentPage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            if (pages[i] != null)
                pages[i].SetActive(i == currentPage);
        }

        // Optional: hide Next button on last page
        if (nextButton != null)
            nextButton.gameObject.SetActive(currentPage < pages.Length - 1);

        // Optional: hide Prev button on first page
        if (prevButton != null)
            prevButton.gameObject.SetActive(currentPage > 0);
    }

    public void NextPage()
    {
        if (currentPage < pages.Length - 1)
        {
            currentPage++;
            ShowCurrentPage();
        }
    }

    public void PreviousPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowCurrentPage();
        }
    }
}