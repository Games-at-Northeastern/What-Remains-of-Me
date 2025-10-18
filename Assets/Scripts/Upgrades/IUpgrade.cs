using UnityEngine;

public enum UpgradeType
{
    VOICEMODULE
}
public abstract class IUpgrade : MonoBehaviour 
{
    /// <summary>
    /// The type of the upgrade this upgrade is
    /// </summary>
    public abstract UpgradeType GetUpgradeType();

    /// <summary>
    /// What the upgrade does when you first aquire it(should also turn it on, but could not if needed)
    /// </summary>
    public abstract void Aquire();

    /// <summary>
    /// How the upgrade should be turned on in the level, other than the first aquirement
    /// </summary>
    public abstract void TurnOn();

}
