using System;
using Interaction;
using UnityEngine;
using UnityEngine.Events;

public class PhysicsPickup : MonoBehaviour, Pickupable
{
    [SerializeField]
    Rigidbody pickupRigidBody;

    [SerializeField]
    Collider pickupCollider;

    [SerializeField]
    Vector3 pickupPositionOffset;

    [SerializeField]
    float impactThreshold = 0.5f;
	
	[SerializeField]
	bool requiresForklift = false;

    [SerializeField]
    UnityEvent onImpactThresholdMet;

    // Invoked when object is grabbed or released
    // Anything which subscribes to these events should ideally be responsible for unsubscribing 
    public Action OnGrabbed, OnDropped;

    public virtual string MessageInteract => "Press <sprite name=\"Xbox_X\"> to pick up";

    private void OnDestroy()
    {
        // Make sure everything is unsubscribed in the events when destroying
        if (OnGrabbed != null) foreach (var d in OnGrabbed.GetInvocationList()) OnGrabbed -= (Action)d;
        if (OnDropped != null) foreach (var d in OnDropped.GetInvocationList()) OnDropped -= (Action)d;
        // THIS COULD BREAK but it looks like it shouldn't...
    }

    public void Interact(InteractableControl interactableControl)
    {
        var pickupController = interactableControl.GetComponent<PickupController>();

        Grab(pickupController);
    }

    public virtual void Grab(PickupController pickupController)
    {
        if (pickupController == null || pickupController.HasPickupable)
        {
            return;
        }

        pickupController.GrabPickUp(this);
        SetPhysicsValues(true);
        OnGrabbed?.Invoke();
    }

    public virtual void Drop(PickupController pickupController)
    {
        transform.parent = null;
        SetPhysicsValues(false);
        OnDropped?.Invoke();
    }

    public virtual void Place(PickupController pickupController)
    {

        transform.position += transform.parent.forward * 2;
        transform.parent = null;
        SetPhysicsValues(false);
    }

    public virtual void Destroy(PickupController pickupController)
    {
        transform.parent = null;
        pickupController.currentPickupable = null;
        Destroy(gameObject);
    }

    public void SetPositionInParent(Transform newParent)

    {
        transform.parent = newParent;
        transform.localPosition = pickupPositionOffset;
        transform.localRotation = Quaternion.identity;

    }
    public virtual void Use()
    {

        Debug.Log("Using the pickupable object");

    }

    void SetPhysicsValues(bool wasPickedUp)
    {
        pickupRigidBody.isKinematic = wasPickedUp;
        pickupCollider.enabled = !wasPickedUp;
    }
    void OnCollisionEnter(Collision collision)
    {
        Vector3 velocity = collision.relativeVelocity;
        if (velocity.x > impactThreshold || velocity.y > impactThreshold || velocity.z > impactThreshold)
        {
            onImpactThresholdMet.Invoke();
        }
    }
	
	public bool RequiresForklift()
	{
		return requiresForklift;
	}

    public virtual void Release() { }
}
