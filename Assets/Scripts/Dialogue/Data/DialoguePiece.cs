using UnityEngine;
namespace Dialogue.Data
{
    [System.Serializable]
    public class DialoguePiece
    {
        [Header("对话详情")]
        public Sprite faceImage;
        public string name;
        public bool onLeft;
        [TextArea]
        public string dialogueText;
        public bool hasToPause;
        public bool isDone;
    }
}