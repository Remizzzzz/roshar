using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionWindow : MonoBehaviour
{
    public static MapSelectionWindow Instance;
    public void Kholinar(){SceneManager.LoadScene("Kholinar");}
    public void ThaylenahCity(){SceneManager.LoadScene("ThaylenahCity");}
    public void RallElorim(){SceneManager.LoadScene("RallElorim");}
    public void Test(){SceneManager.LoadScene("SampleScene");}
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
