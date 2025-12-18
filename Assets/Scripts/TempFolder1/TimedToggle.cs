using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// A time-based item toggler.
/// </summary>
public class TimedToggle : AEventToggle
{
    [SerializeField]
    private bool _useUnscaledTime = false;
    [SerializeField]
    private bool _resetStateUponReactivation = false;
    [SerializeField]
    private float _duration;
    [SerializeField]
    private bool delay;
    [SerializeField]
    private float delayAmount;

    private bool delayComplete = false;
    private bool _state;
    private Coroutine _coroutine;


    private void OnEnable()
    {
        if (!delay)
        {
            _state = _enabledOnStart;
            _coroutine = StartCoroutine(IEFlip());
        }
        else
        {
            StartCoroutine(StartDelay());
        }
    }

    private void OnDisable() => StopCoroutine(_coroutine);

    private IEnumerator IEFlip()
    {
        yield return _useUnscaledTime ? new WaitForSeconds(_duration) : new WaitForSecondsRealtime(_duration);

        _state = !_state; // flippy dippy doo

        FireEvent(_state);

        yield return IEFlip(); // loop IEnum
    }

    private IEnumerator StartDelay()
    {
        yield return _useUnscaledTime ? new WaitForSeconds(delayAmount) : new WaitForSecondsRealtime(delayAmount);

        _state = !_state; // flippy dippy doo

        delayComplete = true;
        _state = !_state;
        FireEvent(_state);

        yield return IEFlip();
    }

}
