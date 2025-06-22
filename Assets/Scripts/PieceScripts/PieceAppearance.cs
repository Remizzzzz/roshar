using UnityEngine;

public class PieceAppearance : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        PieceState state = PieceStateManager.Instance.getState(gameObject, gameObject.GetComponent<PieceMovement>().isFluct);
        if (gameObject.GetComponent<PieceMovement>().IsLocked()){
            AppearanceManager.Instance.isLockedEvent(gameObject);
        } else {
            AppearanceManager.Instance.isNotLockedEvent(gameObject);
        }
        
        if (gameObject.GetComponent<PieceAttack>().IsDistracted()) {
            AppearanceManager.Instance.isDistractedEvent(gameObject);
        } else {
            AppearanceManager.Instance.isNotDistractedEvent(gameObject);
        }

        if (gameObject.GetComponent<PieceAttack>().dmgReduc>0){
            AppearanceManager.Instance.isProtectedEvent(gameObject);
        } else {
            AppearanceManager.Instance.isNotProtectedEvent(gameObject);
        }
    }
}
