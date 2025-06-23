using UnityEngine;
using TMPro; // or UnityEngine.UI

public class TurnManager : MonoBehaviour
{
    /** TurnManager is a singleton that manages the turns of the game.
     It keeps track of the current turn, whether it's the first or second player's turn,
     and whether enhanced summoning is available for either player.
     It also manages the number of summons per turn and the turn number.
     The map parameters are used to determine the rules for summoning and turns.
     it pretty much manages the game flow and all turn related stuff.
     */
    public static TurnManager Instance;
    public MapParameters mapParameters; ///MapParameters is a scriptable object that contains the parameters of the map, such as the number of summons per turn, the turns when special summoning is allowed and the troops that the map unlocks.
    private bool turn=true; ///true if it's the first player turn, false if it's the second player turn
    private bool enhancedSummonFus = false; ///True if the fused player has performed an enhanced summon (and if so, it can summon for one turn), false otherwise
    private int enhancedSummonFusTurn = 0; ///Turn when the enhanced summon was performed, used to check if the enhanced summon malus is still active
    internal bool newTurn = false; 
    private bool enhancedSummonFluct = false; ///True if the fluctuomancer player has performed an enhanced summon (and if so, it can summon for one turn), false otherwise
    private int enhancedSummonFluctTurn = 0; ///Turn when the enhanced summon was performed, used to check if the enhanced summon malus is still active
    private bool startingPlayer = true; ///True if fluctuomancers start first, false if fused start first
    private int turnNumber=1;///Turns start at 1
    private int hasSummoned=0;///TurnManager manage also the number of summon per turn (only one by default)

    //Public method
    public bool isPlayerTurn(bool isFluct) {return (turn==isFluct);}
    public bool nbSummonVerif(){ return (hasSummoned == mapParameters.numberOfSummon); }
    public bool isSummonable(bool isSpecial, bool isFluct) /// Checks if a piece can be summoned
    {
        bool verification = isFluct ? !enhancedSummonFluct : !enhancedSummonFus;
        if (nbSummonVerif()) return false;
        if (isSpecial) return (mapParameters.specialSummoningTurns.Contains(turnNumber) && verification);
        return (mapParameters.summoningTurns.Contains(turnNumber) && verification);
    }
    public void incrSummon(){hasSummoned++;} /// Increments the number of summons made in the current turn
    public void enhancedSummonPerformed(bool isFluct){ /// Marks that an enhanced summon has been performed
        if (isFluct) {
            enhancedSummonFluct = true;
            enhancedSummonFluctTurn = turnNumber;
        } else {
                enhancedSummonFusTurn = turnNumber;
                enhancedSummonFus = true;
            }
    }
    public void updateTurn() /// Updates the turn, increments the turn number, and resets the number of summons made in the current turn
    {
        newTurn = true;
        turn = !turn;
        hasSummoned = 0;
        if (turn && startingPlayer) turnNumber++;

        if (!turn && (turnNumber - 2 >= enhancedSummonFusTurn)) enhancedSummonFus = false;
        if (turn && (turnNumber - 2 >= enhancedSummonFluctTurn)) enhancedSummonFluct = false;
    }
    public int getTurnNumber(){
        return turnNumber;
    }
    public bool getPlayerTurn(){
        return turn!=startingPlayer;
        /**
        turn           | true (1st player turn) | false (2d Player turn) | true (1st player turn) | false (2d Player turn)
        startingPlayer | true (Fluct)           | false (Fus)            | false (Fus)            | true (Fluct)
        output         | false (Fluct)          | false (Fluct)          | true (Fus)             | true (Fus)
        So if turn is false, it's the turn of fluct, if true, it's the turn of fus
        */
    }

    public bool getEnhancedSummonFus() =>enhancedSummonFus;
    public bool getEnhancedSummonFluct() =>enhancedSummonFluct;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Awake(){
        foreach (string troop in mapParameters.unlockedTroops)
        {
            GameData.isUnlocked.Add(troop);
        }
        Instance = this;
    }
    void Start()
    {
        //There will be a method getFirstPlayer(), for now, fluctuomancer always start
        startingPlayer=true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
