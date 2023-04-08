using System;
using UnityEngine;
namespace Menu
{
    public class MenuUI : MonoBehaviour
    {
        public GameObject[] panels;

        public void SwitchPanel(int index)
        {
            for (int i = 0; i < panels.Length; i++)
            {
                if (i == index)
                {
                    panels[i].transform.SetAsLastSibling();
                }
            }
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}