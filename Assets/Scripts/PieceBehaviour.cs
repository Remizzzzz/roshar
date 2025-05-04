using UnityEngine;
using UnityEngine.Tilemaps;

public class PieceBehaviour : MonoBehaviour
{
    public Tilemap tileMap;
    private Vector3Int oldPos;
    void OnMouseDown(){
        transform.localScale=new Vector3(1.3f,1.3f,1.3f);
    }
    void OnMouseDrag(){   
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0; // Assure que l'objet reste sur le plan 2D
        transform.localPosition = mousePosition;
    }
    void OnMouseUp(){
        transform.localScale = new Vector3(1,1,1);
        Vector3Int cellPos = tileMap.WorldToCell(transform.position);
        TileBase tileSelected = tileMap.GetTile(cellPos);
        if (tileMap.ContainsTile(tileSelected)){
            transform.position = tileMap.GetCellCenterWorld(cellPos);
            oldPos=cellPos;
        } else {
            transform.position = tileMap.GetCellCenterWorld(oldPos);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        Vector3Int cellPos = tileMap.WorldToCell(transform.position);
        transform.position = tileMap.GetCellCenterWorld(cellPos);
        oldPos=cellPos;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
