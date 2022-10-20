using System.Collections;
using UnityEngine;

namespace Utilities
{
    public static class UnityEngineExtension
    {
        public delegate void CallBack();

        public static void InvokeNextFrame(this MonoBehaviour _mb, CallBack _callBack)
        {
            _mb.StartCoroutine(ProcessNextFrame(_callBack));
        }

        public static void InvokeAfterTime(this MonoBehaviour _mb, CallBack _callBack, float time)
        {
            _mb.StartCoroutine(WaitToProcess(_callBack, time));
        }

        private static IEnumerator ProcessNextFrame(CallBack callBack)
        {
            yield return null;
            callBack();
        }

        private static IEnumerator WaitToProcess(CallBack callBack, float time)
        {
            yield return new WaitForSeconds(time);
            callBack();
        }
    }
}
