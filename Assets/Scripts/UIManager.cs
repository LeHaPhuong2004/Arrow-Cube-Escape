using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject winPanel;
    public TextMeshProUGUI starText;
    public GridManager gridManager;
    public TextMeshProUGUI stepText;

    public void UpdateStep(int currentStep, int optimalStep)
    {
        stepText.text = $"Step: {currentStep} / {optimalStep}";
    }

    public void ShowWin(int stars)
    {
        winPanel.SetActive(true);
        starText.text = "Stars: " + stars;
    }

    public void HideWin()
    {
        winPanel.SetActive(false);
    }

    public void OnNextButton()
    {
        winPanel.SetActive(false);
        gridManager.NextLevel();
    }
}