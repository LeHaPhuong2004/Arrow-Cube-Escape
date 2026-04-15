using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject winPanel;
    public TextMeshProUGUI starText;
    public GridManager gridManager;
    public TextMeshProUGUI stepText;
    [Header("Canvas")]
    public GameObject menuCanvas;
    public GameObject levelSelectCanvas;
    public GameObject gameCanvas;
    public UnityEngine.UI.Button[] levelButtons;

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
    public void OnPlay()
    {
        menuCanvas.SetActive(false);
        levelSelectCanvas.SetActive(true);

        int unlocked = gridManager.unlockedLevel;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i <= unlocked)
            {
                levelButtons[i].interactable = true;
                levelButtons[i].image.color = Color.white;
            }
            else
            {
                levelButtons[i].interactable = false;
                levelButtons[i].image.color = Color.gray;
            }
        }
    }

    public void OnSelectLevel(int levelIndex)
    {
        
        if (levelIndex > gridManager.unlockedLevel)
        {
            Debug.Log("Level chưa mở");
            return;
        }

        levelSelectCanvas.SetActive(false);
        gameCanvas.SetActive(true);

        HideWin();
        gridManager.LoadLevel(levelIndex);
    }

    public void BackToMenu()
    {
        gameCanvas.SetActive(false);
        levelSelectCanvas.SetActive(false);
        menuCanvas.SetActive(true);

      
        gridManager.ResetLevel();
    }

    public void BackToLevelSelect()
    {
        gameCanvas.SetActive(false);
        levelSelectCanvas.SetActive(true);
    }
}