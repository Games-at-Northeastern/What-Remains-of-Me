using UnityEngine;
public class UpdateVirusUI : MonoBehaviour
{
    [SerializeField] private RectTransform virusTransform;
    //[SerializeField] private float _maxMaskWidth;


    // Start is called before the first frame update
    private void Start()
    {

    }

    // Update is called once per frame
    private void Update() => setCurrentVirusPercentage(PlayerRef.PlayerManager.EnergyManager.VirusPercentage);

    //
    public void setCurrentVirusPercentage(float percentage) => virusTransform.sizeDelta = new Vector2(Mathf.CeilToInt(percentage * 29),
        virusTransform.sizeDelta.y);
}
