using UnityEngine;
using UnityEngine.SceneManagement;


public class WinCondition : MonoBehaviour
{
    /** WinCondition is a singleton that manages the win condition of the game.
     * It keeps track of the number of Fluctuomancers and Fused pieces on the map.
     * If there are no Fluctuomancers left, the Fused player wins, and vice versa.
     * The win condition is checked every frame.
     */
    public static WinCondition Instance;
    internal int fluctOnMap = 0;
    internal int fusOnMap=0;
    public void UpdateFluctOnMap(bool incr) /// Updates the number of Fluctuomancers on the map (true to increment, false to decrement).
    {
        if (incr) fluctOnMap++;
        else fluctOnMap--;
        if (fluctOnMap < 0) fluctOnMap = 0;
    }
    public void UpdateFusOnMap(bool incr) /// Updates the number of Fused pieces on the map (true to increment, false to decrement).
    {
        if (incr) fusOnMap++;
        else fusOnMap--;
        if (fusOnMap < 0) fusOnMap = 0;
    }
    private void VerifyWinCondition()
    { /// Checks the win condition every frame.
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
