using UnityEngine;
using System.Collections.Generic;
using utils;

public class DeepestAbility : Ability
{
    /** * This class implements the Deepest ability, which allows a piece to attack an enemy piece.
     * It inherits from the Ability class and overrides the necessary methods to implement the specific logic for attacking.
     * The ability can only be activated during the player's turn and targets enemy pieces within a range of 1 tile.
     * It immunes the piece from damage for one attack.
     */
    //Ability specific properties
    public GameObject abilityWindow; /// Window to be displayed when the ability starts an interruption
    private GameObject attacker; /// The piece that is currently attacking

    public bool getAbilityCasted() => abilityCasted; // Getter for abilityCasted, used to check if the ability was casted
    public int getVoidlight() => voidlight; // Getter for voidlight, used to check if the ability can be activated
    public void acceptActivation(){ /// Method to accept the activation of the ability
        if (voidlight >= abilityCost){ //Double check to ensure enough voidlight is available
            ActivateAbility();
        } else cancelActivation(); // If not enough voidlight, cancel the activation
    }
    public void cancelActivation(){ /// Method to cancel the activation of the ability
        //In case PieceInteractionManager reset its properties
        PieceInteractionManager.Instance.setTargeter(attacker);
        PieceInteractionManager.Instance.addTarget(CurPos); // Re-add the current position to the target list

        PieceInteractionManager.Instance.attack(CurPos, false); // Proceed with the attack
        resetAbility();
    }
    public void interruptAttack(){ /// Method to interrupt the attack and display the ability window
        attacker = PieceInteractionManager.Instance.targeter;
        if (abilityWindow != null) {
            InterruptionManager.Instance.createWindow(gameObject, abilityWindow);
        }
    }
    //Inherited properties
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        /** * This method is called to activate the Deepest ability.
         * It checks if the ability can be activated, sets the attacker, and updates the piece state.
         * If the ability is successfully activated, it sets the isAbilityActive flag to true.
         */
        GetComponent<PieceAttack>().setImmune();
        PieceInteractionManager.Instance.attack(CurPos, false); // Call the attack method with false to indicate it's a fus piece
        castAbility(); // Cast the ability to deduct the cost
        resetAbility(); // Reset the ability after activation
    }

    protected override void resetAbility()
    {
        attacker = null; // Reset the attacker to null
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
        isAbilityActive = false;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start(){
    }

    // Update is called once per frame
    protected override void Update(){
        restore();
        restoreAbility();
    }
}
