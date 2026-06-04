using System;
using System.Security.Cryptography;
using UnityEngine;

public class PickupController : MonoBehaviour
{
    [SerializeField]
    Transform pickupHolder;

    public Pickupable currentPickupable;

    public bool HasPickupable => currentPickupable != null;

    public void GrabPickUp(Pickupable newPickup)
    {
        currentPickupable = newPickup;

        currentPickupable.SetPositionInParent(pickupHolder);
    }

    public void Update()
    {
        //CheckDropInput();

        CheckUsePickupInput();

        CheckPlaceInput();
    }   
    
    public void CheckDropInput()
    {
        if (currentPickupable != null)
        {
            currentPickupable.Drop(this);
            currentPickupable = null;
        }
 
    }

    void CheckPlaceInput()
    {
        if(Input.GetKeyDown(KeyCode.Z) && HasPickupable)
        {
            currentPickupable.Place(this);
            currentPickupable = null;
        }
    }
    void CheckUsePickupInput()
    {
        if (Input.GetMouseButtonDown(0) && HasPickupable)
        {
            currentPickupable.Use();
        }
    }
}
