using UnityEngine;
using TMPro; // or UnityEngine.UI

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance;
    public TextMeshProUGUI turnText; // ou public Text turnText;
    public MapParameters mapParameters;
    private bool turn=true;
    private bool startingPlayer=true;
    private int turnNumber=1;//Turns start at 1
    private int hasSummoned=0;//TurnManager manage also the number of summon per turn (only one by default)
    //Public method
    public bool isPlayerTurn(bool isFluct) {return (turn==isFluct);}
    public bool isSummonable(bool isSpecial){
        if (hasSummoned==mapParameters.numberOfSummon) return false;
        if (isSpecial) return (mapParameters.specialSummoningTurns.Contains(turnNumber));
        return mapParameters.summoningTurns.Contains(turnNumber);
    }
    public void incrSummon(){hasSummoned++;}
    public void updateTurn(){
        turn=!turn;
        hasSummoned=0;
        if (turn && startingPlayer) turnNumber++;
        if (turn) turnText.text = "Fluctuomanciens " + turnNumber.ToString();
        else turnText.text = "Fusionnés " + turnNumber.ToString();
    }
    public int getTurnNumber(){
        return turnNumber;
    }
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
