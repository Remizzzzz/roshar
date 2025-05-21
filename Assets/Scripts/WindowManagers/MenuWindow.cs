using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuWindow : MonoBehaviour
{

    public static MenuWindow Instance;
    public void playButton()
    {
        SceneManager.LoadScene("Menu - MapSelection");
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
