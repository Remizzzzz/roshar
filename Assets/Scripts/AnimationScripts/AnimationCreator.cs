using UnityEngine;
using System.Collections.Generic;

public class AnimationCreator : MonoBehaviour
{
    public List<Slide> animations = new List<Slide>(); ///list of slides for now, we'll change if we add more animations later
    public bool resetPosition = true; ///if true, the position is reset after the animation
    public bool isHiding = true; ///if true, the object is hidden when it's not animated

    private Vector3 initialPosition; ///initial position of the object, used to reset the position after the animation
    private bool animationStarted = false; ///if true, the animation is started
    private int currentAnimationIndex = 0; ///index of the current animation

    public void Animate(){ /** This method starts the animation defined in the inspector*/
        if (isHiding) gameObject.SetActive(true); 
        animations[currentAnimationIndex].StartAnimation(transform); 
        animationStarted = true;
    }
    void Start()
    {
        initialPosition = transform.position; ///we store the initial position of the object
    }

    void Update(){
        /**
        Each frame, we check if the animation is started and if the current animation is running.
        If the animation is running, we update the animation.
        If the animation is not running, we go to the next animation.
        If we reached the end of the list, we stop the animation and reset the position if needed.
        If the object is set to hide, we also hide it when the animation is finished.
        */
        if (!animationStarted) return;
        if (animations[currentAnimationIndex].IsSliding){
            animations[currentAnimationIndex].UpdateAnimation(transform);
            return;
        }
        currentAnimationIndex++; 
        if (currentAnimationIndex >= animations.Count)
        {
            animationStarted = false;
            currentAnimationIndex = 0;
            if (resetPosition) transform.position = initialPosition;
            if (isHiding) gameObject.SetActive(false);
            return;
        }
        animations[currentAnimationIndex].StartAnimation(transform);
    }
}