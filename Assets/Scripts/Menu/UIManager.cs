using System;
using System.Collections;
using SaveLoad.Logic;
using UnityEngine;
using UnityEngine.UI;
using Utility;
namespace Menu
{
    public class UIManager : MonoBehaviour
    {
        private GameObject _menuCanvas;
        public GameObject menuPrefab;
        public Button settingBtn;
        public GameObject paushPanel;

        private void OnEnable()
        {
            MyEventHandler.AfterSceneLoadEvent += OnAfterSceneLoadEvent;
        }

        private void OnDisable()
        {
            MyEventHandler.AfterSceneLoadEvent -= OnAfterSceneLoadEvent;

        }

        private void Start()
        {
            _menuCanvas = GameObject.FindWithTag("MenuCanvas");
            Instantiate(menuPrefab, _menuCanvas.transform);

            settingBtn.onClick.AddListener(TogglePausePanel);
        }
        private void OnAfterSceneLoadEvent()
        {
            if (_menuCanvas.transform.childCount > 0)
            {
                Destroy(_menuCanvas.transform.GetChild(0).gameObject);
            }
        }

        public void TogglePausePanel()
        {
            bool isOpen = paushPanel.activeInHierarchy;
            
            if(isOpen)
            {
                paushPanel.SetActive(false);
                UnityEngine.Time.timeScale = 1;
            }
            else
            {
                System.GC.Collect();
                paushPanel.SetActive(true);
                UnityEngine.Time.timeScale = 0;
            }
        }

        public void ReturnMenuCanvas()
        {
            UnityEngine.Time.timeScale = 1;
            StartCoroutine(BackToMenu());
        }

        private IEnumerator BackToMenu()
        {
            paushPanel.SetActive(false);
            MyEventHandler.CallEndGameEvent();
            yield return new WaitForSeconds(1f);
            Instantiate(menuPrefab, _menuCanvas.transform);
        }
        
        public void ExitGame()
        {
            Application.Quit();
        }
    }
}