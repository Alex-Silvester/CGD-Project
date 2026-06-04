using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Arranges the position for the children of this component into a grid space.
/// The width and height of the arrangement (x and y) are fixed. Its length is not fixed.
/// It will only arrange items that have been parented through the <see cref="Add(Transform)"/> and <see cref="Remove(Transform)"/> functions.
/// </summary>
public class ArrayArrangement : MonoBehaviour
{
    [SerializeField]
    Vector3 offset = Vector3.zero;

    [SerializeField]
    int height, width;

    [SerializeField]
    bool repositionAllWhenAdded = true;

    [SerializeField]
    bool repositionAllWhenRemoved = false;      // Note: Genearlly meaningless if it's only being used to orgnise crates added to the collector

    List<Transform> children;
    Vector3 nextPosition;

    private void Awake()
    {
        children = new List<Transform>();
        nextPosition = Vector3.zero;
        for (int i = 0; i < transform.childCount; i++)
        {
            children.Add(transform.GetChild(i));
        }
    }

    private void Start() => RepositionChildren();

    /// <summary>
    /// Add and parent an item to the arrangement.
    /// </summary>
    public void Add(Transform item)
    {
        if (children.Contains(item)) return;

        children.Add(item);
        item.SetParent(transform, true);

        if (repositionAllWhenAdded) 
            RepositionChildren(); 
        else 
            RepositionItem(item);

    }

    /// <summary>
    /// Remove and unparent an item from the arrangement.
    /// </summary>
    public void Remove(Transform item) 
    {
        if (!children.Contains(item)) return;
        children.Remove(item);
        item.SetParent(null);
        if (repositionAllWhenRemoved) RepositionChildren();
    }

    // Arranges children in a 3D grid.
    private void RepositionChildren()
    {
        Vector3 relativePosition = Vector3.zero;
        foreach (Transform child in children)
        {
            child.localPosition = relativePosition + offset;
            if (relativePosition.x + 1 < width)
            {
                relativePosition.x += 1;
                continue;
            }
            relativePosition.x = 0;
            if (relativePosition.y + 1 < height)
            {
                relativePosition.y += 1;
                continue;
            }
            relativePosition.y = 0;
            relativePosition.z += 1;
        }
        nextPosition = relativePosition;
    }

    // Arranges a single item in the grid.
    void RepositionItem(Transform item)
    {
        if (!children.Contains(item)) return;

        item.localPosition = nextPosition;
        if (nextPosition.x + 1 < width)
        {
            nextPosition.x += 1;
        }
        else
        {
            nextPosition.x = 0;
            if (nextPosition.y + 1 < height)
            {
                nextPosition.y += 1;
            }
            else
            {
                nextPosition.z += 1;
                nextPosition.y = 0;
            }
        }
    }
}
