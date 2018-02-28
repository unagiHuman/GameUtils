using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System;

namespace GameLib.Util
{

	/// <summary>
	/// on offが急激に切り替わる場合に安定した結果を取得できるクラス
	/// </summary>
	public class BoolHysteresis : MonoBehaviour
	{

		public bool onTrue
		{
			get
			{
				return _onTrue;
			}
		}


		// 衝突判定の平滑用
		public bool onTrueRaw
		{
			set;
			get;
		}

		public float onTrueCurrent;

		public ReactiveProperty<bool> onTrueReactiveProperty;

		[SerializeField] float _filterConstant;

		/// <summary>
		/// この期間Trueであった場合のみTrueにする
		/// </summary>
		[SerializeField] float _enableTime = 1f;

		// カットオフ周波数：小さいほど急な動きが抑制される
		[SerializeField] private float _cutOffFrequency = 30.0f;
		public float CutOffFrequency
		{
			get { return _cutOffFrequency; }
			set
			{
				_cutOffFrequency = value;
				CalculateFilterConstant();
			}
		}

		// 衝突判定の閾値：上限と下限を用意して判定の変化にヒステリシスを持たせる
		[Range(0.0f, 1.0f)] public float detectionThresholdUpper = 0.9f;
		[Range(0.0f, 1.0f)] public float detectionThresholdLower = 0.1f;



		private float _onTrueValue = 0f;
		private bool _onTrue = false;


		void Start()
		{
			onTrueReactiveProperty =
				this.ObserveEveryValueChanged(p => p._onTrue)
					.Throttle(TimeSpan.FromMilliseconds(1000))
					.ToReactiveProperty()
					.AddTo(gameObject);
		}

		/// <summary>
		/// Starts the histeresis loop.
		/// </summary>
		public void StartHisteresisLoop(Func<bool> boolReturnFunc)
		{
			StartCoroutine(CalcHisteresisLoopCotourine(boolReturnFunc));
		}


		// カットオフ周波数から衝突判定平滑用の定数を計算
		private void CalculateFilterConstant()
		{
			_filterConstant = Mathf.Exp(-CutOffFrequency * Time.fixedDeltaTime);
		}


		// インスペクタ上でカットオフ周波数を変更した時用
		private void OnValidate()
		{
			CalculateFilterConstant();
		}


		private IEnumerator IsNotChanged()
		{
			while (true)
			{

				if (_onTrue)
				{



				}

				yield return null;
			}
		}


		/// <summary>
		/// Calculates the histeresis loop cotourine.
		/// </summary>
		/// <returns>The histeresis loop cotourine.</returns>
		private IEnumerator CalcHisteresisLoopCotourine(Func<bool> boolReturnFunc)
		{

			// 値の初期化
			if (_onTrue)
				_onTrueValue = 1.0f;
			else
				_onTrueValue = 0.0f;
			CalculateFilterConstant();

			while (true)
			{

				onTrueRaw = boolReturnFunc();

				bool onTruePrevious = _onTrue;
				onTrueCurrent = 0;
				if (onTrueRaw)
					onTrueCurrent = 1;
				// 衝突しているかどうかを数値化。_filterConstantが大きいほど1サイクル前の状態を優先
				_onTrueValue = (1.0f - _filterConstant) * onTrueCurrent + _filterConstant * _onTrueValue;
				// _onCollisionFloatの値を0.0~1.0に制限
				if (_onTrueValue > 1.0f)
					_onTrueValue = 1.0f;
				else if (_onTrueValue < 0.0f)
					_onTrueValue = 0.0f;

				if (onTruePrevious)
				{
					// 1つ前のサイクルで衝突している場合、下限閾値を下回るまで状態を変化させない
					if (_onTrueValue <= detectionThresholdLower)
						_onTrue = false;
					else
						_onTrue = true;
				}
				else
				{
					// 1つ前のサイクルで衝突していない場合、上限閾値を上回るまで状態を変化させない
					if (_onTrueValue >= detectionThresholdUpper)
						_onTrue = true;
					else
						_onTrue = false;
				}

				if (_onTrue == false)
				{
					onTrueReactiveProperty.Value = _onTrue;
				}

				yield return null;
			}

		}

	}
}
