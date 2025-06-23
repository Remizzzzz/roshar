using UnityEngine;
using System.Collections.Generic;


public enum PieceState {moving,basic,attacking,casting,locked,distracted}
public class PieceStateManager : MonoBehaviour
{
    /** * PieceStateManager is a singleton that manages the state of all pieces in the game.
     * It keeps track of the state of each piece, whether it is a Fluctuomancer or a Fused piece.
     * The states are stored in two dictionaries, one for Fluctuomancers and one for Fused pieces.
     * The states can be updated, retrieved, and removed.
    */
    public static PieceStateManager Instance;
    public GameObject fused;
    public GameObject fluctuomancers;
    private Dictionary<GameObject, PieceState> statesFus = new();
    private Dictionary<GameObject, PieceState> statesFluct = new();

    public void updateState(GameObject p,PieceState s, bool isFluct){
        /// Updates the state of a piece.
        if (isFluct) {
            if (statesFluct.ContainsKey(p)) statesFluct[p]=s;
        }
        else {
            if (statesFus.ContainsKey(p)) statesFus[p]=s;
        }
    }
    public PieceState getState(GameObject p, bool isFluct){
        /// Retrieves the state of a piece.
        if (isFluct) return statesFluct[p];
        return statesFus[p];
    }
    public void remove(GameObject g){ /// Removes a piece from the state manager.
        if (statesFluct.ContainsKey(g)) statesFluct.Remove(g);
        else if (statesFus.ContainsKey(g)) statesFus.Remove(g);
    }
    public bool canMove(bool isFluct){ /// Checks if a piece can move. If there is a piece already moving, it cannot move.
        if (isFluct) return !statesFluct.ContainsValue(PieceState.moving);
        return !statesFus.ContainsValue(PieceState.moving);    
    }
    public bool isAttacked(bool isFluct){ /// Checks if a piece is being attacked.
        //For now, not very useful, but for safety and modularity we keep it
        if (isFluct) return statesFus.ContainsValue(PieceState.attacking);
        return statesFluct.ContainsValue(PieceState.attacking);    
    }
    public bool isCasting(){ /// Checks if a piece is casting an ability.
        return (statesFus.ContainsValue(PieceState.casting) || statesFluct.ContainsValue(PieceState.casting));
    }
    public void addPiece(GameObject p, bool isFluct){ /// Adds a piece to the state manager
        if (isFluct){
            if (!statesFluct.ContainsKey(p)) statesFluct.Add(p,PieceState.basic);
        } else {
            if (!statesFus.ContainsKey(p)) statesFus.Add(p,PieceState.basic);
        }
    }
    private void initDicts(){
        foreach (Transform piece in fused.transform)
        {
            if (!statesFus.ContainsKey(piece.gameObject)){
                statesFus.Add(piece.gameObject,PieceState.basic);

            }
        }
        foreach (Transform piece in fluctuomancers.transform){
            if (!statesFluct.ContainsKey(piece.gameObject)){
                statesFluct.Add(piece.gameObject,PieceState.basic);
            }
        }
    }
    void Awake(){
        Instance=this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initDicts();
    }

    // Update is called once per frame
    void Update()
    {
    }
}