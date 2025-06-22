using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // ou UnityEngine.UI si tu utilises le Text classique

public class HonorVictoryWindow : MonoBehaviour
{
    public static HonorVictoryWindow Instance;
    public GameObject menuButtonObject;
    public GameObject nextGameButtonObject;
    public TMP_Text victoryText;
    public void menuButton(){SceneManager.LoadScene("Menu");}
    public void nextGameButton(){SceneManager.LoadScene("Menu - MapSelection");}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        if (GameData.fusScore >= GameData.winScore){
            victoryText.text = "Honor forces won the war !";
            menuButtonObject.SetActive(true);
            nextGameButtonObject.SetActive(false);
        }
        else {
            victoryText.text = "Honor forces won the battle !\nGet ready for the next one !";
            menuButtonObject.SetActive(false);
            nextGameButtonObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
