using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameLib.System
{
    public class CoroutineTaskScheduler : MonoBehaviour
    {
        public static CoroutineTaskScheduler Instance;

        const int InitialSize = 16;

        int _coroutineId = 0;

        int _fillsize = 0;

        CoroutineTask[] _coroutines = new CoroutineTask[InitialSize];


        private void Awake()
        {
            Instance = this;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        public int StartCoroutineTask(IEnumerator enumerator)
        {
           
            if(_coroutines.Length  == _fillsize)
            {
                Array.Resize(ref _coroutines, checked(_fillsize * 2));
            }

            int returnid=0;
            var isAssign = false;
            _fillsize = 0;
            for (int i = 0; i < _coroutines.Length; i++)
            {
                
                if (_coroutines[i] == null && !isAssign)
                {
                    _coroutines[i] = new CoroutineTask(this, enumerator, _coroutineId);
                    returnid = _coroutineId;
                    _coroutineId = (_coroutineId + 1) & 0x7FFFFFFF;
                    _coroutines[i].finishCoroutineCallback = FinishCallBack;
                    _coroutines[i].StartCoroutine();
                    isAssign = true;
                }

                if (_coroutines[i] != null)
                {
                    _fillsize++;
                }
            }

            return returnid;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Kill(int id)
        {
            for(int index = 0; index < _coroutines.Length; index++)
            {
                if(_coroutines[index] != null && _coroutines[index].id == id)
                {
                    _coroutines[index].Kill();
                    break;
                }
            }
        }

        public void KillAll()
        {
            for (int index = 0; index < _coroutines.Length; index++)
            {
                if (_coroutines[index] != null)
                {
                    _coroutines[index].Kill();
                  
                }
            }
        }


        public void Pause(int id)
        {
            for (int index = 0; index < _coroutines.Length; index++)
            {
                if (_coroutines[index] != null && _coroutines[index].id == id)
                {
                    _coroutines[index].Pause();
                    break;
                }
            }
        }


        public void PauseAll()
        {
            for (int index = 0; index < _coroutines.Length; index++)
            {
                if (_coroutines[index] != null)
                {
                    _coroutines[index].Pause();
                }
            }
        }


        public void Resume(int id)
        {
            for (int index = 0; index < _coroutines.Length; index++)
            {
                if (_coroutines[index] != null && _coroutines[index].id == id)
                {
                    _coroutines[index].Resume();
                    break;
                }
            }
        }


        public void ResumeAll()
        {
            for (int index = 0; index < _coroutines.Length; index++)
            {
                if (_coroutines[index] != null)
                {
                    _coroutines[index].Resume();
                }
            }
        }


        private void FinishCallBack(int coroutineId)
        {
            for(int index = 0; index < _coroutines.Length; index++)
            {
                if(_coroutines[index] != null && _coroutines[index].id == coroutineId)
                {
                    _coroutines[index] = null;
                }
            }
        }


        private void OnDestroy()
        {
            Instance = null;
        }
    }
}
