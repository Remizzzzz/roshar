using UnityEngine;

public enum GamePhase {movement,summon,combat}
public class PhaseManager : MonoBehaviour
{
    /** This class manages the game phases.
     It handles the transitions between movement, summoning, and combat phases.
     It also provides methods to check the current phase and to advance to the next phase.
     */
    public static PhaseManager Instance;
    public bool AnimatePhaseChange = true; ///If true, the phase change will be animated, this is a flag that an object of the interface use
    private GamePhase gamePhase = GamePhase.summon; /// The current game phase, initialized to summoning phase
    public GamePhase getPhase(){return gamePhase;}

    /**The three methods : MovementPhase, SummoningPhase, and CombatPhase
     check if the current game phase is movement, summoning, or combat respectively.
     They return true if the current phase matches the specified phase.
     */
    public bool MovementPhase() {return gamePhase==GamePhase.movement;}
    public bool SummoningPhase() {return gamePhase==GamePhase.summon;}
    public bool CombatPhase() {return gamePhase==GamePhase.combat;}

    public void nextPhase(){
        /** This method advances the game to the next phase.
         It checks the current phase and transitions to the next one accordingly.
         it also updates the AnimatePhaseChange flag to true,
         so the interface will animate the phase change.
         */
        if (MovementPhase()) {
            gamePhase=GamePhase.summon;
            bool isFluctTurn = !TurnManager.Instance.getPlayerTurn();
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
