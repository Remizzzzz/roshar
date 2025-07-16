using UnityEngine;

public class PieceGenerator : MonoBehaviour
{
    private Vector3 generationCoordinate;
    public void generatePiece()/// Generates a new copy of the piece at the specified generation coordinate.
    {
        GameObject piece = Instantiate(gameObject, generationCoordinate, Quaternion.identity);
        piece.GetComponent<PieceMovement>().tileMap = gameObject.GetComponent<PieceMovement>().tileMap; // Set the tilemap for the piece
        piece.GetComponent<PieceAttributes>().pieceName = GetComponent<PieceAttributes>().pieceName; // Set the name of the piece
        //register the piece in the gameManagers
        PieceInteractionManager.Instance.addPiece(piece, piece.GetComponent<PieceMovement>().getCurPos(), piece.GetComponent<PieceMovement>().isFluct); // Update the interaction manager with the new piece
        PieceStateManager.Instance.addPiece(piece, piece.GetComponent<PieceMovement>().isFluct); // Add the piece to the piece state manager
        
        piece.GetComponent<PieceMovement>().setOnMap(false); // Set the piece as not on the map

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        generationCoordinate = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
