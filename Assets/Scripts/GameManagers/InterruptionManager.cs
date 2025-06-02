using UnityEngine;
using System.Collections.Generic;

public class InterruptionManager : MonoBehaviour
{
    public static InterruptionManager Instance;
    private bool interruption = false; // Flag to check if an interruption is currently active
    public bool isInterruptionActive() => interruption; // Property to check if an interruption is active
    private List<GameObject> interruptionWindows = new List<GameObject>(); // List to keep track of active interruption windows
    public void activateInterruption()
    {
        interruption = true; // Set the interruption as active
    }
    public void endInterruption()
    {
        interruption = false; // Set the interruption as inactive
    }
    public void createWindow(GameObject caller, GameObject WindowPreFab,string callerName ="")
    {
        // Create a new interruption window and set the caller
        GameObject Window = Instantiate(WindowPreFab, new Vector3Int(0,0,0), Quaternion.identity);
        Window.GetComponent<WindowInterruption>().setCaller(caller);
        Window.GetComponent<WindowInterruption>().setCallerName(callerName);
        interruptionWindows.Add(Window); // Add the new window to the list of active windows
        activateInterruption(); // Activate the interruption
    }
    public void destroyWindows(int indice=-1){
        if (indice == -1) {
            // If no specific index is provided, destroy all windows
            foreach (GameObject window in interruptionWindows) {
                Destroy(window);
            }
            interruptionWindows.Clear(); // Clear the list after destroying all windows
        } else if (indice >= 0 && indice < interruptionWindows.Count) {
            // If a valid index is provided, destroy the specific window
            Destroy(interruptionWindows[indice]);
            interruptionWindows.RemoveAt(indice); // Remove the destroyed window from the list
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
