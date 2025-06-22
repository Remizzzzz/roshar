using UnityEngine;
using System.Collections.Generic;

public class PhaseAnnounceListener : MonoBehaviour
{
    public GameObject PhaseAnnounce;
    void Start(){
        PhaseAnnounce.GetComponent<AnimationCreator>().Animate();
    }
    void Update(){
        if (PhaseManager.Instance.AnimatePhaseChange)
        {
            PhaseAnnounce.GetComponent<AnimationCreator>().Animate(); //We animate the phase change
            PhaseManager.Instance.AnimatePhaseChange = false; //We reset the flag so the animation is not played again
        }
    }
}