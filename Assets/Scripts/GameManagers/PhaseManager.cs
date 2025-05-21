using UnityEngine;

public enum GamePhase {movement,summon,combat}
public class PhaseManager : MonoBehaviour
{
    public static PhaseManager Instance;
    private GamePhase gamePhase = GamePhase.summon;
    public GamePhase getPhase(){return gamePhase;}
    public bool MovementPhase() {return gamePhase==GamePhase.movement;}
    public bool SummoningPhase() {return gamePhase==GamePhase.summon;}
    public bool CombatPhase() {return gamePhase==GamePhase.combat;}

    public void nextPhase(){
        if (MovementPhase()) gamePhase=GamePhase.summon;
        else if (SummoningPhase()) gamePhase=GamePhase.combat;
        else if (CombatPhase()) {
            gamePhase=GamePhase.movement;
            TurnManager.Instance.updateTurn();
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance=this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
