using UnityEngine;

public class Replace : MonoBehaviour
{
    public Grid grid;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3Int cellPos = grid.WorldToCell(transform.position);
        transform.position = grid.GetCellCenterWorld(cellPos);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
