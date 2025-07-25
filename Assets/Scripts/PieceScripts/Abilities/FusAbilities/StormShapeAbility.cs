using UnityEngine;
using System.Collections.Generic;
using System;

public class StormShapeAbility : Ability
{
    /** * This class implements the StormShape ability, which allows a piece to target and damage other pieces within a range of 2 tiles.
     * It inherits from the Ability class and overrides the necessary methods to implement the specific logic for targeting and damaging pieces.
     * The ability can only be activated during the player's turn and targets pieces within a range of 2 tiles.
     */
    // This ability allows the player to target pieces within a range of 2 tiles and deal 1 true damage to them.
    private int turn=-1;
    [SerializeField] private int _abilityCost = 0;
    public override int abilityCost => _abilityCost;
    protected override void ActivateAbility()
    {
        /** * This method is called to activate the StormShape ability.
         * It checks if the ability can be activated, detects the tiles in range,
         * and updates the PieceInteractionManager to set the targets.
         */
        if (TurnManager.Instance.getTurnNumber()>turn+1 && TurnManager.Instance.isPlayerTurn(false)){
            if (PhaseManager.Instance.CombatPhase()){
                List<Vector3Int> targetsInRange = PieceMovement.detectTilesInRange(CurPos, 2, gameObject.GetComponent<PieceMovement>().tileMap);
                PieceInteractionManager.Instance.areTargeted(targetsInRange,false);
                PieceInteractionManager.Instance.targeter = gameObject;
            }
        } else {
            resetAbility(); // Reset the ability if it's not the player's turn or if the turn is not valid
        }
    }
    protected override void resetAbility()
    {
        isAbilityActive = false; // Reset the ability active state
        PieceInteractionManager.Instance.resetTargets(); // Reset the targets in PieceInteractionManager
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,false);

        if (abilityCasted) turn = TurnManager.Instance.getTurnNumber(); // Update the turn to the current turn
    }

    protected override void Update()
    {
        /** * This method is called every frame to check for user input and handle the ability activation.
         * If the left mouse button is clicked and the ability is active, it checks if the clicked tile is a valid target.
         * If it is, it deals 1 true damage to the target piece and plays a lightning animation.
         */
        base.Update(); // Call the base Update method to handle common functionality
        if (isAbilityActive && Input.GetMouseButtonDown(0)){
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePosition.x, mousePosition.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if (hit.collider != null)
            {
                GameObject hitObject = hit.collider.gameObject;
                if (PieceInteractionManager.Instance.isATarget(hitObject.GetComponent<PieceMovement>().getCurPos())){ //Verify if the hit object is a target
                    hit.collider.gameObject.GetComponent<PieceAttack>().trueDamage(1); // Decrease the LP of the targeted piece (true damage)
                    Vector3 animPos = hitObject.transform.position+ new Vector3(0, 0.5f, 0); // Adjust the position for the animation 
                    AnimationManager.Instance.animate(animPos, AnimationCode.stormShape); // Play the lightning animation
                    castAbility(); //Cast the ability and pay the cost
                    resetAbility();
                } else resetAbility(); // Reset the ability if the hit object is not a target
            } 
            else {
                resetAbility();
            }
        }
    }
}
