using UnityEngine;
using TMPro; // or UnityEngine.UI

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public MapParameters mapParameters;
    private bool turn=true; //true if it's the first player turn, false if it's the second player turn
    private bool enhancedSummonFus = false;
    private int enhancedSummonFusTurn = 0;
    internal bool newTurn = false;
    private bool enhancedSummonFluct = false;
    private int enhancedSummonFluctTurn = 0;
    private bool startingPlayer = true; //True if fluctuomancers start first, false if fused start first
    private int turnNumber=1;//Turns start at 1
    private int hasSummoned=0;//TurnManager manage also the number of summon per turn (only one by default)
    //Public method
    public bool isPlayerTurn(bool isFluct) {return (turn==isFluct);}
    public bool nbSummonVerif(){ return (hasSummoned == mapParameters.numberOfSummon); }
    public bool isSummonable(bool isSpecial, bool isFluct)
    {
        bool verification = isFluct ? !enhancedSummonFluct : !enhancedSummonFus;
        if (nbSummonVerif()) return false;
        if (isSpecial) return (mapParameters.specialSummoningTurns.Contains(turnNumber) && verification);
        return (mapParameters.summoningTurns.Contains(turnNumber) && verification);
    }
    public void incrSummon(){hasSummoned++;}
    public void enhancedSummonPerformed(bool isFluct){
        if (isFluct) {
            enhancedSummonFluct = true;
            enhancedSummonFluctTurn = turnNumber;
        } else {
                enhancedSummonFusTurn = turnNumber;
                enhancedSummonFus = true;
            }
    }
    public void updateTurn()
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
        return turn!=startingPlayer; //If false, it's the turn of fluct, if true, it's the turn of fus
        /*
        turn           | true (1st player turn) | false (2d Player turn) | true (1st player turn) | false (2d Player turn)
        startingPlayer | true (Fluct)           | false (Fus)            | false (Fus)            | true (Fluct)
        output         | false (Fluct)          | false (Fluct)          | true (Fus)             | true (Fus)
        So if turn is true, it's the turn of fluct, if false, it's the turn of fus
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
        //There will be a method getFirstPlayer(), for now, blue always start
        startingPlayer=true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
