namespace Levels.Objects.Platform
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// A virus effector that makes a moving element go along its track in an erratic manner.
    /// </summary>
    public class HecticMovementEffector : AMovingElementVirusEffector
    {
        [SerializeField] private bool _doRandomizeToggle = true;
        
        [SerializeField] private Range _randomRange;

        [Space(15)]

        [SerializeField] private bool _doRandomizeSpeed = false;
        [SerializeField] private Range _speedRange;

        private bool _isEffectorActive = false;
        private Dictionary<int, (Coroutine coroutine, bool initialState)> _processMap;

        private void Awake()
        {
            _randomRange.ValidateRange();

            if (_doRandomizeSpeed)
            {
                _speedRange.ValidateRange();
            }

            _processMap = new();
        }


        // sorta an invariant, but this method is only called when the state of the effector needs to change.
        protected override void AffectMovingElement(MovingElement element)
        {
            int instanceID = element.GetInstanceID();

            // if we're active, disable
            if (_isEffectorActive)
            {
                if (_processMap.TryGetValue(instanceID, out var tuple))
                {
                    element.StopCoroutine(tuple.coroutine);
                    element.SetDir(tuple.initialState);
                }
                else
                {
                    // potential error if a platform is created/destroyed during runtime
                    Debug.LogError($"{element.gameObject.name} does not have an entry in the map of processes! Are you creating the element during runtime?");
                }

                _isEffectorActive = false;
            }
            else // if we're disabled, activate
            {
                var tuple = (element.StartCoroutine(IEProcess(element)), element.GetDir());

                // if a key already exists, then just override the value
                if (_processMap.ContainsKey(instanceID))
                {
                    _processMap[instanceID] = tuple;
                }
                else // if it does not exist, create it with a value
                {
                    _processMap.Add(instanceID, tuple);
                }

                _isEffectorActive = true;
            }
        }

        // this is the same guard clause logic as ChaosEffector.
        // It's super useful for turning rapid-fire invocation into a more event-driven approach, filtering
        // out all the meaningless updates. Maybe this should be the default implementation for ShouldDoEffect(...) instead
        // of the one that's currently there.
        protected override bool ShouldDoEffect(float newVirusPercentage) => (!_isEffectorActive && newVirusPercentage >= _doVirusEffectAt) || (_isEffectorActive && newVirusPercentage < _doVirusEffectAt);

        private IEnumerator IEProcess(MovingElement element)
        {
            if (_doRandomizeSpeed)
            {
                element.SetRandomSpeedModifier(_speedRange.GetValue()); // what's the difference between random speed mod and speed mod?
            }

            while (true)
            {
                yield return new WaitForSeconds(_randomRange.GetValue());

                if (_doRandomizeToggle) {
                    element.ToggleDir();   
                }

                if (_doRandomizeSpeed)
                {
                    element.SetRandomSpeedModifier(_speedRange.GetValue()); // what's the difference between random speed mod and speed mod?
                }
            }
        }
    }

}
