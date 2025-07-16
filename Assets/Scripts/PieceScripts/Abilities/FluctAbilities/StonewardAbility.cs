using UnityEngine;
using System.Collections.Generic;
using utils;

public class StonewardAbility : Ability
{
    /** * This class implements the Stoneward ability, which allows a piece to summon a minion on a tile.
     * It inherits from the Ability class and overrides the necessary methods to implement the specific logic for summoning a minion.
     * The ability can only be activated during the player's turn and targets tiles within a range of 1 tile.
     */
    //Ability specific properties
    public GameObject minionPrefab; /// Prefab for the Stoneward minion
    private List<Vector3Int> summoningTiles = new(); /// Tiles where the minion can be summoned
    private int minionCount = 0; /// Counter for the number of minions summoned
    public void minionDestroyed()
    {
        minionCount--; /// Decrement the minion count when a minion is destroyed
    }
    private void summonMinion(Vector3Int mousePosition){
        /** * This method is called to summon a minion on the selected tile position.
         * It checks if the tile is valid for summoning, instantiates the minion prefab,
         * and updates the necessary managers to register the new minion.
         */
        // Instantiate the minion at the selected tile position
        GameObject minion = Instantiate(minionPrefab, gameObject.GetComponent<PieceMovement>().tileMap.GetCellCenterWorld(mousePosition), Quaternion.identity);
        minion.GetComponent<PieceMovement>().tileMap = gameObject.GetComponent<PieceMovement>().tileMap; // Set the tilemap for the minion
        minion.GetComponent<PieceAttributes>().isMinion = true; // Set the minion as a minion
        minion.GetComponent<PieceAttributes>().setSummoner(gameObject); // Set the summoner of the minion to the current piece

        //register the minion in the gameManagers
        PieceInteractionManager.Instance.addPiece(minion, minion.GetComponent<PieceMovement>().getCurPos(), true); // Update the interaction manager with the new minion
        PieceStateManager.Instance.addPiece(minion, true); // Add the minion to the piece state manager
        
        minion.GetComponent<PieceMovement>().setOnMap(true); // Set the minion as on the map
        minion.GetComponent<PieceMovement>().setCurPos(mousePosition); // Set the current position of the minion

        TileStateManager.Instance.updateState(minion.GetComponent<PieceMovement>().getCurPos(), TileState.occupied);
        
        WinCondition.Instance.UpdateFluctOnMap(true); //Update the number of pieces on the map
        minionCount++; // Increment the minion count
    }
    //Inherited properties
    [SerializeField] private int _abilityCost = 2;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        /** * This method is called to activate the Stoneward ability.
         * It checks if the ability can be activated, detects the tiles in range,
         * and updates the tile states to indicate summoning tiles.
         */
        if (TurnManager.Instance.isPlayerTurn(true) && !PhaseManager.Instance.MovementPhase() && minionCount < 2){
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
        /** * This method is called every frame to check for user input and handle the ability activation.
         * If the left mouse button is clicked and the ability is active, it checks if the clicked tile is a valid summoning tile.
         * If it is, it summons a minion at the selected tile position and resets the ability.
         */
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
