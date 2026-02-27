//using NUnit.Framework;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;


//public class LiftPickUp : MonoBehaviour
//{
//    [SerializeField] GameObject PickupLocation;
//    [SerializeField] GameObject ForkliftLeftLocation;
//    [SerializeField] GameObject ForkliftRightLocation;
//    [SerializeField] GameObject ForkliftBackLocation;
//    [SerializeField] Vector3 pickupPositionOffset;


//    //[SerializeField] List <GameObject> pickupList = new();


//    [SerializeField] bool forkLiftSelected;
//    [SerializeField] bool hasObject;
//    [SerializeField] private GameObject heldObject;

//    //public UnityEvent onGrabbed = new UnityEvent();
//    //public UnityEvent onDropped = new UnityEvent();


//    // Start is called once before the first execution of Update after the MonoBehaviour is created
//    void Start()
//    {

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (heldObject == null)
//            hasObject = false;
//        else
//            hasObject = true;


//        if (pickupList.Count == 0)
//            return;

//        if (pickupList[0].gameObject.tag == "Player")
//        {
//            forkLiftSelected = true;
//        }
//        else
//        {
//            forkLiftSelected = false;
//        }

//    }

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.tag == "Float")
//        {
//            Debug.Log("Crate in Pickup Radius");
//            pickupList.Add(other.gameObject);
//        }
//        else if (other.tag == "Player")
//        {
//            Debug.Log("Forklift in Pickup Radius");
//            pickupList.Add(other.gameObject);
//        }
//    }

//    private void OnTriggerExit(Collider other)
//    {
//        pickupList.Remove(other.gameObject);
//    }

//    public void PickUpSelected()
//    {
//        if (pickupList.Count == 0)
//            return;

//        if (!hasObject)
//        {
//            heldObject = pickupList[0];

//            SetPositionInParent(heldObject.gameObject.transform);

//            // Invoke grab event if it exists
//            if (heldObject.TryGetComponent<PhysicsPickup>(out var pickup))
//            {
//                Debug.Log("Invoking onpickup");
//                pickup.OnGrabbed.Invoke();

//                // Let visual cue elements know the forklift has picked up a crate
//                onGrabbed?.Invoke();
//            }
//        }
//        else if (hasObject)
//        {
//            // Invoke drop event if it exists
//            if (heldObject.TryGetComponent<PhysicsPickup>(out var pickup))
//            {
//                Debug.Log("Invoking ondrop");
//                pickup.OnDropped.Invoke();

//                // Let visual cue elements know the forklift has dropped a crate
//                onDropped?.Invoke();
//            }

//            UnsetPositionInParent(heldObject.transform);
//            heldObject = null;
//        }
//    }
//    //
//    //public void SetPositionInParent(Transform newPosition)
//    //
//    //{
//    //    newPosition.parent = PickupLocation.transform;
//    //    newPosition.transform.position = PickupLocation.transform.position;
//    //    newPosition.transform.rotation = PickupLocation.transform.rotation;
//    //    newPosition.GetComponent<Rigidbody>().isKinematic = true;
//    //}
//    //
//    //public void UnsetPositionInParent(Transform newPosition)
//    //{
//    //    newPosition.parent = null;
//    //    newPosition.GetComponent<Rigidbody>().isKinematic = false;
//    //
//    //    newPosition.GetComponent<Collider>().enabled = true;
//    //}

//    //public void PickUpSelectedForklift()
//    //{
//    //    if (forklift_selected == true && !has_object && has_forklift == false)
//    //    {
//    //        Debug.Log($"Forklift Interaction with {hit.collider.name}");
//    //
//    //        GameObject lifting_forklift = hit.collider.gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject;
//    //        lifting_forklift.GetComponent<Collider>().enabled = false;
//    //
//    //        if (hit.collider.tag == "LeftSide")
//    //        {
//    //            SetForkliftPositionInParent(lifting_forklift.transform, ForkliftLeftLocation.transform);
//    //            lifting_forklift.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
//    //            held_object = lifting_forklift;
//    //            has_forklift = true;
//    //        }
//    //
//    //        if (hit.collider.tag == "RightSide")
//    //        {
//    //            SetForkliftPositionInParent(lifting_forklift.transform, ForkliftRightLocation.transform);
//    //            lifting_forklift.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
//    //            held_object = lifting_forklift;
//    //            has_forklift = true;
//    //        }
//    //
//    //        if (hit.collider.tag == "BackSide")
//    //        {
//    //            lifting_forklift.transform.rotation = Forkcast.transform.rotation;
//    //            SetForkliftPositionInParent(lifting_forklift.transform, ForkliftBackLocation.transform);
//    //            held_object = lifting_forklift;
//    //            has_forklift = true;
//    //        }
//    //
//    //        if (has_forklift)
//    //        {
//    //            held_object.GetComponent<DrivingController>().togglePlayerLifted();
//    //        }
//    //    }
//    //    else if (forklift_selected == false && held_object != null && has_forklift == true)
//    //    {
//    //        //Debug.Log("Dropping Forklift");
//    //        ray_dist = 1.5f;
//    //        UnsetPositionInParent(held_object.transform);
//    //        held_object.GetComponent<DrivingController>().togglePlayerLifted();
//    //        has_forklift = false;
//    //    }
//    //}
//}
