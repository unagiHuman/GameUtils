﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UniRx;
using UniRx.Triggers;


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


        public Action<MonoBehaviour, int> finishCoroutineCallback;


        public int id
        {
            get
            {
                return _coroutineId;
            }
        }


        State _state = State.NONE;

        IEnumerator _coroutine;

        IEnumerator _nested;

        MonoBehaviour _mb;


        int _coroutineId = 0;


        public CoroutineTask(MonoBehaviour mb, IEnumerator task, int id)
        {
            this._state = State.NONE;
            this._coroutine = task;
            this._mb = mb;
            this._nested = Wrapper();
            this._coroutineId = id;


            mb.OnDestroyAsObservable().Subscribe(prop =>
            {
                _state = State.TERMINATE;
                if (finishCoroutineCallback != null)
                {
                    finishCoroutineCallback(this._mb, this._coroutineId);
                }
            });
        }


        public void StartCoroutine()
        {
            _state = State.RUN;
            _mb.StartCoroutine(_nested);
            _mb.StartCoroutine(Wait());
        }


        public void Pause()
        {
            if (_state == State.RUN) _state = State.PAUSE;
        }


        public void Resume()
        {
            if (_state == State.PAUSE) _state = State.RUN;
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

            if (_state == State.RUN)
            {
                if (task.MoveNext())
                {
                    var child = task.Current;

                    if (child == null)
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
                                if (ie.Current is bool)
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

                            yield return child;
                        }
                        else
                        {
                            yield return child;
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
            if (finishCoroutineCallback != null)
            {
                finishCoroutineCallback(this._mb, this._coroutineId);
            }
        }

    }
}
