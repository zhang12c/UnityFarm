using System;
using System.Collections;
using DG.Tweening;
using Dialogue.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utility;
namespace Dialogue.UI
{
    public class DialogueUI : MonoBehaviour
    {
        public GameObject dialogueBox;
        public Text dialogueTxt;
        public Image FaceLeft, FaceRight;
        public TextMeshProUGUI FaceLeftName, FaceRightName;

        public GameObject continueBox;
        private void Awake()
        {
            continueBox.SetActive(false);
        }

        private void OnEnable()
        {
            MyEventHandler.ShowDialogueEvent += OnShowDialogueEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.ShowDialogueEvent -= OnShowDialogueEvent;
        }
        private void OnShowDialogueEvent(DialoguePiece obj)
        {
            StartCoroutine(ShowDialogue(obj));
        }
        private IEnumerator ShowDialogue(DialoguePiece piece)
        {
            if (piece != null)
            {
                piece.isDone = false;
                dialogueBox.SetActive(true);
                continueBox.SetActive(false);

                dialogueTxt.text = piece.dialogueText;

                if (piece.name != String.Empty)
                {
                    if (piece.onLeft)
                    {
                        FaceRight.gameObject.SetActive(false);
                        FaceLeft.gameObject.SetActive(true);
                        FaceLeft.sprite = piece.faceImage;
                        FaceLeftName.text = piece.name;
                    }
                    else
                    {
                        FaceLeft.gameObject.SetActive(false);
                        FaceRight.gameObject.SetActive(true);
                        FaceRight.sprite = piece.faceImage;
                        FaceRightName.text = piece.name;
                    }
                }
                else
                {
                    FaceLeft.gameObject.SetActive(false);
                    FaceRight.gameObject.SetActive(false);
                    FaceLeftName.gameObject.SetActive(false);
                    FaceRightName.gameObject.SetActive(false);
                }

                yield return dialogueTxt.DOText(piece.dialogueText, 1f).WaitForCompletion();
                piece.isDone = true;

                if (piece.hasToPause && piece.isDone)
                {
                    continueBox.SetActive(true);
                }
                else
                {
                    continueBox.SetActive(false);
                }
                yield return new WaitForSeconds(2f);
                continueBox.SetActive(false);
            }
            else
            {
                dialogueBox.SetActive(false);
                yield break;
            }
        }
        
        
    }
}