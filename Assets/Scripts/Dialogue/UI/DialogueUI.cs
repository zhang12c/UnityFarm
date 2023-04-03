using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace Dialogue.UI
{
    public class DialogueUI : MonoBehaviour
    {
        public GameObject dialogueBox;
        public TextMeshProUGUI dialogueTxt;
        public Image FaceLeft, FaceRight;
        public TextMeshProUGUI FaceLeftName, FaceRightName;

        public GameObject continueBox;
        private void Awake()
        {
            continueBox.SetActive(false);
        }
    }
}