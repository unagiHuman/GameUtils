using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx.Toolkit;

public class SoundSEPool : ObjectPool<SeSource> {

	private	SeSource	_prefab;

	private	Transform	_parent;

	private SoundSeting.Se _se;

	public SoundSEPool(SeSource	prefab, Transform parent){
		_prefab	= prefab;
		_parent = parent;
	}
		
	protected override SeSource CreateInstance ()
	{
		var audioSource = GameObject.Instantiate (_prefab);
		audioSource.transform.parent = _parent;
		return audioSource;
	}
}
