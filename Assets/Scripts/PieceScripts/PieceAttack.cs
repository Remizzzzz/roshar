using UnityEngine;
using System.Collections.Generic;
using System;

public class PieceAttack : MonoBehaviour {

    [Range(1,12)]
    public int lp=6;
    
    [Range(1,3)]
    public int range=1;

    [Range(1,5)]
    public int baseAtk=2;
    public int nbAtk =1;
    public int d4Atk=0;
    public int dmgReduc=0;

    private int curTurn = 1;
    internal PieceMovement pM; //To reuse the map logic of PieceMovement
    private int curLp;
    private int oldLp; //Used to reset if a wrong priority order is made
    private int curNbAtk;
    internal int curAtk;
    internal bool isAttacking=false;
    private bool isDistracted = false; //To check if the piece is distracted, so it can't attack
    private bool immunity = false; //To check if the piece is immune to damage, used for special cases
    private bool Protected = false; //To check if the piece is protected, used for special cases
    //public methods
    public bool lpChanged => curLp != oldLp; //To check if the LP changed
    public void resetLp(){
        curLp = oldLp; //Reset the LP to the old value
    }
    public void setImmune(){
        immunity = true;
    }
    public void isProtected(int reduc){ 
        dmgReduc = reduc; 
        Protected = true; //Set the piece as protected, so it can take less damage
    } //Set the damage reduction, used for special cases
    public bool IsDistracted() => isDistracted;
    public void decrCurNbAtk(){curNbAtk--;}
    public int getCurLp() => curLp;
    public int getCurNbAtk() =>curNbAtk;
    public void boostAttack(int boost){
        curAtk += boost;
    }
    public void damage(int dmg){
        if (!immunity) curLp-=(dmg-dmgReduc);
        else immunity = false; //If the piece is immune, it will not take damage, but the immunity will be removed
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
    public void trueDamage(int dmg){
        if (!immunity) curLp-=dmg;
        else immunity = false; 
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

    public void distractPiece(){
        isDistracted = true;
    }
    public void focusPiece(){
        isDistracted = false;
    }
    public void heal(int regen){
        curLp+=regen;
        if (curLp>lp) curLp=lp;
    }

    //Private methods
    
    private void refreshAttack(){
        if (curTurn<TurnManager.Instance.getTurnNumber() && TurnManager.Instance.isPlayerTurn(pM.isFluct)){
            curNbAtk=nbAtk;
            curAtk = baseAtk;
            oldLp = curLp; //Save the current LP to check if it changed
            curTurn=TurnManager.Instance.getTurnNumber();
            if (Protected){
                Protected = false; //Reset the protection after the turn ends
                dmgReduc = 0; //Reset the damage reduction after the turn ends
            }
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
        if (TurnManager.Instance.isPlayerTurn(pM.isFluct) && PhaseManager.Instance.CombatPhase() && !isDistracted && !PieceStateManager.Instance.isCasting() && !InterruptionManager.Instance.isInterruptionActive()){ //If it's the piece's players turn and combat mode is activated, it's more likely a selection
            if (!PieceStateManager.Instance.isAttacked(pM.isFluct) && !PieceStateManager.Instance.isAttacked(!pM.isFluct)){//If it's not an attack, but a selection to attack (the PlayerTurn bool already indirectly check this condition, but for safety)
                // ! Note that the second bool actually check if another piece of the same color is already attacking rather than checking if the piece is attacked, it was useless to create a new method
                if (curNbAtk>0){ //If the piece has an attack remaining this turn
                    List<Vector3Int> targetsInRange = getReachableTargets();
                    PieceInteractionManager.Instance.areTargeted(targetsInRange,pM.isFluct);
                    PieceInteractionManager.Instance.setTargeter(gameObject);
                    PieceStateManager.Instance.updateState(gameObject, PieceState.attacking,pM.isFluct);
                    isAttacking=true;
                }
            }
        } else if (PhaseManager.Instance.CombatPhase()){//If it's not the turn of the piece's player, that can means the piece is attacked
            if (PieceStateManager.Instance.isAttacked(pM.isFluct)){ //If the piece is attacked
                Debug.Log(PieceInteractionManager.Instance.hasNeighbor<MagnifiedAbility>(pM.getCurPos(), true, pM.isFluct));
                
                //Interruption for attack
                if (GetComponent<DeepestAbility>() != null) {
                    if (GetComponent<DeepestAbility>().getVoidlight() >= GetComponent<DeepestAbility>().abilityCost && !GetComponent<DeepestAbility>().getAbilityCasted()) GetComponent<DeepestAbility>().interruptAttack(); //If the piece has the Deepest ability, interrupt the attack
                    else PieceInteractionManager.Instance.attack(pM.getCurPos(),pM.isFluct);
                } else if (PieceInteractionManager.Instance.hasNeighbor<MagnifiedAbility>(pM.getCurPos(), true, pM.isFluct)){
                    Debug.Log("Neighbor has Magnified Ability");
                    MagnifiedAbility magAbility = PieceInteractionManager.Instance.getNeighborComponent<MagnifiedAbility>(pM.getCurPos(), true, pM.isFluct);
                    if (magAbility.getVoidlight() >= magAbility.abilityCost && !magAbility.getAbilityCasted()) {
                        Debug.Log("Interrupting attack with Magnified Ability");
                        magAbility.interruptAttack(pM.getCurPos()); //If the neighbor has the Magnified ability, interrupt the attack
                    } else PieceInteractionManager.Instance.attack(pM.getCurPos(),pM.isFluct);
                } else if (GetComponent<MagnifiedAbility>()!=null){
                    if (GetComponent<MagnifiedAbility>().getVoidlight() >= GetComponent<MagnifiedAbility>().abilityCost && !GetComponent<MagnifiedAbility>().getAbilityCasted()) {
                        GetComponent<MagnifiedAbility>().interruptAttack(pM.getCurPos()); //If the piece has the Magnified ability, interrupt the attack
                    } else PieceInteractionManager.Instance.attack(pM.getCurPos(),pM.isFluct);
                }


                //Normal attack procedure
                else PieceInteractionManager.Instance.attack(pM.getCurPos(),pM.isFluct);
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
        curAtk = baseAtk;

    }

    // Update is called once per frame
    void Update() 
    {
        refreshAttack();
        //The following code seems to not be working, I'll check into that later
        if (isAttacking && Input.GetMouseButtonDown(0) && !InterruptionManager.Instance.isInterruptionActive()) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get Mouse Position
            mousePosition.z = 0; 
            Vector3Int cellPos = pM.tileMap.WorldToCell(mousePosition);
            if (!PieceInteractionManager.Instance.isATarget(cellPos) && cellPos!=pM.getCurPos()){
                PieceStateManager.Instance.updateState(gameObject,PieceState.basic,pM.isFluct);
                PieceInteractionManager.Instance.resetTargets();
                PieceInteractionManager.Instance.targeter=null;
                isAttacking=false;
            }
        }

        bool hasFluctNeighbor = false; //Flag to check if a neighbor is a fluct piece
        if (pM.isFluct && !Protected) {
            if (!pM.isSpecial && !pM.enhancedTroop) {  //If the troop is basic

                //This all logic was created AFTER I created hasNeighbor<T>(), that's why I didn't use it
                List<Vector3Int> neighbors = PieceMovement.detectTilesInRange(pM.getCurPos(), 1,pM.tileMap); //Get neighboors in range 1
                foreach (Vector3Int neighbor in neighbors) {
                    if (neighbor != pM.getCurPos()) {//Avoid checking the current position
                        GameObject nPiece = PieceInteractionManager.Instance.getPiece(neighbor);
                        if (nPiece != null){
                            PieceAttack neighborPiece = nPiece.GetComponent<PieceAttack>();
                            if (neighborPiece.pM.isFluct) {
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
