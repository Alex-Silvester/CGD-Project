using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static CrateExtensions;

/// <summary>
///  This class is mostly for demonstration.
///  Represents a crate that can be collected.
/// </summary>
public class CrateObject : MonoBehaviour, ICollectable
{
    [Header("ICollectable values")]
    [SerializeField]
    float score;

    [SerializeField]
    CrateTag crateTag;

    [SerializeField]
    bool useColouredTags = true;

    [Space]
    [SerializeField]
    ParticleEffectLibrary effectLibrary;

    [SerializeField] 
    GameObject tempNewCrateMeshEdges, tempNewCrateMeshSupports;

    [SerializeField, Range(0f, 100f)]
    float dropLaunchForce = 5f;

    [SerializeField]
    UnityEvent onDestroyed;

    float maxScore;
    string startingPromptText;
    ContextualPromptSource promptSource;
    PhysicsPickup pickup;
    TextMeshPro[] textObjects;
    Material material;
    Rigidbody body;


    // As in the minimum score the crate can have
    float MaximumScoreReduction => maxScore * (1 - DamageBehaviour.maximumScoreLossPercentage) - DamageBehaviour.maximumScoreLossValue;

    private static readonly float floatingTextPlayerDetectionRadius = 1f;

    /// <summary>
    /// Instantiate and initialise a new crate.
    /// </summary>
    /// <param name="prefab">Prefab to use, must have a ICollectable.</param>
    /// <param name="transform">Transform to spawn and parent to.</param>
    /// <param name="crateTag">Tag for the crate.</param>
    /// <param name="score">Starting score of the crate.</param>
    /// <returns></returns>
    public static ICollectable Instantiate(GameObject prefab, Transform transform, CrateTag crateTag, DamageBehaviour damageBehaviour, float score)
    {
        var collectable = Instantiate(prefab, transform).GetComponent<ICollectable>();
        collectable.Tag = crateTag;
        collectable.Score = collectable.MaxScore = score;
        collectable.DamageBehaviour = damageBehaviour;
        return collectable;
    }

    private void Awake()
    {
        CanCollect = CanDamage = true;
        body = GetComponent<Rigidbody>();
        material = GetComponent<Renderer>().material;
        textObjects = GetComponentsInChildren<TextMeshPro>();
        promptSource = GetComponentInChildren<ContextualPromptSource>();

        startingPromptText = "<sprite name=\"Xbox_Y\">";

        // Bind grabbing event to pickup controller
        if (!TryGetComponent(out pickup))
        {
            Debug.LogWarning("No PhysicsPickup on CrateObject");
        }

    }

    private void OnEnable()
    {
        if (pickup != null)
        {
            pickup.OnGrabbed += OnGrabbed;
            pickup.OnDropped += OnDropped;
        }
    }

    private void OnDisable()
    {
        if (pickup != null)
        {
            pickup.OnGrabbed -= OnGrabbed;
            pickup.OnDropped -= OnDropped;
        }
    }

    // Handles taking damage on collisions if this crate is going fast enough
    private void OnCollisionEnter(Collision collision)
    {
        var relativeVelocity = collision.relativeVelocity;
        if (relativeVelocity.magnitude > DamageBehaviour.collisionVelocityForCrateDamage)
        {
            if (CanDamage) DamageCrate(relativeVelocity, collision.contacts[0].point);
        }
    }

    public float MaxScore
    {
        get => maxScore;
        set => maxScore = value;
    }

    public float Score
    {
        get => score;
        set
        {
            // Object is destroyed if score reaches 0
            score = value;
            if (score <= 0)
            {
                Destroy(GameObject, 1.3f);
                //onDestroyed uses audio for when the crate's number reaches 0 - Callum.S
                onDestroyed.Invoke();
            }

            else UpdateTextObjects();
        }
    }

    public CrateTag Tag
    {
        get { return crateTag; }
        set
        {
            crateTag = value;
            if (useColouredTags) RecolourCrate();
        }
    }

    public DamageBehaviour DamageBehaviour { get; set; }

    public GameObject GameObject { get => gameObject; }
    public bool CanCollect { get; set; }

    public bool CanDamage { get; set; }

    // Colour this object based on its tag
    void RecolourCrate()
    {
        material.SetColor("_Color", crateTag.GetColourFromTag());

        //Temporary fix
        tempNewCrateMeshEdges.GetComponent<Renderer>().material.SetColor("_Color", crateTag.GetColourFromTag());
        tempNewCrateMeshSupports.GetComponent<Renderer>().material.SetColor("_Color", crateTag.GetColourFromTag());
    }

    // Make the object collect-able or not
    void OnGrabbed()
    {
        UpdatePromptTextToDrop();
        CanCollect = CanDamage = false;
        body.isKinematic = true;
    }

    void OnDropped()
    {
        UpdatePromptTextToGrab();
        CanCollect = CanDamage = true;
        body.isKinematic = false;
        // -transform.right is apparently the forward direction of the crate relative to the forklift's forward direction
        body.AddForce(-transform.right * dropLaunchForce, ForceMode.Impulse);
    }

    // Reduces the crate's score and displays the text for that
    private void DamageCrate(Vector3 relativeVelocity, Vector3 position)
    {
        // Handle literal scores as integers - cast as int
        float temp = Score;
        float damage = (int)GetScoreLoss(relativeVelocity);
        int reduction = (int)Mathf.Max(MaximumScoreReduction, Score - damage);
        if (reduction != temp)
        {
            InstanceDamageText(damage);
            PlayCollisionSparks(position, Quaternion.Euler(relativeVelocity.normalized));
        }
        Score = reduction;
    }

    // Play the collision spark effect when taking damage
    private void PlayCollisionSparks(Vector3 position, Quaternion direction)
    {
        var effect = effectLibrary.Get<ColoredParticleEffect>(ParticleEffectLibrary.CrateCollisionSparks);
        effect.color = effect.emissionColor = crateTag.GetColourFromTag();
        effect.AtPosition(position)
              .AtRotation(direction)
              .Play();
    }

    // Creates text representing damage that the crate will take
    private void InstanceDamageText(float damage)
    {
        if (damage > 0)
        {
			// Get direction to nearest player (most likely the one that hit the crate)
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, floatingTextPlayerDetectionRadius);
			foreach (var hitCollider in hitColliders)
			{
				// Check if the collider has the "Player" tag
				if (hitCollider.CompareTag("Player"))
				{
					// Spawn floating text
					FloatingTextManager.instance.Create($"-{damage}", transform.position, hitCollider.transform, Color.red);
					
					break; // Stop once we find the first player
				}
			}
        }
    }

    // Returns how much score would be lost based on the relative velocity of a collision
    float GetScoreLoss(Vector3 relativeVelocity) => (relativeVelocity.magnitude - DamageBehaviour.collisionVelocityForCrateDamage) * DamageBehaviour.damageCoefficient;

    // Displays the current score in the textObjects
    void UpdateTextObjects()
    {
        foreach (var tmp in textObjects)
        {
            tmp.SetText(score.ToString());
        }
    }

    // Particularly ugly ways to change the prompt text
    void UpdatePromptTextToGrab()
    {
        if (promptSource == null) return;

        promptSource.message = $"{startingPromptText} Grab";
    }

    void UpdatePromptTextToDrop()
    {
        if (promptSource == null) return;

        promptSource.message = $"{startingPromptText} Drop";
    }
}
