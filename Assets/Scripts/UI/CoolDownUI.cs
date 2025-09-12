using System;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownUI : MonoBehaviour
{
    public Image shadowImage;

    private float _coolDownTimer;
    private float _coolDownTime;
    bool _loading;


    protected virtual void OnEnable()
    {
    }

    protected virtual void OnDisable()
    {
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        shadowImage.fillAmount = 0f;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!_loading) return;
        if (_coolDownTimer > 0f)
        {
            shadowImage.fillAmount = _coolDownTimer / _coolDownTime;
        }
        else if (_loading)
        {
            _loading = false;
            shadowImage.fillAmount = 0f;
        }
        _coolDownTimer -= Time.deltaTime;
    }

    protected void StartCoolDown(float time)
    {
        _coolDownTimer = time;
        _coolDownTime = time;
        _loading = true;

    }
}
