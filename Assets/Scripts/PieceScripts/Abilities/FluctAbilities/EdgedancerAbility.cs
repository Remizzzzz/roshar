using UnityEngine;
using System.Collections.Generic;
using utils;

public class EdgedancerAbility : Ability
{
    // Ability specific properties
    public int healAmount = 3;

    private List<Vector3Int> abilityTargets;
    //Inherited properties
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        if (TurnManager.Instance.isPlayerTurn(true)){
            abilityTargets = PieceMovement.detectTilesInRange(CurPos, 1, gameObject.GetComponent<PieceMovement>().tileMap);
            PieceInteractionManager.Instance.setTargeter(gameObject); // Set the targeter to this piece
            abilityTargets = PieceInteractionManager.Instance.areTargeted(abilityTargets, false ); //False because we want to heal allies
        } else {
            resetAbility();
        }
    }

    protected override void resetAbility()
    {
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
        PieceInteractionManager.Instance.resetTargets(true); // Reset the targets in PieceInteractionManager (reset for allies)
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
            if (abilityTargets.Contains(mousePosition) && mousePosition != CurPos)
            {
                GameObject targetPiece = PieceInteractionManager.Instance.getPiece(mousePosition);
                targetPiece.GetComponent<PieceAttack>().heal(healAmount); // heal the target piece
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
