using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class Ability : MonoBehaviour
{
    //Variables and method to use
    protected static int stormlight = 6; //Fulgiflamme, mana for ability of fluct
    protected static int voidlight = 6; // Néantiflamme, mana for ability of fus
    public Vector3Int CurPos => GetComponent<PieceMovement>().getCurPos();
    protected bool isAbilityActive = false; // Flag to check if the ability is currently active
    protected bool abilityCasted = false; // Flag to check if the ability was casted
    private int turnOfCast = -1; // Turn when the ability was casted, used for cooldown or restoration logic
    protected void castAbility(){
        if (gameObject.GetComponent<PieceMovement>().isFluct)
        {
            stormlight -= abilityCost; // Deduct the stormlight cost
            Debug.Log("Stormlight used: " + abilityCost + ", Remaining Stormlight: " + stormlight);
        }
        else
        {
            voidlight -= abilityCost; // Deduct the voidlight cost
            Debug.Log("Voidlight used: " + abilityCost + ", Remaining Voidlight: " + voidlight);
        }
        turnOfCast = TurnManager.Instance.getTurnNumber(); // Update the turn of cast
        abilityCasted=true; // Set the ability as casted
    }

    //Abstract variables and method to redefine

    [Tooltip("Cost of the ability, set per ability Instance")]
    public abstract int abilityCost {get;} // Cost of the ability, to be defined in derived classes

    protected virtual void ActivateAbility()
    {
        // This method should be overridden in derived classes to implement the specific ability logic
        Debug.Log("Ability not implemented in base class. Please override this method in derived classes.");
        abilityCasted=true;
        return; // Return true to indicate the ability was successfully activated
    }

    protected virtual void resetAbility()
    {
        // This method should be overridden in derived classes to implement the logic for resetting the ability
        PieceStateManager.Instance.updateState(gameObject,PieceState.basic,gameObject.GetComponent<PieceMovement>().isFluct);
        Debug.Log("Reset ability not implemented in base class. Please override this method in derived classes.");
        isAbilityActive = false; // Reset the ability active state
    }

    //Private method and variables
    private bool canRestore = true; // Flag to check if restoration is allowed (to limit restoration to one time every 8 turns)
    protected void restore(){
        if (TurnManager.Instance.getTurnNumber()%8 == 0 && canRestore) // Every 8 turns, restore stormlight and voidlight
        {
            stormlight = 6; // Restore stormlight to full
            voidlight = 6; // Restore voidlight to full
            canRestore = false; // Prevent further restoration until next turn
        } else if (TurnManager.Instance.getTurnNumber()%8 != 0) {
            canRestore = true; // Allow restoration again in the next turn
        }
    }
    protected void restoreAbility(){
        if (abilityCasted){
            if (TurnManager.Instance.getTurnNumber() > turnOfCast) // Check if the ability was casted more than one turn ago
            {
                abilityCasted = false;
                turnOfCast = TurnManager.Instance.getTurnNumber(); // Update the turn of cast to the current turn
            }
        }
    }

    protected virtual void startAbility(){
        PieceStateManager.Instance.updateState(gameObject,PieceState.casting,gameObject.GetComponent<PieceMovement>().isFluct);
        isAbilityActive = true; // Set the ability as active
        bool isFluct = gameObject.GetComponent<PieceMovement>().isFluct;
        Debug.Log("Stormlight: " + stormlight + ", Voidlight: " + voidlight + ", Ability Cost: " + abilityCost + ", Is Fluct: " + isFluct);
        if (isFluct && stormlight >= abilityCost) // Check if the piece is Fluct and has enough stormlight
        {
            ActivateAbility(); // Attempt to activate the ability
        }
        else if (!isFluct && voidlight >= abilityCost) // Check if the piece is Fus and has enough voidlight
        {
            ActivateAbility(); // Attempt to activate the ability
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        restore();
        restoreAbility();
        if (Input.GetMouseButtonDown(1) && !InterruptionManager.Instance.isInterruptionActive()) // 1 = right click
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get Mouse Position
            mousePosition.z = 0; 
            Vector3Int cellPos = GetComponent<PieceMovement>().tileMap.WorldToCell(mousePosition);

            if (cellPos==CurPos && !PieceStateManager.Instance.isCasting())
            {
                if (!abilityCasted) startAbility(); //Start ability routine
            } else {
                if (isAbilityActive) // If the ability is active, reset it
                {
                    resetAbility();
                }
            }
        }
    }
}
