using UnityEngine;
using System.Collections.Generic;
using TMPro; /// or UnityEngine.UI
public class PhaseAnnounceListener : MonoBehaviour
{
    /**
     * This class listens for phase changes and updates the UI accordingly.
     * It plays an animation when the phase changes and shows a special announce if it's a special summoning turn.
     * 
     */
    public GameObject PhaseAnnounce;
    public GameObject SpecialAnnounce;
    public TMP_Text specialAnnounceText;
    private bool specialAnnouncePlayed = false;
    void Start(){
        PhaseAnnounce.GetComponent<AnimationCreator>().Animate();
    }
    void Update(){
        if (PhaseManager.Instance.AnimatePhaseChange)
        {
            /** If AnimatePhaseChange flag is true
             * We check if this is a new turn and if a special announce has already been played.
             * If it is a new turn, we set the text of the special announce and animate it.
             * We also set the flag to true so we do not play the special announce again.
             */
            PhaseAnnounce.GetComponent<AnimationCreator>().Animate(); //We animate the phase change
            PhaseManager.Instance.AnimatePhaseChange = false; //We reset the flag so the animation is not played again
            if (TurnManager.Instance.newTurn) specialAnnouncePlayed = false; //If this is a new turn, we reset the special announce flag
            if (specialAnnouncePlayed) return; //If the special announce has already been played, we do not play it again

            if (TurnManager.Instance.newTurn){
                TurnManager.Instance.newTurn = false; //We reset the flag so the turn is not updated again
                if (TurnManager.Instance.isSummonable(true,true)) {
                    specialAnnounceText.text = "Special Summon"; //We set the text of the special announce if this is a special turn
                    SpecialAnnounce.GetComponent<AnimationCreator>().Animate();  //We animate the special announce
                }
                specialAnnouncePlayed = true; //We set the flag to true so we do not play the special announce again

            }
        }
    }
}