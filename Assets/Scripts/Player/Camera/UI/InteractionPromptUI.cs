using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    public GameObject root;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI actionText;
    public Image crosshair;

    private PlayerInteractor interactor;

    void Start()
    {
        interactor = FindFirstObjectByType<PlayerInteractor>();
        if (interactor == null)
        {
            Debug.LogError("PlayerInteractor not found");
            return;
        }
        Hide();
    }

    void LateUpdate()
    {
        if (interactor == null) return;

        if (interactor.HasTarget)
            Show(interactor.CurrentTarget);
        else
            Hide();
    }

    void Show(Interactable target)
    {
        if (root != null && !root.activeSelf) root.SetActive(true);

        if (nameText != null) nameText.text = target.displayName;
        if (actionText != null) actionText.text = "[E] Взаимодействовать";
        if (crosshair != null) crosshair.color = new Color(1f, 0.8f, 0.2f, 1f);
    }

    void Hide()
    {
        if (root != null && root.activeSelf) root.SetActive(false);
        if (crosshair != null) crosshair.color = new Color(1f, 1f, 1f, 0.5f);
    }
}