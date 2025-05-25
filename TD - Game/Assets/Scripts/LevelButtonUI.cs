using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelButtonUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Button button;
    public Image backgroundImage;

    public void Init(string levelName, bool interactable, System.Action onClick)
    {
        levelText.text = levelName;
        button.interactable = interactable;
        backgroundImage.color = interactable ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        button.onClick.RemoveAllListeners();
        if (interactable && onClick != null)
            button.onClick.AddListener(() => onClick());
    }
}