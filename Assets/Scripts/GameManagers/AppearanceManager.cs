using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using utils;

public class AppearanceManager : MonoBehaviour
{
    public static AppearanceManager Instance;
    public GameObject protectedPrefab;
    public GameObject lockedPrefab;
    public GameObject distractedPrefab;
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
        if (piece.transform.Find("protected") != null) return;
        GameObject protect = Instantiate(protectedPrefab, piece.transform);
        protect.name = "protected";
        protect.transform.localPosition = Vector3.zero;
        protect.GetComponent<SpriteRenderer>().sortingOrder = piece.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }

    public void isNotProtectedEvent(GameObject piece){
        Transform protectedTransform = piece.transform.Find("protected");
        if (protectedTransform != null) {
            Destroy(protectedTransform.gameObject);
        }
    }

    public void isLockedEvent(GameObject piece){
        if (piece.transform.Find("locked") != null) return;
        GameObject locked = Instantiate(lockedPrefab, piece.transform);
        locked.name = "locked";
        locked.transform.localPosition = Vector3.zero;
        locked.GetComponent<SpriteRenderer>().sortingOrder = piece.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
    public void isNotLockedEvent(GameObject piece){
        Transform lockedTransform = piece.transform.Find("locked");
        if (lockedTransform != null) {
            Destroy(lockedTransform.gameObject);
        }
    }
    public void isDistractedEvent(GameObject piece){
        if (piece.transform.Find("distracted") != null) return;
        GameObject distracted = Instantiate(distractedPrefab, piece.transform);
        distracted.name = "distracted";
        distracted.transform.localPosition = Vector3.zero;
        distracted.GetComponent<SpriteRenderer>().sortingOrder = piece.GetComponent<SpriteRenderer>().sortingOrder + 1;
    }
    public void isNotDistractedEvent(GameObject piece){
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
