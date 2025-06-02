using UnityEngine;
using System;
using TMPro;


public class ButtonForInterruption : MonoBehaviour
{
    public TextMeshPro buttonText; // ou public Text phaseText;
    private Action onClickAction;
    public void setOnClickAction(Action action)
    {
        onClickAction = action;
    }
    public void setText(string text)
    {
        buttonText.text = text; // Set the text of the button
    }
    public void setColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color; // Set the color of the button
    }
    void OnMouseDown()
    {
        if (onClickAction != null)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.5f, 0.5f); // Change color to indicate the button is pressed
            onClickAction.Invoke(); // Call the action when the button is clicked
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
