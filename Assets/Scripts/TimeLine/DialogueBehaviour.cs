using Dialogue.Data;
using UnityEngine;
using UnityEngine.Playables;
using Utility;
namespace TimeLine
{
    [System.Serializable]
    public class DialogueBehaviour : PlayableBehaviour
    {
        private PlayableDirector _director; 

        public DialoguePiece dialoguePiece;

        public override void OnPlayableCreate(Playable playable)
        {
            _director = (playable.GetGraph().GetResolver() as PlayableDirector);
        }

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            // 当这个片段开始播放的时候
            // 传递一下对话的内容
            MyEventHandler.CallShowDialogueEvent(dialoguePiece);
            if (Application.isPlaying)
            {
                if (dialoguePiece.hasToPause)
                {
                    // 暂停timeLine
                    TimeLineManager.Instance.PauseTimeLime(_director);
                    // 等待按空格
                }
                else
                {
                    // 如果没有下一条了
                    // 就直接关闭
                    MyEventHandler.CallShowDialogueEvent(null);
                }
            }
        }

        /// <summary>
        /// 每一帧去检查 / 执行
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        /// <param name="playerData"></param>
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (Application.isPlaying)
            {
                TimeLineManager.Instance.IsDone = dialoguePiece.isDone;
            }
        }

        /// <summary>
        /// 播放停止的时候就关闭对话框
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            MyEventHandler.CallShowDialogueEvent(null);
        }

        /// <summary>
        /// 在动画开始的时候将游戏时间暂停
        /// </summary>
        /// <param name="playable"></param>
        public override void OnGraphStart(Playable playable)
        {
            MyEventHandler.CallUpdateGameStateEvent(GameState.Pause);
        }

        /// <summary>
        /// 在。。。。。结束后运行
        /// </summary>
        /// <param name="playable"></param>
        public override void OnGraphStop(Playable playable)
        {
            MyEventHandler.CallUpdateGameStateEvent(GameState.Play);
        }
    }
}