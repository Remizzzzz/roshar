using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using utils;

public class PursuerAbility : Ability
{
    /** * This class implements the Pursuer ability, which allows a piece to move to a tile and boost its attack.
     * It inherits from the Ability class and overrides the necessary methods to implement the specific logic for moving and boosting attack.
     * The ability can only be activated during the player's turn and targets tiles within a range of 1 tile.
     */
    //Ability specific properties
    private List<Vector3Int> abilityTargets=new(); 
    //Inherited properties
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        if (TurnManager.Instance.isPlayerTurn(false)){ //If it's fus turn
            List<Vector3Int> enemies = PieceInteractionManager.Instance.getTargetOnMap(true);
            Tilemap tileMap = gameObject.GetComponent<PieceMovement>().tileMap;
            foreach (Vector3Int enemy in enemies){
                List<Vector3Int> neighbours = PieceMovement.detectTilesInRange(enemy,1,tileMap);
                foreach (Vector3Int neighbour in neighbours){
                    if (TileStateManager.Instance.getState(neighbour) != TileState.occupied){
                        TileStateManager.Instance.updateState(neighbour,TileState.reachable);
                        tileMap.RefreshTile(neighbour);
                        if (!abilityTargets.Contains(neighbour)) abilityTargets.Add(neighbour);
                    }
                }
            }
        } else resetAbility();
    }

    protected override void resetAbility()
    {
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
        TileStateManager.Instance.resetMap();
        abilityTargets.Clear();
        isAbilityActive = false;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start(){
    }

    // Update is called once per frame
    protected override void Update(){
        /** * This method is called every frame to check for user input and handle the ability activation.
         * If the left mouse button is clicked and the ability is active, it checks if the clicked tile is a valid target.
         * If it is, it moves the piece to that tile and boosts its attack, then resets the ability.
         */
        base.Update();
        if (Input.GetMouseButtonDown(0) && isAbilityActive) // 0 = left click
        {
            Vector3Int mousePosition = Utils.getMousePositionOnTilemap(gameObject.GetComponent<PieceMovement>().tileMap);
            if (abilityTargets.Contains(mousePosition)){
                gameObject.GetComponent<PieceMovement>().moveTo(mousePosition);
                gameObject.GetComponent<PieceAttack>().boostAttack(1);
                castAbility(); // Cast the ability and pay the cost
                resetAbility(); // Reset the ability after moving
            } else {
                resetAbility(); // Reset the ability if the target is not valid
            }
        }
    }
}
