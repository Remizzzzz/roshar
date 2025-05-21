using UnityEngine;
using UnityEngine.SceneManagement;

public class OdiumVictoryWindow : MonoBehaviour
{
    public static OdiumVictoryWindow Instance;
    public void menuButton(){SceneManager.LoadScene("Menu");}
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
