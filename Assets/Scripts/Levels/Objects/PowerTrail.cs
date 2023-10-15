using UnityEngine;
using UnityEngine.VFX;

public class PowerTrail : MonoBehaviour, IRenderableTrail
{
    private MeshRenderer _meshRenderer;

    [SerializeField] private Material _deadMaterial;
    [SerializeField] private Material _poweredMaterial;

    [Space(15)]

    [SerializeField] private VisualEffect _virusEffect;
    [SerializeField] private MovingElement _virusEffectMover;
    [SerializeField, HideInInspector] private Transform[] _initialTrail;

    [Space(15)]

    [SerializeField, Range(0f, 1f)] private float _doVirusVisualsAt = 0.1f;
    [SerializeField, Tooltip("What charge does this element become \"powered\" at?")] private float _enableAt = 19.95f;

    [Space(15)]

    [SerializeField] private AControllable linkedControllable;

    private bool _isActive = false;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        linkedControllable.OnEnergyChange.AddListener(SetActiveStatus);
        linkedControllable.OnVirusChange.AddListener(SetVirus);

        _virusEffectMover.SetPermanentTrack(_initialTrail);
    }

    public void SetPoints(Transform[] positions) => _virusEffectMover.SetTrack(positions);

#if UNITY_EDITOR
    public void EditorOnlySetPoints(Transform[] positions)
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Cannot call Editor-Only methods while playing!");
            return;
        }

        _initialTrail = positions;
    }
#endif

    public void SetActiveStatus(float energy)
    {
        if (enabled == !_isActive)
        {
            _meshRenderer.material = energy >= _enableAt ? _poweredMaterial : _deadMaterial;
        }
    }

    public void SetVirus(float virusPercentage)
    {
        if (_virusEffect)
        {
            _virusEffect.SetFloat("Density", virusPercentage);
        }

        if (virusPercentage > _doVirusVisualsAt)
        {
            if (!_virusEffectMover.gameObject.activeInHierarchy)
            {
                _virusEffectMover.gameObject.SetActive(true);
                _virusEffectMover.Activate();
            }

            _virusEffectMover.SetSpeedModifier(4f * virusPercentage);
        }
        else
        {
            if (_virusEffectMover.gameObject.activeInHierarchy)
            {
                _virusEffectMover.gameObject.SetActive(false);
                _virusEffectMover.Deactivate();
            }
        }
    }
}
