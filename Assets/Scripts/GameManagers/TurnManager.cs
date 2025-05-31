using UnityEngine;
using TMPro; // or UnityEngine.UI

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public TextMeshProUGUI turnText; // ou public Text turnText;
    public MapParameters mapParameters;
    private bool turn=true; //false if it's the first player turn, true if it's the second player turn
    private bool enhancedSummonFus = false;
    private int enhancedSummonFusTurn = 0;
    private bool enhancedSummonFluct = false;
    private int enhancedSummonFluctTurn = 0;
    private bool startingPlayer = true; //True if fluctuomanciens start first, false if fusionnes start first
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
        turn = !turn;
        hasSummoned = 0;
        if (turn && startingPlayer) turnNumber++;
        if (turn) turnText.text = "Fluctuomanciens " + turnNumber.ToString();
        else turnText.text = "Fusionnés " + turnNumber.ToString();

        if (!turn && (turnNumber - 2 >= enhancedSummonFusTurn)) enhancedSummonFus = false;
        if (turn && (turnNumber - 2 >= enhancedSummonFluctTurn)) enhancedSummonFluct = false;
    }
    public int getTurnNumber(){
        return turnNumber;
    }
    public bool getPlayerTurn(){
        return turn!=startingPlayer; //If true, it's the turn of fluct, if false, it's the turn of fus
    }

    public bool getEnhancedSummonFus() =>enhancedSummonFus;
    public bool getEnhancedSummonFluct() =>enhancedSummonFluct;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        if (turn) turnText.text = "Fluctuomanciens " + turnNumber.ToString();
        else turnText.text = "Fusionnés " + turnNumber.ToString();
        //There will be a method getFirstPlayer(), for now, blue always start
        turn=true;
        startingPlayer=true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
