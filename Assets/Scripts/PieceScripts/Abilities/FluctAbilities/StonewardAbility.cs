using UnityEngine;
using System.Collections.Generic;
using utils;

public class StonewardAbility : Ability
{
    //Ability specific properties
    public GameObject minionPrefab; // Prefab for the Stoneward minion
    private List<Vector3Int> summoningTiles = new(); // Tiles where the minion can be summoned
    private void summonMinion(Vector3Int mousePosition){
        // Instantiate the minion at the selected tile position
        GameObject minion = Instantiate(minionPrefab, gameObject.GetComponent<PieceMovement>().tileMap.GetCellCenterWorld(mousePosition), Quaternion.identity);
        minion.GetComponent<PieceMovement>().tileMap = gameObject.GetComponent<PieceMovement>().tileMap; // Set the tilemap for the minion

        //register the minion in the gameManagers
        PieceInteractionManager.Instance.addPiece(minion, minion.GetComponent<PieceMovement>().getCurPos(), true); // Update the interaction manager with the new minion
        PieceStateManager.Instance.addPiece(minion, true); // Add the minion to the piece state manager
        
        minion.GetComponent<PieceMovement>().setOnMap(true); // Set the minion as on the map
        minion.GetComponent<PieceMovement>().setCurPos(mousePosition); // Set the current position of the minion

        TileStateManager.Instance.updateState(minion.GetComponent<PieceMovement>().getCurPos(), TileState.occupied);
        
        WinCondition.Instance.UpdateFluctOnMap(true); //Update the number of pieces on the map
    }
    //Inherited properties
    [SerializeField] private int _abilityCost = 2;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        if (TurnManager.Instance.isPlayerTurn(true) && !PhaseManager.Instance.MovementPhase()){
            List<Vector3Int> possibleTiles = PieceMovement.detectTilesInRange(CurPos, 1, gameObject.GetComponent<PieceMovement>().tileMap);
            foreach (Vector3Int tilePos in possibleTiles){
                if (TileStateManager.Instance.getState(tilePos) == TileState.occupied) continue; // Skip occupied tiles
                else {
                    TileStateManager.Instance.updateState(tilePos,TileState.summoningTile);
                    gameObject.GetComponent<PieceMovement>().tileMap.RefreshTile(tilePos);
                    summoningTiles.Add(tilePos); // Add the tile to the summoning tiles list
                }
            }
        } else {
            resetAbility();
        }
    }

    protected override void resetAbility()
    {
        summoningTiles.Clear(); // Clear the summoning tiles list
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
        TileStateManager.Instance.resetMap(); // Reset the tile states
        isAbilityActive = false;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start(){
    }

    // Update is called once per frame
    protected override void Update(){
        base.Update();
        if (Input.GetMouseButtonDown(0) && isAbilityActive) // 0 = left click
        {
            Vector3Int mousePosition = Utils.getMousePositionOnTilemap(gameObject.GetComponent<PieceMovement>().tileMap);
            if (summoningTiles.Contains(mousePosition) && mousePosition != CurPos)
            {
                summonMinion(mousePosition); // Summon the minion at the selected tile position
                PieceStateManager.Instance.updateState(gameObject, PieceState.basic, gameObject.GetComponent<PieceMovement>().isFluct); // Update the piece state
                castAbility(); // Cast the ability and pay the cost
                resetAbility(); // Reset the ability after damaging the piece
            }
            else
            {
                PieceInteractionManager.Instance.resetTargets();
                resetAbility(); // Reset the ability if the target is not valid
            }
        }
    }
}
