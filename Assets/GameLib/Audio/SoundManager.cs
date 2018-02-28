using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GameLib.Audio
{

	public class SoundManager : MonoBehaviour
	{

		public static SoundManager Instance;

		[SerializeField] SoundSeting _setting;

		[SerializeField] SeSource _sourcePrefab;

		private SoundSEPool _pool;

		List<SeSource> _playSources = new List<SeSource>();


		void Awake()
		{
			Instance = this;
			_pool = new SoundSEPool(_sourcePrefab, this.transform);

			foreach (var one in _setting.ses.Where(p => p.clip != null).Select(p => p.type))
			{
				PreLoad(one);
			}
		}


		void PreLoad(SoundSeting.Type type)
		{
			var source = _pool.Rent();
			var query = _setting.ses.Where(p => p.type == type);
			if (query.Any())
			{
				source.PreLoad(query.First().clip);
				_pool.Return(source);
			}
		}


		public void QuantizePlay(SoundSeting.Type type, Vector3 position, float endTime = 0f)
		{

			var source = _pool.Rent();
			var query = _setting.ses.Where(p => p.type == type);
			if (query.Any())
			{
				bool isStop = false;
				if (endTime > 0)
				{
					isStop = true;
				}
				_playSources.Add(source);
				source.Play(query.First(), position, false);
				//Music.QuantizePlay (source.source);
				if (isStop)
				{
					StartCoroutine(QuantizePlayCoroutine(source, endTime));
				}
				else
				{
					StartCoroutine(QuantizePlayCoroutine(source, source.source.clip.length));
				}
			}
		}


		public void Play(SoundSeting.Type type, Vector3 position, float loopTime = 0f)
		{
			var source = _pool.Rent();
			var query = _setting.ses.Where(p => p.type == type);
			if (query.Any())
			{
				bool isLoop = false;
				if (loopTime > 0)
				{
					isLoop = true;
				}
				_playSources.Add(source);
				source.Play(query.First(), position, isLoop);
				if (isLoop)
				{
					StartCoroutine(StopLoopCoroutine(source, loopTime));
				}

				StartCoroutine(WaitPlayCoroutine(source));
			}
		}


		public void Stop(SoundSeting.Type type)
		{

			foreach (var one in _playSources)
			{
				if (one != null && one.se.type == type)
				{
					one.Stop();
					break;
				}
			}
		}


		public void Stop()
		{

			foreach (var one in _playSources)
			{
				if (one != null)
				{
					one.Stop();
				}
			}
		}


		IEnumerator StopLoopCoroutine(SeSource source, float time)
		{
			yield return new WaitForSeconds(time);
			source.Stop();
		}


		IEnumerator QuantizePlayCoroutine(SeSource source, float stopTime)
		{
			while (true)
			{
				if (Music.IsJustChangedBeat())
				{
					break;
				}
				yield return null;
			}

			while (source.IsPlay())
			{

				if (stopTime > 0)
				{
					if (
						Music.IsJustChangedBeat() &&
						source.source.time >= stopTime)
					{
						break;
					}
				}


				yield return null;
			}
			_playSources.Remove(source);
			_pool.Return(source);
		}


		IEnumerator WaitPlayCoroutine(SeSource source)
		{

			while (source.IsPlay())
			{

				yield return null;
			}
			_playSources.Remove(source);
			_pool.Return(source);
		}

	}
}
