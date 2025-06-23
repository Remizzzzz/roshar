using UnityEngine;
using System.Collections.Generic;
using utils;

public class LightweaverAbility : Ability
{

    /** * This class implements the Lightweaver ability, which allows a piece to deal damage to an enemy piece.
     * It inherits from the Ability class and overrides the necessary methods to implement the specific logic for dealing damage.
     * The ability can only be activated during the player's turn and targets enemies within a range of 1 tile.
     */
    //Ability specific properties
    public int abilityDamage = 6;
    List<Vector3Int> abilityTargets=new();

    //Inherited properties
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        /** * This method is called to activate the Lightweaver ability.
         * It checks if the ability can be activated, sets the targets, and updates the piece state.
         * If the ability is successfully activated, it sets the isAbilityActive flag to true.
         */
        if (TurnManager.Instance.isPlayerTurn(true)){
            abilityTargets = PieceMovement.detectTilesInRange(CurPos, 1, gameObject.GetComponent<PieceMovement>().tileMap);
            PieceInteractionManager.Instance.setTargeter(gameObject); // Set the targeter to this piece
            abilityTargets = PieceInteractionManager.Instance.areTargeted(abilityTargets, true);
        } else {
            resetAbility();
        }
    }

    protected override void resetAbility()
    {
        abilityTargets.Clear(); // Clear the ability targets list
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
        PieceInteractionManager.Instance.resetTargets(); // Reset the targets in PieceInteractionManager
        isAbilityActive = false;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start(){
    }

    // Update is called once per frame
    protected override void Update(){
        /** * This method is called every frame to check for user input and handle the ability activation.
         * If the left mouse button is clicked and the ability is active, it checks if the clicked tile is a valid target.
         * If it is, it deals damage to the target piece and resets the ability.
         */
        base.Update();
        if (Input.GetMouseButtonDown(0) && isAbilityActive) // 0 = left click
        {
            Vector3Int mousePosition = Utils.getMousePositionOnTilemap(gameObject.GetComponent<PieceMovement>().tileMap);
            if (abilityTargets.Contains(mousePosition) && mousePosition != CurPos)
            {
                GameObject targetPiece = PieceInteractionManager.Instance.getPiece(mousePosition);
                targetPiece.GetComponent<PieceAttack>().damage(abilityDamage); // Deal damage to the target piece
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
