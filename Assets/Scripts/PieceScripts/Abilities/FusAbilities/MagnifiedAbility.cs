using UnityEngine;
using System.Collections.Generic;
using utils;

public class MagnifiedAbility : Ability
{

    /** * This class implements the Magnified ability, which allows a piece to protect allies.
     * It inherits from the Ability class and overrides the necessary methods to implement the specific logic for protection and attack.
     * The ability can only be activated during each player's turn and targets allies within a range of 1 tile.
     */

    //Interruption logic
    
    private GameObject attacker=null;
    private Vector3Int defender; /// Position of the defender piece, use for interruption logic

    public bool getAbilityCasted() => abilityCasted; /// Getter for abilityCasted, use to check if the ability was casted
    public int getVoidlight() => voidlight; /// Getter for voidlight, use to check if the ability can be activated
    
    public void interruptAttack(Vector3Int defender){ /// Method to interrupt the attack and display the ability window
        attacker = PieceInteractionManager.Instance.targeter;
        this.defender = defender; // Store the defender position for later use
        if (abilityWindow != null) {
            InterruptionManager.Instance.createWindow(gameObject, abilityWindow);
        }
    }  
    
    //Ability specific properties
    public GameObject abilityWindow;
    public int dmgReduction = 1; /// Amount of damage reduction provided by the ability
    private List<Vector3Int> abilityTargets = new List<Vector3Int>();
    private void launchAttack(){ /// Method to launch the attack with the current targeter and defender, after the interruption
        if (attacker != null) {
            PieceInteractionManager.Instance.setTargeter(attacker);
            PieceInteractionManager.Instance.addTarget(defender); // Add the target position to the target list
            PieceInteractionManager.Instance.attack(defender, false); // Proceed with the attack
        }
    }

    public void acceptActivation(){ /// Method to accept the activation of the ability
        if (voidlight >= abilityCost){ //Double check to ensure enough voidlight is available
            abilityTargets = PieceMovement.detectTilesInRange(CurPos, 1, GetComponent<PieceMovement>().tileMap); // Get the tiles in range for the ability
            
            PieceInteractionManager.Instance.setTargeter(gameObject); // Set the current piece as the targeter
            abilityTargets = PieceInteractionManager.Instance.areTargeted(abilityTargets, true);  //Target only fus
            PieceInteractionManager.Instance.protect(dmgReduction); // Protect the targeted pieces with the damage reduction
            PieceInteractionManager.Instance.resetTargets(true); // Reset the targets after protection
            
            GetComponent<PieceAttack>().Protect(dmgReduction);
            launchAttack(); // Launch the attack with the current targeter and defender
            castAbility(); // Cast the ability to deduct the cost
            resetAbility(); // Reset the ability after activation 
        } else cancelActivation(); // If not enough voidlight, cancel the activation
    }
    public void cancelActivation(){ /// Method to cancel the activation of the ability
        launchAttack(); // Launch the attack with the current targeter and defender
        resetAbility();
    }
    //Inherited properties
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        if (abilityWindow != null && !abilityCasted && TurnManager.Instance.isPlayerTurn(false)) {
            InterruptionManager.Instance.createWindow(gameObject, abilityWindow);
        }
    }

    protected override void resetAbility()
    {
        attacker = null; // Reset the attacker to null
        abilityTargets.Clear(); // Clear the ability targets
        defender = Vector3Int.zero; // Reset the defender position
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
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
