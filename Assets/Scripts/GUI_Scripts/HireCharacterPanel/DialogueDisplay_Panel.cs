using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueDisplay_Panel : Panel_Base //, IDeallocatable
{
    [SerializeField] private TextMeshProUGUI dialogueDisplayText;

    private ConcurrentQueue<string> dialoguePhrases;

    private TextAnimate animatedText;

    public IEnumerator[] CO { get => _co; }
    private IEnumerator[] _co;

    private WaitWhile waitWhileTextAnimating;
    private WaitWhile waitWhileTextIsPaused;


    private void Awake()
    {
        animatedText = dialogueDisplayText.GetComponent<TextAnimate>();
        waitWhileTextAnimating = new WaitWhile(() => animatedText.CO is not null);
        waitWhileTextIsPaused = new WaitWhile(() => _co[1] is not null);
        _co = new IEnumerator[2]; // 

    }



    public void LoadPanel(params string[] dialoguePhrases_IN)
    {
        animatedText.SetVisibility(isVisible: false);
        dialoguePhrases = new ConcurrentQueue<string>(dialoguePhrases_IN);
    }


    public void DisplayContents()
    {
        for (int i = 0; i < _co.Length; i++)
        {
            if (_co[i] is not null)
            {
                StopCoroutine(_co[i]);
                _co[i] = null;
            }
        }
        _co[0] = DisplayContentsRoutine();
        StartCoroutine(_co[0]);
    }

    private IEnumerator DisplayContentsRoutine()//bool awaitAtBeginning = false)
    {
        animatedText.SetVisibility(isVisible: true);
        while (dialoguePhrases.TryDequeue(out string phrase))
        {
            dialogueDisplayText.text = phrase;
            //animatedText.UpSize(lerpSpeedModifier: 5f);
            animatedText.Reveal(lerpSpeedModifier: 5f);
            yield return waitWhileTextAnimating;

            if (dialoguePhrases.Count > 0)
            {
                _co[1] = CancellableWaitForSeconds();
                StartCoroutine(_co[1]);
                yield return waitWhileTextIsPaused;
            }
        }

        _co[0] = null;
    }

    private IEnumerator CancellableWaitForSeconds()
    {
        yield return TimeTickSystem.WaitForSeconds_One;
        _co[1] = null;
    }

    public void FastForwardDisplayAnimation()
    {
        if (_co[0] is not null)
        {
            if (animatedText.CO is not null)
            {
                animatedText.InstantFinaliseAndResolveVisibility(isVisible: true);
                Debug.Log("only complete to end");
            }
            else if (_co[1] is not null)
            {
                Debug.Log("only skip pause ");
                StopCoroutine(_co[1]);
                _co[1] = null;
            }
        }
    }
}
