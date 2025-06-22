using UnityEngine;
using UnityEngine.SceneManagement;

public class MapSelectionWindow : MonoBehaviour
{
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
