using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class PauseMenuUI : MonoBehaviour
{
    [Header("Panel")]
    public GameObject panel;

    [Header("Buttons")]
    public Button saveButton;
    public Button loadButton;
    public Button continueButton;
    public Button quitButton;

    [Header("Settings")]
    public int currentSlot = 1;

    [Header("Debug")]
    public bool debugLogs = true;

    private PlayerInputActions input;
    private bool isOpen = false;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.UI.Enable();
        input.UI.Cancel.performed += OnCancel;
    }

    void OnDisable()
    {
        input.UI.Cancel.performed -= OnCancel;
    }

    void Start()
    {
        if (panel != null) panel.SetActive(false);

        if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
        if (loadButton != null) loadButton.onClick.AddListener(OnLoadClicked);
        if (continueButton != null) continueButton.onClick.AddListener(OnContinueClicked);
        if (quitButton != null) quitButton.onClick.AddListener(OnQuitClicked);
    }

    void OnDestroy()
    {
        if (saveButton != null) saveButton.onClick.RemoveListener(OnSaveClicked);
        if (loadButton != null) loadButton.onClick.RemoveListener(OnLoadClicked);
        if (continueButton != null) continueButton.onClick.RemoveListener(OnContinueClicked);
        if (quitButton != null) quitButton.onClick.RemoveListener(OnQuitClicked);
    }

    void OnCancel(InputAction.CallbackContext ctx)
    {
        ToggleMenu();
    }

    void ToggleMenu()
    {
        isOpen = !isOpen;

        if (panel != null) panel.SetActive(isOpen);

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;

        if (debugLogs)
            Debug.Log($"[PauseMenu] Меню {(isOpen ? "открыто" : "закрыто")}");
    }

    void OnSaveClicked()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("[PauseMenu] SaveManager не найден!");
            return;
        }

        SaveManager.Instance.SaveGame(currentSlot);
    }

    void OnLoadClicked()
    {
        if (SaveManager.Instance == null)
        {
            Debug.LogError("[PauseMenu] SaveManager не найден!");
            return;
        }

        SaveManager.Instance.LoadGame(currentSlot);
    }

    void OnContinueClicked()
    {
        if (isOpen) ToggleMenu();
    }

    void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}