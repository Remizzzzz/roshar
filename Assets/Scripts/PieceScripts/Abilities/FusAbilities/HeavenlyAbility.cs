using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using utils;

public class HeavenlyAbility : Ability
{
    /** * This class implements the Heavenly ability, which allows a piece to teleport allies to a tile.
     * It inherits from the Ability class and overrides the necessary methods to implement the specific logic for teleporting allies.
     * The ability can only be activated during the player's turn and targets allies within a range of 1 tile.
     */
    //Ability specific properties
    public int alliesTeleportable = 2; /// Number of allies that can be teleported
    public GameObject selectedSpriteAppearanceOne; /// AppearanceSprite for the first part of the ability
    public GameObject selectedSpriteAppearanceTwo; /// AppearanceSprite for the second part of the ability
    private List<GameObject> selectionSpriteList=new(); /// List to hold the selection sprites for allies
    private List<Vector3Int> abilityTargets=new();  /// List of tiles that can be targeted by the ability
    private List<PieceMovement> targetedAllies = new List<PieceMovement>(); /// List of pieces targeted by the ability
    private bool isTargetingAllies = false; /// Flag to indicate if the ability targets are allies
    private bool isTargetingTiles = true; /// Flag to indicate if the ability targets are tiles
    private int alliesTargeted = 0;
    private void selectAlly(Vector3Int pos){ 
        /** * This method is called to select an ally at the specified position.
         * It instantiates the appropriate selection sprite based on the number of allies targeted
         * and adds it to the selectionSpriteList.
         */
        if (alliesTargeted ==1){
            GameObject selectedOne = Instantiate(selectedSpriteAppearanceOne);
            selectedOne.transform.position = gameObject.GetComponent<PieceMovement>().tileMap.GetCellCenterWorld(pos);
            selectionSpriteList.Add(selectedOne);
        } else if (alliesTargeted ==2){
            GameObject selectedTwo = Instantiate(selectedSpriteAppearanceTwo);
            selectedTwo.transform.position = gameObject.GetComponent<PieceMovement>().tileMap.GetCellCenterWorld(pos);
            selectionSpriteList.Add(selectedTwo);
        }
        
    }
    private void switchPhase(){ /// Switches the targeting phase of the ability
        isTargetingAllies = !isTargetingAllies;
        isTargetingTiles = !isTargetingTiles;
    }
    private void detectAllies(){
        /** * This method is called to detect allies within a range of 1 tile.
         * It checks if the ability is targeting allies, and if so, it sets the targets accordingly.
         * If the ability is targeting tiles, it switches the phase to tile targeting.
         */
        if (isTargetingTiles) switchPhase();
        if (TurnManager.Instance.isPlayerTurn(false)){
            abilityTargets = PieceMovement.detectTilesInRange(CurPos, 1, gameObject.GetComponent<PieceMovement>().tileMap);
            PieceInteractionManager.Instance.setTargeter(gameObject); // Set the targeter to this piece
            abilityTargets = PieceInteractionManager.Instance.areTargeted(abilityTargets, true);
        }
    }
    private void detectTiles(){
        /** * This method is called to detect tiles where allies can be teleported.
         * It checks if the ability is targeting tiles, and if so, it updates the tile states
         * and adds the tiles to the ability targets list.
         */
        if (!isTargetingTiles) switchPhase();
        abilityTargets.Clear(); // Clear the ability targets list
        Tilemap tileMap = gameObject.GetComponent<PieceMovement>().tileMap;
        BoundsInt bounds = tileMap.cellBounds;
        foreach (Vector3Int pos in bounds.allPositionsWithin) //Set the boundaries of the map
        {
            if (tileMap.HasTile(pos) && TileStateManager.Instance.isNotOccupied(pos)) //Go through each tile and reset those that aren't occupied
            {
                TileStateManager.Instance.updateState(pos,TileState.reachable);
                tileMap.RefreshTile(pos);
                abilityTargets.Add(pos); // Add the tile to the ability targets list
            }
        }
    }
    //Inherited properties
    [SerializeField] private int _abilityCost = 2;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        detectAllies();
    }

    protected override void resetAbility()
    {
        foreach(GameObject selectionSprite in selectionSpriteList){
            Destroy(selectionSprite);
        }
        alliesTargeted=0;
        selectionSpriteList.Clear();
        TileStateManager.Instance.resetMap(); // Reset the tile states
        PieceInteractionManager.Instance.resetTargets(true); // Reset the targets
        abilityTargets.Clear(); // Clear the ability targets list
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
        isAbilityActive = false;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start(){
    }

    // Update is called once per frame
    protected override void Update(){
        /** * This method is called every frame to check for user input and handle the ability activation.
         * If the left mouse button is clicked and the ability is active, it checks if the clicked tile is a valid target.
         * If it is, it either selects an ally or teleports an ally to the selected tile.
         */
        base.Update();
        if (Input.GetMouseButtonDown(0) && isAbilityActive){
            Vector3Int mousePosition = Utils.getMousePositionOnTilemap(gameObject.GetComponent<PieceMovement>().tileMap);

            if (isTargetingAllies){
                if (abilityTargets.Contains(mousePosition) && mousePosition != CurPos){
                    GameObject targetPiece = PieceInteractionManager.Instance.getPiece(mousePosition);
                    if (!targetedAllies.Contains(targetPiece.GetComponent<PieceMovement>())){
                        targetedAllies.Add(targetPiece.GetComponent<PieceMovement>()); // Add the targeted piece to the list
                    }
                    alliesTargeted++;
                    selectAlly(mousePosition);
                    if (alliesTargeted >= alliesTeleportable){
                        PieceInteractionManager.Instance.resetTargets(true); // Reset the targets
                        alliesTargeted = targetedAllies.Count; // Set the number of targeted allies to the count of the list
                        detectTiles(); // Switch to tile targeting phase
                    }
                } else {
                    resetAbility(); // Reset the ability if the mouse position is not valid
                }
            } else {
                if (abilityTargets.Contains(mousePosition)){
                    PieceMovement ally = targetedAllies[0]; // Get the first targeted ally
                    targetedAllies.RemoveAt(0); // Remove the ally from the list
                    ally.moveTo(mousePosition); // Move the ally to the selected tile
                    if (targetedAllies.Count == 0){
                        castAbility();
                        resetAbility(); // Reset the ability
                    } else {
                        detectTiles(); // Continue targeting tiles for the next ally
                    }
                } else {
                    if (alliesTargeted > targetedAllies.Count){
                        castAbility(); // Cast the ability if the mouse position is not valid
                        resetAbility();
                    } else {
                        resetAbility(); // Reset the ability if the mouse position is not valid
                    }
                }
            }
        }
    }
}
