using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnFarm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        public string startSceneName = string.Empty;

        private void OnEnable() {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable() {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {
            StartCoroutine(Transition(sceneToGo, positionToGo));
        }

        /// <summary>
        /// Change Scene
        /// </summary>
        /// <param name="sceneName">Scene Name</param>
        /// <param name="targetPosition">Target Position</param>
        /// <returns></returns>
        private IEnumerator Transition(string sceneName, Vector3 targetPosition)
        {
            EventHandler.CallBeforeSceneUnloadEvent();

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);

            EventHandler.CallAfterSceneLoadedEvent();
        }

        /// <summary>
        /// Load scene and set active
        /// </summary>
        /// <param name="sceneName">Scene Name</param>
        /// <returns></returns>
        private IEnumerator LoadSceneSetActive(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            Scene newSceme = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

            SceneManager.SetActiveScene(newSceme);
        }
    }
}
