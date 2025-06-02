using UnityEngine;
using System.Collections.Generic;
using utils;

public class DeepestAbility : Ability
{
    //Ability specific properties
    public GameObject abilityWindow;
    private GameObject attacker;

    public bool getAbilityCasted() => abilityCasted; // Getter for abilityCasted, used to check if the ability was casted
    public int getVoidlight() => voidlight; // Getter for voidlight, used to check if the ability can be activated
    public void acceptActivation(){
        if (voidlight >= abilityCost){ //Double check to ensure enough voidlight is available
            ActivateAbility();
        } else cancelActivation(); // If not enough voidlight, cancel the activation
    }
    public void cancelActivation(){
        //In case PieceInteractionManager reset its properties
        PieceInteractionManager.Instance.setTargeter(attacker);
        PieceInteractionManager.Instance.addTarget(CurPos); // Re-add the current position to the target list

        PieceInteractionManager.Instance.attack(CurPos, false); // Proceed with the attack
        resetAbility();
    }
    public void interruptAttack(){
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
        GetComponent<PieceAttack>().setImmune();
        PieceInteractionManager.Instance.attack(CurPos, false); // Call the attack method with false to indicate it's a fus piece
        castAbility(); // Cast the ability to deduct the cost
        resetAbility(); // Reset the ability after activation
    }

    protected override void resetAbility()
    {
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
