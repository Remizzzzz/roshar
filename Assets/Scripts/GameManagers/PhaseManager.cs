using UnityEngine;

public enum GamePhase {movement,summon,combat}
public class PhaseManager : MonoBehaviour
{
    public static PhaseManager Instance;
    public bool AnimatePhaseChange = true; //If true, the phase change will be animated, this a flag that an object of the interface use
    private GamePhase gamePhase = GamePhase.summon;
    public GamePhase getPhase(){return gamePhase;}
    public bool MovementPhase() {return gamePhase==GamePhase.movement;}
    public bool SummoningPhase() {return gamePhase==GamePhase.summon;}
    public bool CombatPhase() {return gamePhase==GamePhase.combat;}

    public void nextPhase(){
        if (MovementPhase()) {
            gamePhase=GamePhase.summon;
            bool isFluctTurn = !TurnManager.Instance.getPlayerTurn(); //Yup, don't try to understand, here getPlayerTurn() returns !isFluctTurn, and below, getPlayerTurn() returns isFluctTurn, idk why
            if (isFluctTurn){
                if (TurnManager.Instance.getEnhancedSummonFluct()) gamePhase=GamePhase.combat;
            } else {
                if (TurnManager.Instance.getEnhancedSummonFus()) gamePhase=GamePhase.combat;
            }
            if (!TurnManager.Instance.mapParameters.summoningTurns.Contains(TurnManager.Instance.getTurnNumber())) gamePhase=GamePhase.combat;
            
        }
        else if (SummoningPhase()) gamePhase=GamePhase.combat;
        else if (CombatPhase()) {
            gamePhase=GamePhase.movement;
            if (PieceInteractionManager.Instance.getTargetOnMap(TurnManager.Instance.getPlayerTurn())==null) gamePhase=GamePhase.summon;
            TurnManager.Instance.updateTurn();
        }
        AnimatePhaseChange = true; //We set the flag to true, so the interface will animate the phase change
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
