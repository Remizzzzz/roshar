using UnityEngine;

public class PieceAppearance : MonoBehaviour
{
    /** PieceAppearance is a MonoBehaviour that handles the appearance of the piece.
     * It listens for changes in the piece's state and updates its appearance accordingly, by asking the AppearanceManager.
     */
    private SpriteRenderer spriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
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
