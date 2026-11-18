using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject controlsPanel;     // Drag ControlsPanel here
    public GameObject firstButton;       // Main menu's first button (e.g. Start)
    public GameObject backButton;        // Back button inside ControlsPanel

    private string[] availableMaps = new string[] { "_Complete-Game", "JungleMapScene" };

    void Start()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        // ✅ Automatically focus first menu button for controller input
        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton);
    }

    void Update()
    {
        // ✅ Enter or A button to "click" selected UI button
        if (Input.GetButtonDown("Submit"))
        {
            GameObject selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                Button btn = selected.GetComponent<Button>();
                if (btn != null)
                {
                    btn.onClick.Invoke();
                }
            }
        }

        // ✅ Escape or B button to go back when Controls panel is active
        if (controlsPanel != null && controlsPanel.activeSelf && Input.GetButtonDown("Cancel"))
        {
            HideControls();
        }
    }

    public void StartGame()
    {
        int index = Random.Range(0, availableMaps.Length);
        SceneManager.LoadScene(index >= 0 ? availableMaps[index] : "_Complete-Game");
    }

    public void LoadDesert() => SceneManager.LoadScene("_Complete-Game");

    public void LoadJungle() => SceneManager.LoadScene("JungleMapScene");

    public void ShowControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(true);

        // ✅ Delay selection to avoid controller re-focusing "Start" button
        if (backButton != null)
            StartCoroutine(DelayedFocus(backButton));
    }

    public void HideControls()
    {
        if (controlsPanel != null)
            controlsPanel.SetActive(false);

        // ✅ Return focus to main menu button
        if (firstButton != null)
            StartCoroutine(DelayedFocus(firstButton));
    }

    private System.Collections.IEnumerator DelayedFocus(GameObject target)
    {
        yield return null; // Wait 1 frame
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit Game");
    }
}
