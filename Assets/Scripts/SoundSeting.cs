using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DataBase/Create SoundSeting", fileName = "SoundSeting")]
public class SoundSeting : ScriptableObject {

	public enum Type
	{
		HoldSE1,
		HoldSE2,
		HoldSE3,
		LASERSE1,
		INVADERKILL,
		UFO,
		INVADERBGM1,
		INVADERBGM2,
		INVADERBGM3,
		INVADERBGM4,
		INVADERGAMECLEAR,
		SWITCHALARM,
		SWITCHGAMEOVER,
		SWITCHGAMECLEAR,
		SWUTCHPUSHSWITCH,
		SWITCHLOOPSWITCH,
		COUNTDOWNVOICE,

		RANKSS = 100,
		RANKS,
		RANKA,
		RANKB,
		RANKC,
		RANKD,

		SE_APPLAUSE02 = 200,
	}

	[System.Serializable]
	public class Se  {
		public Type	type;
		public AudioClip clip;
	}

	public Se[] ses;
}
