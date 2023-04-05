using UnityEngine;
using Utility;
public class AnimationEvent : MonoBehaviour
{
    public void FootStepSound()
    {
        MyEventHandler.CallPlaySoundEvent(SoundName.FootStepSoft);
    }
}
