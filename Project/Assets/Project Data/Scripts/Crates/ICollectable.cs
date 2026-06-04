using UnityEngine;

/// <summary>
/// An interface which can designate objects as being collectable (crates and such).
/// If we want collectables to hold some data, implement it here.
/// </summary>
public interface ICollectable
{
    public float MaxScore { get; set; }
    public float Score { get; set; }
    //CanCollect is being used to determine if the object  is held currently
    public bool CanCollect { get; set; }
    public bool CanDamage { get; set; }
    public GameObject GameObject { get; }
    public CrateExtensions.DamageBehaviour DamageBehaviour { get; set; }
    public CrateExtensions.CrateTag Tag { get; set; }

}
