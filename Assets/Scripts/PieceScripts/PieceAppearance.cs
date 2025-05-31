using UnityEngine;

public class PieceAppearance : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite basic;
    public Sprite locked;
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
            if (locked != null) spriteRenderer.sprite = locked;
            else Debug.LogWarning("Locked sprite is not assigned for " + gameObject.name);
        } else {
            switch (state)
            { //For other possible states
                case PieceState.basic:
                    if (basic !=null)
                        spriteRenderer.sprite = basic;
                    else 
                        Debug.LogWarning("Basic sprite is not assigned for " + gameObject.name);
                    break;
                default:
                    if (basic != null)
                        spriteRenderer.sprite = basic; // Default to basic if state is not recognized
                    else 
                        Debug.LogWarning("Basic sprite is not assigned for " + gameObject.name);
                    break;
            }
        }
    }
}
