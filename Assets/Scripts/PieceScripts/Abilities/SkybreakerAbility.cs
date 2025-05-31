using UnityEngine;
using System.Collections.Generic;
using utils;

public class SkybreakerAbility : Ability
{
    //Specific properties for Skybreaker ability
    private int calculateDamage(){
        return Utils.launchD4(1)+1;
    }
    //inherited properties
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        if (TurnManager.Instance.isPlayerTurn(true)){
            List<Vector3Int> targetsInRange = PieceMovement.detectTilesInRange(CurPos, 1, gameObject.GetComponent<PieceMovement>().tileMap);
        }
    }

    protected override void resetAbility()
    {
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
        // TODO: Implement the reset logic
        isAbilityActive = false;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start(){
    }

    // Update is called once per frame
    protected override void Update(){
        base.Update();
    }
}
