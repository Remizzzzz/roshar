using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionWindow : MonoBehaviour
{
    /**
     * MapSelectionWindow is a singleton class that manages the map selection window.
     * It allows the user to select different maps to load.
     * The maps are loaded using Unity's SceneManager.
     */
    public static MapSelectionWindow Instance;
    public void Akinah(){SceneManager.LoadScene("Akinah");}
    public void Azimir(){SceneManager.LoadScene("Azimir");}
    public void Kholinar(){SceneManager.LoadScene("Kholinar");}
    public void Kurth(){SceneManager.LoadScene("Kurth");}
    public void Narak(){SceneManager.LoadScene("Narak");}
    public void Panatham(){SceneManager.LoadScene("Panatham");}
    public void RallElorim(){SceneManager.LoadScene("RallElorim");}
    public void Shinovar(){SceneManager.LoadScene("Shinovar");}
    public void ThaylenahCity(){SceneManager.LoadScene("ThaylenahCity");}
    public void Vedenar(){SceneManager.LoadScene("Vedenar");}
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
