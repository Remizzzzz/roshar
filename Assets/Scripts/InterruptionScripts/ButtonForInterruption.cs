using UnityEngine;
using System;
using TMPro;


public class ButtonForInterruption : MonoBehaviour
{
    /** This class is used to create a button for interruptions windows
     * It is a simple button that can be clicked to perform an action.
     * The button has a text that can be set, and a color that can be set.
     * The button is scaled to fit the window.
     * The button is a child of the interruption window.
     * The button is used to perform an action (that can be set too) when clicked.
    */
    public TextMeshPro buttonText; // ou public Text phaseText;
    private Action onClickAction; /// Action to be performed when the button is clicked
    public void setOnClickAction(Action action) /// Sets the action to be performed when the button is clicked
    {
        onClickAction = action;
    }
    public void setText(string text)
    {
        buttonText.text = text; /// Set the text of the button
    }
    public void setColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color; /// Set the color of the button
    }
    void OnMouseDown()
    {
        if (onClickAction != null)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f); /// Change color to indicate the button is pressed
            onClickAction.Invoke(); /// Call the action when the button is clicked
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonText.GetComponent<TextMeshPro>().sortingLayerID = GetComponent<SpriteRenderer>().sortingLayerID;
        buttonText.GetComponent<TextMeshPro>().sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
        transform.localScale = new Vector3(0.2f, 0.2f, 1); // Scale the button to fit the window
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
