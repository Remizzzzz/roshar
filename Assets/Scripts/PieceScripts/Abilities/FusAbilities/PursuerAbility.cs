using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using utils;

public class PursuerAbility : Ability
{
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
