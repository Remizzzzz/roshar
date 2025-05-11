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

    private PieceMovement pM; //To reuse the map logic of PieceMovement
    private int curLp;
    private int dmgReduc=0;
    private int curNbAtk;
    public void damage(int dmg){
        curLp-=(dmg-dmgReduc);
        if (curLp<=0) {
            PieceInteractionManager.Instance.remove(pM.getCurPos());
            PieceStateManager.Instance.remove(gameObject);
            Destroy(gameObject);
        }
    }
    public void setDmgReduc(int dmgRed){dmgReduc=dmgRed;}
    public void heal(int regen){
        curLp+=regen;
        if (curLp>lp) curLp=lp;
    }
    //public attackAction();
    //Private methods
    private List<Vector3Int> getReachableTargets(){
        List<Vector3Int> tilesReachables= new List<Vector3Int>{pM.getCurPos()}; //The List where we'll keep our valid coordinates
            if(pM.getOnMap()){
                List<Vector3Int> search = new List<Vector3Int>(); //The tempList used for searching

                for (int i=0; i<range;i++){ //range determines the depth of search, each loop is one depth
                    foreach(Vector3Int rSearch in tilesReachables){ //Search will go through each neighboor of the tiles in tilesReachables, and add them to tilesReachables if they're not alreday in it
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
            //Now that we have all the occupied tiles in range, we select only the one with a valid target on it
            return PieceInteractionManager.Instance.getAttackTargets(tilesReachables,pM.isFluct);
    }

    //On Mouse methods
    void OnMouseDown(){
        if (TurnManager.Instance.isPlayerTurn(pM.isFluct) && PieceInteractionManager.Instance.combatModeEnabled()){
            if (!PieceStateManager.Instance.isAttacked(pM.isFluct) && PieceStateManager.Instance.isAttacked(!pM.isFluct)){//If it's not an attack, but a selection to attack (the PlayerTurn bool already indirectly check this condition, but for safety)
                // ! Note that the second bool actually check if another piece of the same color is already attacking rather than checking if the piece is attacked, it was useless to create a new method
                List<Vector3Int> targetsInRange = getReachableTargets();
                PieceInteractionManager.Instance.areTargeted(targetsInRange,pM.isFluct);
            }
        }
    }

    void Awake(){
        pM = GetComponent<PieceMovement>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curLp=lp;
        curNbAtk=nbAtk;
    }

    // Update is called once per frame
    void Update() 
    {
        
    }
}
