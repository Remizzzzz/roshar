using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using utils;

public class AppearanceManager : MonoBehaviour
{
    /** This class manages the appearance of pieces in the game.
     It handles visual effects such as protection, locking, and distraction.
    */
    public static AppearanceManager Instance;
    public GameObject protectedPrefab; /// Prefab for the protected visual effect
    public GameObject lockedPrefab; /// Prefab for the locked visual effect
    public GameObject distractedPrefab; /// Prefab for the distracted visual effect

    /** Dictionaries to hold references to fluctuating pieces and fusion pieces.
     These dictionaries are used to manage the visual effects associated with these pieces.
     They're linked to PieceInteractionManager to get access to the position of the pieces.
    */
    private Dictionary<Vector3Int,GameObject> fluct=new();
    private Dictionary<Vector3Int,GameObject> fus=new();
    internal Tilemap tileMap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance=this;
        tileMap = TileStateManager.Instance.tileMap;
        fluct = PieceInteractionManager.Instance.getFluctDictionnary();
        fus = PieceInteractionManager.Instance.getFusDictionnary();
    }

    public void isProtectedEvent(GameObject piece){
        /** This method is called when a piece is protected.
         It checks if the "protected" visual effect already exists, and if not, it creates one.
         The effect is instantiated as a child of the piece's transform and positioned at the piece's position.
        */
        if (piece.transform.Find("protected") != null) return;
        GameObject protect = Instantiate(protectedPrefab, piece.transform);
        protect.name = "protected";
        protect.transform.localPosition = Vector3.zero;
        protect.GetComponent<SpriteRenderer>().sortingOrder = piece.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    public void isNotProtectedEvent(GameObject piece){
        /** This method is called when a piece is no longer protected.
         It checks if the "protected" visual effect exists, and if so, it destroys it.
         This effectively removes the visual effect from the piece.
        */
        Transform protectedTransform = piece.transform.Find("protected");
        if (protectedTransform != null) {
            Destroy(protectedTransform.gameObject);
        }
    }

    public void isLockedEvent(GameObject piece){
        /** This method is called when a piece is locked.
         It checks if the "locked" visual effect already exists, and if not, it creates one.
         The effect is instantiated as a child of the piece's transform and positioned at the piece's position.
        */
        if (piece.transform.Find("locked") != null) return;
        GameObject locked = Instantiate(lockedPrefab, piece.transform);
        locked.name = "locked";
        locked.transform.localPosition = Vector3.zero;
        locked.GetComponent<SpriteRenderer>().sortingOrder = piece.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
    public void isNotLockedEvent(GameObject piece){
        /** This method is called when a piece is no longer locked.
         It checks if the "locked" visual effect exists, and if so, it destroys it.
         This effectively removes the visual effect from the piece.
        */
        Transform lockedTransform = piece.transform.Find("locked");
        if (lockedTransform != null) {
            Destroy(lockedTransform.gameObject);
        }
    }
    public void isDistractedEvent(GameObject piece){
        /** This method is called when a piece is distracted.
         It checks if the "distracted" visual effect already exists, and if not, it creates one.
         The effect is instantiated as a child of the piece's transform and positioned at the piece's position.
        */
        if (piece.transform.Find("distracted") != null) return;
        GameObject distracted = Instantiate(distractedPrefab, piece.transform);
        distracted.name = "distracted";
        distracted.transform.localPosition = Vector3.zero;
        distracted.GetComponent<SpriteRenderer>().sortingOrder = piece.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
    public void isNotDistractedEvent(GameObject piece){
        /** This method is called when a piece is no longer distracted.
         It checks if the "distracted" visual effect exists, and if so, it destroys it.
         This effectively removes the visual effect from the piece.
        */
        Transform distractedTransform = piece.transform.Find("distracted");
        if (distractedTransform != null) {
            Destroy(distractedTransform.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
