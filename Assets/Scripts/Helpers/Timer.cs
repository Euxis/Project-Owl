using System;
using System.Collections;
using UnityEngine;

namespace Helpers
{
    
    public class Timer : MonoBehaviour
    {
        /// <summary>
        /// Simple timer that executes an action after a given amount of time.
        /// </summary>
        /// <param name="time">The amount of time to wait.</param>
        /// <param name="callback">The action to execute at end of time.</param>
        public void DoTimer(float time, Action callback)
        {
            StartCoroutine(ActionTimer(time, callback));
        }

        private IEnumerator ActionTimer(float time, Action callback)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
    }
}