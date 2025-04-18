using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LogMenuManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject logButtonPrefab;
    public Transform logListParent;
    public GameObject logDisplayPanel;
    public TMP_Text logDisplayText;
    public TMP_Text pointsText;
    public GameObject closeButton;

    [Header("Background Elements to Disable")]
    public GameObject logScrollView;
    public GameObject backButton;

    [Header("Log Data")]
    public List<LogEntry> logEntries;

    private PointManager pointManager;

    void OnEnable()
    {
        // First try singleton access
        if (PointManager.Instance != null)
        {
            pointManager = PointManager.Instance;
        }
        else
        {
            pointManager = FindObjectOfType<PointManager>();
        }

        if (pointManager == null)
        {
            Debug.LogError("LogMenuManager: No PointManager found in scene.");
            return;
        }

        ResetLogView();
        RefreshLogList();
        UpdatePointsUI();
    }

    void UpdatePointsUI()
    {
        if (pointManager != null)
            pointsText.text = $"Points: {pointManager.CurrentPoints}";
    }

    public void RefreshLogList()
    {
        foreach (Transform child in logListParent)
        {
            Destroy(child.gameObject);
        }

        foreach (var log in logEntries)
        {
            GameObject newButton = Instantiate(logButtonPrefab, logListParent);
            newButton.GetComponentInChildren<TMP_Text>().text = log.title;

            Button btn = newButton.GetComponent<Button>();

            if (pointManager.CurrentPoints >= log.pointsRequired)
            {
                btn.interactable = true;
                btn.onClick.AddListener(() => ShowLog(log.content));
            }
            else
            {
                btn.interactable = false;
                newButton.GetComponentInChildren<TMP_Text>().text += " (Locked)";
            }
        }
    }

    public void ShowLog(string content)
    {
        logDisplayText.text = content;
        logDisplayPanel.SetActive(true);

        if (logScrollView != null) logScrollView.SetActive(false);
        if (backButton != null) backButton.SetActive(false);
        if (closeButton != null) closeButton.SetActive(true);
    }

    public void CloseLogDisplay()
    {
        ResetLogView();
    }

    public void ResetLogView()
    {
        logDisplayPanel.SetActive(false);
        if (closeButton != null) closeButton.SetActive(false);
        if (logScrollView != null) logScrollView.SetActive(true);
        if (backButton != null) backButton.SetActive(true);
    }

    // Call this when points change to update locked logs
    public void Refresh()
    {
        RefreshLogList();
        UpdatePointsUI();
    }
}
