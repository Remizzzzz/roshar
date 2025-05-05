using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro; // ou UnityEngine.UI si tu utilises le Text classique

public class TileClickUI : MonoBehaviour
{
    public Tilemap tilemap;
    public Camera cam;
    public TextMeshProUGUI coordText; // ou public Text coordText;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // clic gauche
        {
            Vector3 mouseWorldPos = cam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0;

            Vector3Int cellPos = tilemap.WorldToCell(mouseWorldPos);
            TileBase tile = tilemap.GetTile(cellPos);

            if (tile != null)
            {
                coordText.text = "Coordonnées : " + cellPos.ToString() + "\nState : "+TileStateManager.Instance.getState(cellPos).ToString();
            }
            else
            {
                coordText.text = "Pas de tile ici";
            }
        }
    }
}
