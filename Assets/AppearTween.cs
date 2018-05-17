using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearTween : MonoBehaviour {

    public static AppearTween instance;

    Tweener initialTweener;

	// Use this for initialization
	void Start () {
        initialTweener = this.transform.DOMoveY(-2, 2f).From().
            OnComplete(AnimationCompleted).
            OnRewind(AnimationRewinded).
            SetAutoKill(false);
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void HideObjects()
    {
        initialTweener.PlayBackwards();
    }

    void AnimationCompleted()
    {
        SequencePuzzleManager.instance.CmdGenerateNewSequence();
    }

    void AnimationRewinded()
    {
        initialTweener.Kill();
        GameManager.instance.LoadNextScene();
    }
}
