using UnityEngine;
using System.Collections.Generic;
using utils;

public class MaskedAbility : Ability
{
    //Ability specific properties
    public Dictionary<GameObject,int> distractedPieces = new();
    public int turnsToDistract = 2; // Number of turns to distract the piece
    private List<Vector3Int> abilityTargets=new();

    //Inherited properties
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        if (TurnManager.Instance.isPlayerTurn(false) && distractedPieces.Count < 2) // Check if it's the player's turn and if there are less than 2 distracted pieces
        { 
            abilityTargets = PieceInteractionManager.Instance.getTargetOnMap(true); // Masked pieces are Fus, so the ability targets Fluct
            PieceInteractionManager.Instance.setTargeter(gameObject); // Set the targeter to this piece
            abilityTargets = PieceInteractionManager.Instance.areTargeted(abilityTargets, false);
        } else {
            resetAbility(); // Reset the ability if it's not the player's turn
        }
    }

    protected override void resetAbility()
    {
        PieceInteractionManager.Instance.resetTargets(); // Reset the targets in PieceInteractionManager
        abilityTargets.Clear(); // Clear the ability targets list
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
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
            if (abilityTargets.Contains(mousePosition))
            {
                GameObject targetPiece = PieceInteractionManager.Instance.getPiece(mousePosition);
                targetPiece.GetComponent<PieceAttack>().distractPiece(); // distract the target piece
                PieceStateManager.Instance.updateState(targetPiece, PieceState.distracted, targetPiece.GetComponent<PieceMovement>().isFluct);
                distractedPieces.Add(targetPiece,TurnManager.Instance.getTurnNumber());
                castAbility(); // Cast the ability and pay the cost
                resetAbility(); // Reset the ability after locking the piece
            }
            else
            {
                resetAbility(); // Reset the ability if the target is not valid
            }
        }

        GameObject del = null;
        foreach (GameObject piece in distractedPieces.Keys)
        {
            if (TurnManager.Instance.getTurnNumber() - distractedPieces[piece] > turnsToDistract) // Check if the distract duration has passed
            {
                piece.GetComponent<PieceAttack>().focusPiece();
                PieceStateManager.Instance.updateState(piece, PieceState.basic, piece.GetComponent<PieceMovement>().isFluct);
                del = piece; // Mark the piece for removal
            }
        }
        if (del != null){
            distractedPieces.Remove(del); // Remove the piece from the locked pieces dictionary
        }
    }
}
