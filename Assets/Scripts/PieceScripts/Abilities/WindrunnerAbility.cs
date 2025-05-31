using UnityEngine;

public class WindrunnerAbility : Ability
{
    [SerializeField] private int _abilityCost = 1;
    public override int abilityCost => _abilityCost;

    protected override void ActivateAbility()
    {
        // TODO: Implement the ability logic
        Debug.Log("WindrunnerAbility activated!");
        abilityCasted = true;
    }

    protected override void resetAbility()
    {
        // TODO: Implement the reset logic
        isAbilityActive = false;
        abilityCasted = true;
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start(){
    }

    // Update is called once per frame
    protected override void Update(){
        base.Update();
    }
}
