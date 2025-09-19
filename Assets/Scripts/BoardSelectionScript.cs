using UnityEngine;
using System.Collections.Generic;

public class BoardSelectionScript : MonoBehaviour
{
    public List<GameObject> toHide = new List<GameObject>(); // List of game objects to hide when the board selection is on
    public bool highBoard = false;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        
        // Rendre le sprite invisible au démarrage
        Color c = sr.color;
        c.a = 0f;
        sr.color = c;
    }

    void OnMouseOver()
    {
        // Afficher le sprite quand la souris est dessus
        Color c = sr.color;
        c.a = 1f;
        sr.color = c;
    }

    void OnMouseExit()
    {
        // Cacher le sprite quand la souris sort
        Color c = sr.color;
        c.a = 0f;
        sr.color = c;
    }

    void OnMouseDown()
    {


    }

    void Update()
    {
    }
}
