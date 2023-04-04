using System.Collections;
using System.Collections.Generic;
using Dialogue.Data;
using NPC;
using UnityEngine;
using UnityEngine.Events;
using Utility;
namespace Dialogue.Logic
{
    [RequireComponent(typeof(NPCMovement))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class DialogueController : MonoBehaviour
    {
        private NPCMovement _npcMovement => GetComponent<NPCMovement>();

        public UnityEvent OnFinishEvent;

        public List<DialoguePiece> dialogueList = new List<DialoguePiece>();

        private Queue<DialoguePiece> _dialoguePieces;

        private bool canTalk;

        private GameObject _uiSign;

        private bool _isSpeaking;
        private void Awake()
        {
            FillDialogueStack();
            _uiSign = transform.GetChild(1).gameObject;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.gameObject.CompareTag("Player"))
            {
                canTalk = !_isSpeaking && _npcMovement.interactable;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                canTalk = false;
            }
        }

        private void Update()
        {
            _uiSign.SetActive(canTalk);

            if (canTalk && Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(DialogueRoutine());
            }
        }

        private IEnumerator DialogueRoutine()
        {
            _isSpeaking = true;
            if (_dialoguePieces.TryDequeue(out DialoguePiece result))
            {
                MyEventHandler.CallShowDialogueEvent(result);
                yield return new WaitUntil((() => result.isDone = true));
                _isSpeaking = false;
            }
            else
            {
                MyEventHandler.CallShowDialogueEvent(null);
                FillDialogueStack();
                _isSpeaking = false;
                
                OnFinishEvent?.Invoke();
            }
        }

        private void FillDialogueStack()
        {
            _dialoguePieces = new Queue<DialoguePiece>();
            for (int i = 0; i < dialogueList.Count; i++)
            {
                dialogueList[i].isDone = false;
                _dialoguePieces.Enqueue(dialogueList[i]);
            }
        }
    }
}