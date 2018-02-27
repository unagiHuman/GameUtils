using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLib.System;

public class CustomCoroutineTest : MonoBehaviour {


    IEnumerator TestCorutine()
    {
        while (true)
        {
           
            yield return Nest1Coroutine();
            Debug.Log("DoCoroutone");
        }
    }

    IEnumerator Nest1Coroutine()
    {
        var time = 0f;
        while (true)
        {
            if (time > 2f)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Do Nest1 Coroutone " + time.ToString());
            time += 0.5f;
        }
        yield return Nest2Coroutine();
    }

    IEnumerator Nest2Coroutine()
    {
        var time = 0f;
        while (true)
        {
            if(time > 2f)
            {
                break;
            }
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Do Nest2 Coroutone " + time.ToString());
            time += 0.5f;
        }

        yield return null;
       
    }

    /*
    [Button("DoCoroutine", "DoCoroutine", null)]
    [SerializeField]
    bool DoCoroutineButton;


    CoroutineTask _task;

    private void DoCoroutine()
    {
        _task = this.StartCoroutineTask(TestCorutine());
        _task.StartCoroutine();
    }


    [Button("PauseCoroutine", "PauseCoroutine", null)]
    [SerializeField]
    bool PauseCoroutineButton;

    private void PauseCoroutine()
    {
        _task.Pause();
    }


    [Button("ResumeCoroutine", "ResumeCoroutine", null)]
    [SerializeField]
    bool ResumeCoroutineButton;

    private void ResumeCoroutine()
    {
        _task.Resume();
    }

    [Button("KillCoroutine", "KillCoroutine", null)]
    [SerializeField]
    bool KillCoroutineButton;

    private void KillCoroutine()
    {
        _task.Kill();
    }
    */
}
