using UnityEngine;
using UnityEngine.SceneManagement;


public class WinCondition : MonoBehaviour
{
    public static WinCondition Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    internal int fluctOnMap = 0;
    internal int fusOnMap=0;
    public void UpdateFluctOnMap(bool incr)
    {
        if (incr) fluctOnMap++;
        else fluctOnMap--;
        if (fluctOnMap < 0) fluctOnMap = 0;
    }
    public void UpdateFusOnMap(bool incr)
    {
        if (incr) fusOnMap++;
        else fusOnMap--;
        if (fusOnMap < 0) fusOnMap = 0;
    }
    private void VerifyWinCondition()
    {
        if (fluctOnMap <= 0 && !TurnManager.Instance.mapParameters.summoningTurns.Contains(TurnManager.Instance.getTurnNumber()))
        {
            GameData.fusScore ++;
            SceneManager.LoadScene("OdiumVictory");
        }
        else if (fusOnMap <= 0 && !TurnManager.Instance.mapParameters.summoningTurns.Contains(TurnManager.Instance.getTurnNumber()))
        {
            GameData.fluctScore ++;
            SceneManager.LoadScene("HonorVictory");
        }
    }
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        VerifyWinCondition();
    }
}
