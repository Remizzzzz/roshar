using UnityEngine;
using System.Collections.Generic;
using utils;

public class WindrunnerAbility : Ability
{
    //Specific properties for Windrunner ability
    Dictionary<GameObject,int> lockedPieces = new();
    public int turnsToLock = 2; // Number of turns to lock the piece

    // General unherited properties
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;
    private List<Vector3Int> abilityTargets;
    protected override void ActivateAbility()
    {
        if (TurnManager.Instance.isPlayerTurn(true)){ 
            abilityTargets = PieceInteractionManager.Instance.getTargetOnMap(false); //Windrunners are Fluct, so the ability targets Fus
            Debug.Log("Windrunner ability activated. Targets: " + abilityTargets.Count);
            PieceInteractionManager.Instance.setTargeter(gameObject); // Set the targeter to this piece
            PieceInteractionManager.Instance.areTargeted(abilityTargets, true);
            
        } else {
            resetAbility(); // Reset the ability if it's not the player's turn
        }
    }

    protected override void resetAbility()
    {
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
        isAbilityActive = false;
        PieceInteractionManager.Instance.resetTargets(); // Reset the targets in PieceInteractionManager
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
                targetPiece.GetComponent<PieceMovement>().lockPiece(); // Lock the target piece
                PieceStateManager.Instance.updateState(targetPiece, PieceState.locked, targetPiece.GetComponent<PieceMovement>().isFluct);
                lockedPieces.Add(targetPiece,TurnManager.Instance.getTurnNumber());
                castAbility(); // Cast the ability and pay the cost
                resetAbility(); // Reset the ability after locking the piece
            }
            else
            {
                PieceInteractionManager.Instance.resetTargets();
                resetAbility(); // Reset the ability if the target is not valid
            }
        }

        GameObject del = null;
        foreach (GameObject piece in lockedPieces.Keys)
        {
            if (TurnManager.Instance.getTurnNumber() - lockedPieces[piece] >= turnsToLock) // Check if the lock duration has passed
            {
                piece.GetComponent<PieceMovement>().unlockPiece();
                PieceStateManager.Instance.updateState(piece, PieceState.basic, piece.GetComponent<PieceMovement>().isFluct);
                del = piece; // Mark the piece for removal
            }
        }
        if (del != null){
            lockedPieces.Remove(del); // Remove the piece from the locked pieces dictionary
        }
    }
}
