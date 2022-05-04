using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace AnFarm.Transition
{
    public class TransitionManager : MonoBehaviour
    {
        [SceneName]
        public string startSceneName = string.Empty;

        private CanvasGroup FadeCanvasGroup;
        private bool isFade;

        private void OnEnable()
        {
            EventHandler.TransitionEvent += OnTransitionEvent;
        }

        private void OnDisable()
        {
            EventHandler.TransitionEvent -= OnTransitionEvent;
        }

        private void Start()
        {
            StartCoroutine(LoadSceneSetActive(startSceneName));

            FadeCanvasGroup = FindObjectOfType<CanvasGroup>();
        }

        private void OnTransitionEvent(string sceneToGo, Vector3 positionToGo)
        {           
            if(!isFade)
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

            yield return Fade(1);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

            yield return LoadSceneSetActive(sceneName);

            // Move player position
            EventHandler.CallMoveToPosition(targetPosition);

            EventHandler.CallAfterSceneLoadedEvent();
            
            yield return Fade(0);
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

        /// <summary>
        /// Scene FadeIn FadeOut
        /// </summary>
        /// <param name="targetAlpha">1 is Black, 0 is transparent</param>
        /// <returns></returns>
        private IEnumerator Fade(float targetAlpha)
        {
            isFade = true;

            FadeCanvasGroup.blocksRaycasts = true;

            float speed = Mathf.Abs(FadeCanvasGroup.alpha - targetAlpha) / Settings.fadeDuration;

            while (!Mathf.Approximately(FadeCanvasGroup.alpha, targetAlpha))
            {
                FadeCanvasGroup.alpha = Mathf.MoveTowards(FadeCanvasGroup.alpha, targetAlpha, speed * Time.deltaTime);
                yield return null;
            }

            FadeCanvasGroup.blocksRaycasts = false;

            isFade = false;
        }
    }
}
