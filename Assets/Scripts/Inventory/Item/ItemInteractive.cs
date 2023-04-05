using System;
using System.Collections;
using UnityEngine;
using Utility;
namespace Inventory.Item
{
    /// <summary>
    /// 道具摇晃起来
    /// </summary>
    public class ItemInteractive : MonoBehaviour
    {
        private bool _isAnimating;
        private WaitForSeconds pause = new WaitForSeconds(0.04f);

        private void OnTriggerEnter2D(Collider2D col)
        {
            if(!_isAnimating)
                if (col.transform.position.x < transform.position.x)
                {
                    // 人物在左边
                    StartCoroutine(RotateRight());
                }
                else
                {
                    StartCoroutine(RotateLeft());
                }
            
            MyEventHandler.CallPlaySoundEvent(SoundName.Rustle);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(!_isAnimating)
                if (other.transform.position.x > transform.position.x)
                {
                    // 人物在左边
                    StartCoroutine(RotateRight());
                }
                else
                {
                    StartCoroutine(RotateLeft());
                }
            
            MyEventHandler.CallPlaySoundEvent(SoundName.Rustle);
        }

        private IEnumerator RotateLeft()
        {
            _isAnimating = true;
            for (int i = 0; i < 4; i++)
            {
                transform.GetChild(0).Rotate(0, 0, 2);
                yield return pause;
            }
            for (int i = 0; i < 5; i++)
            {
                transform.GetChild(0).Rotate(0,0,-2);
                yield return pause;
            }
            transform.GetChild(0).Rotate(0, 0, 2);
            yield return pause;

            _isAnimating = false;
        }
        private IEnumerator RotateRight()
        {
            _isAnimating = true;
            for (int i = 0; i < 4; i++)
            {
                transform.GetChild(0).Rotate(0, 0, -2);
                yield return pause;
            }
            for (int i = 0; i < 5; i++)
            {
                transform.GetChild(0).Rotate(0,0,2);
                yield return pause;
            }
            transform.GetChild(0).Rotate(0, 0, -2);
            yield return pause;

            _isAnimating = false;
        }
    }
}