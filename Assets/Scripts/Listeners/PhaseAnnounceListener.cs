using UnityEngine;
using System.Collections.Generic;
using TMPro; // or UnityEngine.UI
public class PhaseAnnounceListener : MonoBehaviour
{
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
            PhaseAnnounce.GetComponent<AnimationCreator>().Animate(); //We animate the phase change
            PhaseManager.Instance.AnimatePhaseChange = false; //We reset the flag so the animation is not played again
            if (TurnManager.Instance.newTurn) specialAnnouncePlayed = false; //If this is a new turn, we reset the special announce flag
            if (specialAnnouncePlayed) return; //If the special announce has already been played, we do not play it again

            if (TurnManager.Instance.newTurn){
                TurnManager.Instance.newTurn = false; //We reset the flag so the turn is not updated again
                if (TurnManager.Instance.isSummonable(true,true)) {
                    specialAnnounceText.text = "Special Summon"; //We set the text of the special announce if this is a special turn
                    SpecialAnnounce.GetComponent<AnimationCreator>().Animate(); //We animate the special announce
                }
                specialAnnouncePlayed = true; //We set the flag to true so we do not play the special announce again

            }
        }
    }
}