using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

namespace GameLib.Util
{
	public class SceneLoader : MonoBehaviour
	{

		public static SceneLoader Instance;

		[SerializeField] SceneObject mainScene;
		

		[Serializable]
		public class SceneLevel
		{
			public string parentLevelName;
			public SceneObject sceneObj;
			public bool isActive;
			public bool isDontDestroy;
		}

		List<SceneLevel> _preScenes = new List<SceneLevel>();

		private void Awake()
		{
			Instance = this;
			
		}



		public IEnumerator SceneLoad(SceneLevel sceneLevel)
		{
			var scene = GetScene(sceneLevel.sceneObj);
			Debug.LogFormat("scene ={0}", scene.IsValid());
			if (scene.IsValid())
			{

			}
			else
			{

				if(_preScenes.Count > 0)
				{
					SetActiveScene(mainScene);
					var removeList = new List<SceneLevel>();
					foreach (var one in _preScenes)
					{
						if (one.isDontDestroy) continue;
						if (one.parentLevelName == sceneLevel.parentLevelName) continue;
						scene = GetScene(one.sceneObj);
						SceneManager.UnloadScene(scene);
						removeList.Add(one);
						yield return null;
					}
					foreach(var one in removeList)
					{
						_preScenes.Remove(one);
					}
					
				}



				yield return SceneManager.LoadSceneAsync(sceneLevel.sceneObj.sceneName, LoadSceneMode.Additive);


				if (!sceneLevel.isDontDestroy)
				{
					_preScenes.Add(sceneLevel);
				}
				
				if (sceneLevel.isActive)
				{
					SetActiveScene(sceneLevel.sceneObj);
				}
			}
		}

		private Scene GetScene(SceneObject scene)
		{
			return SceneManager.GetSceneByName(scene.sceneName);
		}


		private void SetActiveScene(SceneObject scene)
		{
			SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene.sceneName));
		}


	}
}

