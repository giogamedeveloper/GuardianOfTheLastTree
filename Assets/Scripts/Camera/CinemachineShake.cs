using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CinemachineShake : MonoBehaviour
{
    public CinemachineCamera cinemachineCamera;

    //Componente de cinemachine que deberá ser recuperado por codigo, que nos permitirá modificar el comportamiento del noise
    private CinemachineBasicMultiChannelPerlin _virtualCameraNoise;

    //perfil por defectro del Shake
    public ShakeSO defaultProfile;

    //Almacenamiento de los valores originales de los valores de la camara
    NoiseSettings _originalNoise;
    private float _originalAmplitude;
    private float _originalFrequency;
    private Coroutine _shakeCoroutine;

    private static CinemachineShake _instance;
    public static CinemachineShake Instance => _instance;

    void Awake()
    {
        if (_instance == null) _instance = this;
        else Destroy(this);
    }

    /// <summary>
    ///  Start is called once before the first execution of Update after the MonoBehaviour is created
    /// </summary>
    void Start()
    {
        _virtualCameraNoise = cinemachineCamera.GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    /// <summary>
    ///Incluia el shake de la camara pudiendo recibir un scriptrable como parametro con los valores del shalke recibido 
    /// </summary>
    /// <param name="profile"></param>
    public void StartShake(ShakeSO profile = null)
    {
        if (profile == null)
        {
            profile = defaultProfile;
        }
        if (_shakeCoroutine != null) StopCoroutine(_shakeCoroutine);
        _shakeCoroutine = StartCoroutine(Shake(profile));
    }

    /// <summary>
    /// Corrutina qye almacena el estado previo, realiza el shake indicado durante el tiempo indicado y por úlimo restautra el estado previo
    /// </summary>
    /// <param name="profile"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    private IEnumerator Shake(ShakeSO profile)
    {
        float timer = profile.shakeDuration;

        _originalNoise = _virtualCameraNoise.NoiseProfile;
        _originalAmplitude = _virtualCameraNoise.AmplitudeGain;
        _originalFrequency = _virtualCameraNoise.FrequencyGain;

        _virtualCameraNoise.NoiseProfile = profile.noiseType;

        while (timer > 0f)
        {
            _virtualCameraNoise.AmplitudeGain = profile.amplitudCurve.Evaluate(timer / profile.shakeDuration);
            _virtualCameraNoise.FrequencyGain = profile.frequencyCurve.Evaluate(timer / profile.shakeDuration);
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _virtualCameraNoise.NoiseProfile = _originalNoise;
        _virtualCameraNoise.AmplitudeGain = _originalAmplitude;
        _virtualCameraNoise.FrequencyGain = _originalFrequency;
    }
}
