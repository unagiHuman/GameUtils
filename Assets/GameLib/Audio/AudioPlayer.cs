using UnityEngine;
using System.Collections;

namespace GameLib.Audio
{
	public class AudioPlayer : MonoBehaviour
	{

		public static AudioPlayer Instance;

		/// <summary>
		/// オーディオソース。
		/// </summary>
		public AudioSource Source;

		/// <summary>
		/// フェードイン再生を行うかどうか。
		/// </summary>
		public bool IsFade;

		/// <summary>
		/// フェードインする時の時間。
		/// </summary>
		public double FadeInSeconds = 1.0;

		/// <summary>
		/// フェードイン再生中かどうか
		/// </summary>
		bool IsFadePlaying = false;

		/// <summary>
		/// フェードアウト再生中かどうか
		/// </summary>
		bool IsFadeStopping = false;

		/// <summary>
		/// フェードアウトする時の時間。
		/// </summary>
		double FadeOutSeconds = 1.0;

		/// <summary>
		/// フェードイン/アウト経過時間。
		/// </summary>
		double FadeDeltaTime = 0;

		/// <summary>
		/// 基本ボリューム。
		/// </summary>
		float BaseVolume;


		void Awake()
		{
			Instance = this;
		}

		/// <summary>
		/// フレーム毎処理。
		/// </summary>
		void Update()
		{
			// フェードイン
			if (IsFadePlaying)
			{
				FadeDeltaTime += Time.deltaTime;
				if (FadeDeltaTime >= FadeInSeconds)
				{
					FadeDeltaTime = FadeInSeconds;
					IsFadePlaying = false;
				}
				Source.volume = (float)(FadeDeltaTime / FadeInSeconds) * BaseVolume;
			}

			// フェードアウト
			if (IsFadeStopping)
			{
				FadeDeltaTime += Time.deltaTime;
				if (FadeDeltaTime >= FadeOutSeconds)
				{
					FadeDeltaTime = FadeOutSeconds;
					IsFadePlaying = false;
					Source.Stop();
				}
				Source.volume = (float)(1.0 - FadeDeltaTime / FadeOutSeconds) * BaseVolume;
			}
		}

		/// <summary>
		/// 再生を行います。
		/// </summary>
		public void Play()
		{
			if (Source.isPlaying)
			{
				if (IsFade)
				{
					StopFadeOut(FadeInSeconds);
				}
				else
				{
					Stop();
				}
			}
			BaseVolume = 1;
			FadeDeltaTime = 0;
			Source.volume = 0;
			Source.Play();
			IsFadePlaying = true;
			IsFadeStopping = false;
		}

		/// <summary>
		/// 停止を行います。
		/// </summary>
		public void Stop()
		{
			Source.Stop();
		}

		/// <summary>
		/// 停止を行います。
		/// <param name="fadeSec">フェードアウト完了までの秒数。</param>
		/// </summary>
		public void StopFadeOut(double fadeSec)
		{
			BaseVolume = Source.volume;
			FadeDeltaTime = 0;
			FadeOutSeconds = fadeSec;
			IsFadeStopping = true;
			IsFadePlaying = false;
		}

		/// <summary>
		/// 一時停止を行います。
		/// </summary>
		public void Pause()
		{
			Source.Pause();
		}
	}
}