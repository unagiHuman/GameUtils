using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;


namespace GameLib.System
{

    public class CoroutineTask 
    {

        enum State
        {
            NONE,
            RUN,
            TERMINATE,
            PAUSE,
        }


        State _state = State.NONE;

        IEnumerator _coroutine;

        IEnumerator _nested;

        MonoBehaviour _mb;


        public CoroutineTask(MonoBehaviour mb, IEnumerator task)
        {
            _state = State.NONE;
            this._coroutine = task;
            this._mb = mb;
            this._nested = Wrapper();
        }


        public Coroutine StartCoroutine()
        {
            _state = State.RUN;
            _mb.StartCoroutine(_nested);
            return _mb.StartCoroutine(Wait());
        }


        public void Pause()
        {
            _state = State.PAUSE;
        }


        public void Resume()
        {
            _state = State.RUN;
        }


        public void Kill()
        {
            _state = State.TERMINATE;
        }


        private IEnumerator Wrapper()
        {
            while (true)
            {
                var ie = MoveNext(_coroutine);
                yield return ie;
                if (ie.Current is bool)
                {
                    if (!(bool)ie.Current)
                    {
                        break;
                    }
                }
            }
            _state = State.TERMINATE;
        }


        private IEnumerator MoveNext(IEnumerator task)
        {

            if(_state == State.RUN)
            {
                if (task.MoveNext())
                {
                    var child = task.Current;
                    

                    if(child == null)
                    {
                        yield return null;
                    }
                    else
                    {
                        var type = child.GetType();
                        if (child is IEnumerator)
                        {
                            while (true)
                            {

                                var ie = MoveNext((IEnumerator)child);
                                yield return ie;
                                if(ie.Current is bool)
                                {
                                    if (!(bool)ie.Current)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        else if (type == typeof(Coroutine))
                        {
                            Debug.LogWarning("Not Support Nest StartCoroutine");
                        }
                        else
                        {
                            if (_state == State.RUN)
                            {
                                yield return child;
                            }
                            else if (_state == State.PAUSE)
                            {
                                yield return null;
                            }
                            else
                            {
                                yield return false;
                            }
                        }
                    }
                   
                }
                else
                {
                    yield return false;
                }
            }
            else if (_state == State.PAUSE)
            {
                yield return null;
            }
            else
            {
                yield return false;
            }
          
        }


        private IEnumerator Wait()
        {
            while (_state != State.TERMINATE)
            {
                yield return null;
            }
        }


      
    }


    public static class MonoBehaviourCoroutineExtension
    {
        public static CoroutineTask StartCoroutineTask(this MonoBehaviour mb, IEnumerator task)
        {
            return new CoroutineTask(mb, task);
        }
    }
}
