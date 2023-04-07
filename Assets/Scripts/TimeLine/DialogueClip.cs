using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
namespace TimeLine
{
    public class DialogueClip : PlayableAsset,ITimelineClipAsset
    {
        public DialogueBehaviour dailogue = new DialogueBehaviour();

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<DialogueBehaviour>.Create(graph, dailogue);
            return playable;
        }
        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.None;
            }
        }
    }
}