using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace GameLib.Audio
{
	public class BGMManager : MonoBehaviour
	{
		public static BGMManager Instance;

		public enum Music
		{
			NO,
			STARTLOOP,
			WAITSTART,
			RESULTLOOP,
		}

		[Serializable]
		public struct MusicAudioPlayer
		{
			public Music music;
			public AudioPlayer source;
		}

		[SerializeField] MusicAudioPlayer[] _musicSources;

		private Dictionary<Music, AudioPlayer> _musicSourceDict = new Dictionary<Music, AudioPlayer>();

		private Music _nowMusic;

		void Awake()
		{
			Instance = this;
			_musicSourceDict = _musicSources.ToDictionary(k => k.music, v => v.source);
			_nowMusic = Music.NO;
		}

		public void Play(Music music, bool stop = true)
		{
			if (_nowMusic == music && !stop)
			{
				return;
			}
			_nowMusic = music;
			_musicSourceDict[music].Play();
		}

		public void Stop()
		{
			foreach (var one in _musicSourceDict)
			{
				one.Value.Stop();
			}
		}

		public bool IsPlaying(Music music)
		{
			return _musicSourceDict[music].Source.isPlaying;
		}
	}
}
