using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject firstPauseButton; // 🎯 Drag Resume button in Inspector

    private bool isPaused = false;

    void Start()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
        // ✅ Escape manually triggers Pause even if a UI element is focused
        if (!isPaused && Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
            return;
        }

        // 🔁 Toggle pause with controller (Start)
        if (Input.GetButtonDown("PauseController"))
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }

        // ✅ Escape or B (Cancel) unpauses when already paused
        if (isPaused && (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel")))
        {
            ResumeGame();
        }

        // ✅ A or Enter to click UI button
        if (isPaused && Input.GetButtonDown("Submit"))
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                Button btn = selected.GetComponent<Button>();
                btn?.onClick.Invoke();
            }
        }
    }


    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        EventSystem.current.SetSelectedGameObject(null); // Clear selection
    }

    public void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        StartCoroutine(SetFirstButton());
    }

    private System.Collections.IEnumerator SetFirstButton()
    {
        yield return null; // Wait one frame
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstPauseButton);
    }

    public void LoadDesertScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("_Complete-Game");
    }

    public void LoadJungleScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("JungleMapScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
