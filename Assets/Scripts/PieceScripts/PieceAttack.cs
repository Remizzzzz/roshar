using UnityEngine;
using System.Collections.Generic;
using System;

public class PieceAttack : MonoBehaviour
{
    public int lp=6;
    public int baseAtk=2;
    public int nbAtk =1;
    public int range=1;
    public int d4Atk=0;
    public int dmgReduc=0;

    private int curTurn = 1;
    internal PieceMovement pM; //To reuse the map logic of PieceMovement
    private int curLp;
    private int curNbAtk;
    internal bool isAttacking=false;

    //public methods
    public void decrCurNbAtk(){curNbAtk--;}
    public int getCurLp() => curLp;
    public int getCurNbAtk() =>curNbAtk;
    public void damage(int dmg){
        curLp-=(dmg-dmgReduc);
        if (curLp <= 0)
        {
            TileStateManager.Instance.updateState(pM.getCurPos(), TileState.basic);
            pM.tileMap.RefreshTile(pM.getCurPos());
            PieceInteractionManager.Instance.remove(pM.getCurPos());
            PieceStateManager.Instance.remove(gameObject);
            if (pM.isFluct) WinCondition.Instance.UpdateFluctOnMap(false);
            else WinCondition.Instance.UpdateFusOnMap(false);
            Destroy(gameObject);
        }
    }

    public void heal(int regen){
        curLp+=regen;
        if (curLp>lp) curLp=lp;
    }

    //Private methods
    
    private void refreshMoves(){
        if (curTurn<TurnManager.Instance.getTurnNumber()){
            curNbAtk=nbAtk;
            curTurn=TurnManager.Instance.getTurnNumber();
        }
    }

    private List<Vector3Int> getReachableTargets(){ //Created after pM.detectTilesInRange, not worth modifying
        List<Vector3Int> tilesReachables= new List<Vector3Int>{pM.getCurPos()}; //The List where we'll keep our valid coordinates
            if(pM.getOnMap()){
                List<Vector3Int> search = new List<Vector3Int>(); //The tempList used for searching

                for (int i=0; i<range;i++){ //range determines the depth of search, each loop is one depth
                    foreach(Vector3Int rSearch in tilesReachables){ //Search will go through each neighbor of the tiles in tilesReachables, and add them to tilesReachables if they're not alreday in it
                        foreach(Vector3Int neighbour in (Math.Abs(rSearch.y)%2==1?pM.getNeighbourOffsetOdd():pM.getNeighbourOffset())){ //Change the offset depending on Y (odd or not)
                            if (!tilesReachables.Contains(neighbour+rSearch) && !search.Contains(neighbour+rSearch) && pM.tileMap.GetTile(neighbour+rSearch)!=null) { //Of course, we don't add coordinates that aren't on the map
                                search.Add(neighbour+rSearch); //If the coor isn't in tilesReachable or search and is in the map, add it to search
                            //I thought of only taking occupied tiles, but if I do, my depth algorithm don't work, 
                            // if I select afterwards only tiles occupied, it costs the same as I already filter 
                            // in getAttackTarget() so there's no point doing it
                            }
                        }
                    }
                    tilesReachables.AddRange(search);
                    search.Clear();
                }
            }
            tilesReachables.Remove(pM.getCurPos());
            //Now that we have all the tiles in range, we select only the one with a valid target on it
            return PieceInteractionManager.Instance.getAttackTargets(tilesReachables,pM.isFluct);
    }

    //On Mouse methods
    void OnMouseDown(){
        if (TurnManager.Instance.isPlayerTurn(pM.isFluct) && PhaseManager.Instance.CombatPhase()){ //If it's the piece's players turn and combat mode is activated, it's more likely a selection
            if (!PieceStateManager.Instance.isAttacked(pM.isFluct) && !PieceStateManager.Instance.isAttacked(!pM.isFluct)){//If it's not an attack, but a selection to attack (the PlayerTurn bool already indirectly check this condition, but for safety)
                // ! Note that the second bool actually check if another piece of the same color is already attacking rather than checking if the piece is attacked, it was useless to create a new method
                if (curNbAtk>0){ //If the piece has an attack remaining this turn
                    List<Vector3Int> targetsInRange = getReachableTargets();
                    PieceInteractionManager.Instance.areTargeted(targetsInRange,pM.isFluct);
                    PieceInteractionManager.Instance.setAttacker(this);
                    PieceStateManager.Instance.updateState(gameObject, PieceState.attacking,pM.isFluct);
                    isAttacking=true;
                }
            }
        } else if (PhaseManager.Instance.CombatPhase()){//If it's not the turn of the piece's player, that can means the piece is attacked
            if (PieceStateManager.Instance.isAttacked(pM.isFluct)){ //If the piece is attacked
                PieceInteractionManager.Instance.attack(pM.getCurPos(),pM.isFluct);
            }
        }
    }

    void Awake(){
        pM = GetComponent<PieceMovement>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curLp = lp;
        curNbAtk = nbAtk;

    }

    // Update is called once per frame
    void Update() 
    {
        refreshMoves();
        //The following code seems to not be working, I'll check into that later
        if (isAttacking && Input.GetMouseButtonDown(0)){
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get Mouse Position
            mousePosition.z = 0; 
            Vector3Int cellPos = pM.tileMap.WorldToCell(mousePosition);
            if (!PieceInteractionManager.Instance.isATarget(cellPos) && cellPos!=pM.getCurPos()){
                PieceStateManager.Instance.updateState(gameObject,PieceState.basic,pM.isFluct);
                PieceInteractionManager.Instance.resetTargets();
                PieceInteractionManager.Instance.attacker=null;
                isAttacking=false;
            }
        }
        if (pM.isFluct) {
            if (!pM.isSpecial && !pM.enhancedTroop) //If the troop is basic
            {
                bool hasFluctNeighbor = false;
                List<Vector3Int> neighbors = pM.detectTilesInRange(pM.getCurPos(), 1);
                foreach (Vector3Int neighbor in neighbors)
                {
                    if (neighbor != pM.getCurPos())
                    {
                        GameObject nPiece = PieceInteractionManager.Instance.getPiece(neighbor);
                        if (nPiece != null)
                        {
                            PieceAttack neighborPiece = nPiece.GetComponent<PieceAttack>();
                            if (neighborPiece.pM.isFluct)
                            {
                                hasFluctNeighbor = true;
                                break;
                            }
                        }
                    }
                }
                if (hasFluctNeighbor) dmgReduc = 1;
                else dmgReduc = 0;
            }
        }
    }
}
