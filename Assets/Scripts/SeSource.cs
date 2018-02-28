using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SeSource : MonoBehaviour {

	public	SoundSeting.Se	se
	{
		get {
			return _se;
		}
	}

	public AudioSource source
	{
		get {
			return _source;
		}
	}

	private SoundSeting.Se	_se;

	private AudioSource	_source;

	void Awake()
	{
		_source = GetComponent<AudioSource> ();
	}


	public void Play(SoundSeting.Se se, Vector3 position, bool isLoop = false)
	{
		_se = se;
		_source.clip = se.clip;
		_source.transform.position = position;
		_source.loop = isLoop;
		_source.Play ();
	}


	public void Stop()
	{
		_source.Stop ();
	}


	public bool IsPlay()
	{
		return _source.isPlaying;
	}


	public void PreLoad(AudioClip clip)
	{
		_source.clip = clip;
		_source.clip.LoadAudioData ();
	}

}
