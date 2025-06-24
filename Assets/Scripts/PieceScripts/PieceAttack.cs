using UnityEngine;
using System.Collections.Generic;
using System;

public class PieceAttack : MonoBehaviour {
    /** PieceAttack is a MonoBehaviour that handles the attack logic of the piece.
     * It manages the attack range, damage, and other related parameters. 
     Overall it manages all attack-related logic of the piece.
     */

    [Range(1,12)]
    public int lp=6;
    
    [Range(1,3)]
    public int range=1;

    [Range(1,5)]
    public int baseAtk=2;
    public int nbAtk =1;
    public int d4Atk=0;
    public int dmgReduc=0;

    private int curTurn = 1; ///To check if the turn has changed, so we can refresh the attack
    internal PieceMovement pM; ///To reuse the map logic of PieceMovement
    private int curLp; ///Current LP of the piece, used to check if the piece is alive
    private int oldLp; ///Used to reset lp to the old value, just in case
    private int curNbAtk; ///Current number of attacks remaining for the piece, used to check if the piece can attack
    internal int curAtk; ///Current attack of the piece, used to check the atk after modifiers
    internal bool isAttacking=false; ///To check if the piece is attacking, used to manage the onMouseDown method
    private bool isDistracted = false; ///To check if the piece is distracted, so it can't attack
    private bool immunity = false; ///To check if the piece is immune to damage, used for special cases
    internal bool isProtected = false; ///To check if the piece is Protected, used for special cases
    //public methods
    public bool lpChanged => curLp != oldLp; ///To check if the LP changed
    public void resetLp(){
        curLp = oldLp; ///Reset the LP to the old value
    }
    public void setImmune(){ ///Set the piece as immune, so it can't take damage
        immunity = true;
    }
    public void Protect(int reduc){ 
        dmgReduc = reduc; 
        isProtected = true; ///Set the piece as Protected, so it can take less damage
    } //Set the damage reduction, used for special cases
    public bool IsDistracted() => isDistracted;
    public void decrCurNbAtk(){curNbAtk--;} ///Decrement the current number of attacks remaining for the piece.
    public int getCurLp() => curLp; 
    public int getCurNbAtk() =>curNbAtk;
    public void boostAttack(int boost){ ///Boost the attack of the piece, used for special cases
        curAtk += boost;
    }
    public void damage(int dmg){ ///Deal damage to the piece, used for attack logic
        if (!immunity) {
            curLp-=(dmg-dmgReduc);
            AnimationManager.Instance.animate(pM.tileMap.GetCellCenterWorld(pM.getCurPos()), AnimationCode.damage, dmg-dmgReduc); //Animate the damage
        }
        else {
            AnimationManager.Instance.animate(pM.tileMap.GetCellCenterWorld(pM.getCurPos()), AnimationCode.damage); //Animate the immunity
            immunity = false; ///If the piece is immune, it will not take damage, but the immunity will be removed
        }
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
    public void trueDamage(int dmg){ ///Deal true damage to the piece, not used for now
        if (!immunity) {
            curLp-=dmg;
            AnimationManager.Instance.animate(pM.tileMap.GetCellCenterWorld(pM.getCurPos()), AnimationCode.damage, dmg); //Animate the damage
        }
        else {
            immunity = false; 
            AnimationManager.Instance.animate(pM.tileMap.GetCellCenterWorld(pM.getCurPos()), AnimationCode.damage); //Animate the immunity
        }
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

    public void distractPiece(){ ///Distract the piece, so it can't attack
        isDistracted = true;
    }
    public void focusPiece(){ ///Focus the piece, so it can attack again
        isDistracted = false;
    }
    public void heal(int regen){ ///Heal the piece, used for special cases
        AnimationManager.Instance.animate(pM.tileMap.GetCellCenterWorld(pM.getCurPos()), AnimationCode.damage, -regen); //Animate the heal
        curLp+=regen;
        if (curLp>lp) curLp=lp;
    }

    //Private methods
    
    private void refreshAttack(){ ///Refresh the attack parameters of the piece, used to check if the piece can attack
        if (curTurn<TurnManager.Instance.getTurnNumber() && TurnManager.Instance.isPlayerTurn(pM.isFluct)){
            curNbAtk=nbAtk;
            curAtk = baseAtk;
            oldLp = curLp; ///Save the current LP to check if it changed
            curTurn=TurnManager.Instance.getTurnNumber();
            if (isProtected){
                isProtected = false; ///if the piece was protected reset the protection after the turn ends
                dmgReduc = 0; ///and reset the damage reduction after the turn ends
            }
        }
    }

    private List<Vector3Int> getReachableTargets(){ //Created after pM.detectTilesInRange, not worth modifying
        /** getReachableTargets() returns a list of Vector3Int that are reachable by the piece, 
         * based on its range and the tiles occupied by other pieces.
         * It uses the PieceInteractionManager to get the attack targets.
         */
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
        /** OnMouseDown is called when the player clicks on the piece.
         * It checks if the piece is locked, distracted,if an interruption is active and if a piece is casting, and if not, it checks if it's the player's turn.
         * If it's the player's turn and it's the combat phase, it sets the piece as attacking and updates its state.
         * If it's not the player's turn, it checks if the piece is attacked and handles the attack logic accordingly.
         If the piece has the Deepest or Magnified ability, it checks if the ability can be used and interrupts the attack if possible.
         */
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
        /**
            * Update is called once per frame.
            * It checks if the turn has changed, and if so, refreshes the attack parameters.
            * It also checks if the piece is attacking and if the mouse button is pressed, and if so, it checks if the piece is a Lancer and has a fluct neighbor.
            * If the piece is a Lancer and has a fluct neighbor, it sets the damage reduction to 1, otherwise it sets it to 0.
            */
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
        if (gameObject.name.Contains("Lancer") && !isProtected) { //If the piece is a Lancer and not protected
                //This logic was created AFTER I created hasNeighbor<T>(), that's why I didn't use it
                List<Vector3Int> neighbors = PieceMovement.detectTilesInRange(pM.getCurPos(), 1,pM.tileMap); //Get neighboors in range 1
                foreach (Vector3Int neighbor in neighbors) {
                    if (neighbor != pM.getCurPos()) { //Avoid checking the current position
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
