using UnityEngine;
using System.Collections.Generic;
using utils;

public class WindrunnerAbility : Ability
{
    // This ability allows the player to lock a piece for a certain number of turns, preventing it from moving.

    //Specific properties for Windrunner ability
    Dictionary<GameObject,int> lockedPieces = new();
    public int turnsToLock = 2; // Number of turns to lock the piece
    private List<Vector3Int> abilityTargets=new();


    // General inherited properties
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;
    protected override void ActivateAbility()
    {
        if (TurnManager.Instance.isPlayerTurn(true)){ 
            abilityTargets = PieceInteractionManager.Instance.getTargetOnMap(false); //Windrunners are Fluct, so the ability targets Fus
            PieceInteractionManager.Instance.setTargeter(gameObject); // Set the targeter to this piece
            abilityTargets = PieceInteractionManager.Instance.areTargeted(abilityTargets, true);
            
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
                targetPiece.GetComponent<PieceMovement>().lockPiece(); // Lock the target piece
                PieceStateManager.Instance.updateState(targetPiece, PieceState.locked, targetPiece.GetComponent<PieceMovement>().isFluct);
                lockedPieces.Add(targetPiece,TurnManager.Instance.getTurnNumber());
                castAbility(); // Cast the ability and pay the cost
                resetAbility(); // Reset the ability after locking the piece
            }
            else
            {
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
