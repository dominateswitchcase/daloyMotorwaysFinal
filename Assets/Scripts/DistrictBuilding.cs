using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DistrictBuilding : MonoBehaviour
{
    [Header("Commuter Spawning")]
    public GameObject commuterPrefab;
    public Vector3 spawnOffset = new Vector3(2f, 0, 0);

    [Header("Shed Requirements")]
    public float shedSearchRadius = 4f;
    public float dangerTimer = 45f;

    [Header("UI Settings")]
    public TextMeshPro timerText;
    public float textHeightOffset = 4f;
    public int textSize = 40;

    private float currentTimer;
    public bool isDemandingShed = false;
    private Color buildingColor;

    
    [Header("Game Over Panel")]
    public UIAutoAnimation GameOverPanel;
    public PopupManager GameOverPopupManager;   

    void Start()
    {
        Debug.Log($"Script is attached to: {gameObject.name}", gameObject); 
        currentTimer = dangerTimer;

        buildingColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

        MeshRenderer mesh = GetComponentInChildren<MeshRenderer>();
        if (mesh != null)
        {
            mesh.material.color = buildingColor;
        }

        if (timerText == null) AutoGenerateTimerText();
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (TimeManager.Instance != null) TimeManager.Instance.RegisterBuilding(this);
    }

    void AutoGenerateTimerText()
    {
        GameObject textObj = new GameObject("Auto_TimerText");
        textObj.transform.position = this.transform.position + new Vector3(0, textHeightOffset, 0);
        textObj.transform.SetParent(this.transform);

        timerText = textObj.AddComponent<TextMeshPro>();
        timerText.alignment = TextAlignmentOptions.Center;
        timerText.fontSize = textSize;
        timerText.color = Color.red;
        timerText.fontStyle = FontStyles.Bold;
    }

    void Update()
    {
        if (!isDemandingShed) return;

        if (timerText != null && timerText.gameObject.activeSelf && Camera.main != null)
        {
            timerText.transform.LookAt(Camera.main.transform);
            timerText.transform.Rotate(0, 180, 0);
        }

        WaitingShed availableShed = FindAvailableShed();

        if (availableShed == null)
        {
            currentTimer -= Time.deltaTime;

            if (timerText != null)
            {
                timerText.text = Mathf.CeilToInt(currentTimer).ToString();
            }

            // if (currentTimer <= 0) GameOver();
            if (currentTimer <= 40f) GameOver();
        }
        else
        {
            isDemandingShed = false;
            currentTimer = dangerTimer;
            if (timerText != null) timerText.gameObject.SetActive(false);

            SpawnCommuter(availableShed);
        }
    }

    WaitingShed FindAvailableShed()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, shedSearchRadius);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("WaitingShed"))
            {
                WaitingShed shedScript = hit.GetComponent<WaitingShed>();
                if (shedScript != null && shedScript.HasSpace())
                {
                    return shedScript;
                }
            }
        }
        return null;
    }

    void SpawnCommuter(WaitingShed shedTarget)
    {
        if (commuterPrefab != null)
        {
            Vector3 desiredPosition = transform.position + spawnOffset;
            Vector3 spawnPosition = desiredPosition;

            Vector3 rayStart = desiredPosition + new Vector3(0, 20f, 0);
            if (Physics.Raycast(rayStart, Vector3.down, out RaycastHit hit, 100f))
            {
                spawnPosition = hit.point;
            }

            GameObject newCommuter = Instantiate(commuterPrefab, spawnPosition, Quaternion.identity);

            CapsuleCollider col = newCommuter.GetComponent<CapsuleCollider>();
            if (col != null)
            {
                float halfHeight = (col.height * newCommuter.transform.localScale.y) / 2f;
                newCommuter.transform.position += new Vector3(0, halfHeight, 0);
            }

            CommuterAI ai = newCommuter.GetComponent<CommuterAI>();

            if (ai != null)
            {
                ai.SetDestination(shedTarget);
                ai.commuterColor = buildingColor;
                shedTarget.ReserveSpot();

                MeshRenderer commuterMesh = newCommuter.GetComponentInChildren<MeshRenderer>();
                if (commuterMesh != null)
                {
                    commuterMesh.material.color = buildingColor;
                }
            }
        }
        else
        {
            Debug.LogError("Commuter Prefab is missing from the DistrictBuilding script!");
        }
    }

    public void ActivateDemand()
    {
        if (!isDemandingShed)
        {
            isDemandingShed = true;
            currentTimer = dangerTimer;
            if (timerText != null) timerText.gameObject.SetActive(true);
        }
    }

    void GameOver()
    {
        // SceneManager.LoadScene("Main Menu");
        GameOverPopupManager.ShowPopup();
        GameOverPanel.EntranceAnimation();
        Debug.Log("GAME OVER! A building was left without a shed.");
        
        // GameOverPopupManager.ShowPopup();
        // GameOverPanel.EntranceAnimation();
        // Time.timeScale = 0f;

        // // GameOverPanel.ShowPopup();
        // GameOverPanel.GetComponent<PopupManager>().ShowPopup();
        // GameOverPanel.EntranceAnimation();



    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shedSearchRadius);
    }
}