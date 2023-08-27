using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeUtil : MonoBehaviour {
    public static CameraShakeUtil Instance { get; private set; }
    
    [SerializeField] private CinemachineVirtualCamera vcam;
    private float _shakeTimer;

    private void Awake() {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

    private void Update() {
        if (vcam != null && _shakeTimer > 0) {
            _shakeTimer -= Time.deltaTime;

            if (_shakeTimer <= 0) {
                CinemachineBasicMultiChannelPerlin perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                perlin.m_AmplitudeGain = 0;
                perlin.m_FrequencyGain = 0;
            }
        }
    }

    public void ShakeCamera(float amplitude, float frequency, float time) {
        CinemachineBasicMultiChannelPerlin perlin = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        perlin.m_AmplitudeGain = amplitude;
        perlin.m_FrequencyGain = frequency;
        _shakeTimer = time;
    }
}