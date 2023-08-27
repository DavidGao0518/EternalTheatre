using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace Manager
{
    public class CutsceneBehaviour : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private bool stopMusic;

		[Header("Fade")]
        [SerializeField] private bool useFadeOut;
        [SerializeField] private float fadeStartFrom = 3;
        [SerializeField] private float fadeDuration = 3;

        // Start is called before the first frame update
        void Start()
        {
            if (stopMusic) AppManager.GetInstance().musicManager.StopMusic(); //TODO or play music here
            videoPlayer.loopPointReached += EndReached;

            if (useFadeOut) Invoke(nameof(FadeRoutine), fadeStartFrom);
        }

        void EndReached(VideoPlayer vp)
        {
            //vp.playbackSpeed = vp.playbackSpeed / 10.0F;
            AppManager.GetInstance().sceneLoader.LoadNextScene();
        }

        void FadeRoutine()
		{
            AppManager.GetInstance().musicManager.StopMusic(fadeDuration);
		}
    }

}

