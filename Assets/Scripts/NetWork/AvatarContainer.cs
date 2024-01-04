using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarContainer : MonoBehaviour, IEnumerable<AvatarContainerChild>
{
    private List<AvatarContainerChild> avatarList = new List<AvatarContainerChild>();

    public AvatarContainerChild this[int index] => avatarList[index];
    public int Count => avatarList.Count;

    private void OnTransformChildrenChanged()
    {
        avatarList.Clear();
        foreach (Transform child in transform)
        {
            avatarList.Add(child.GetComponent<AvatarContainerChild>());
        }
    }

    public IEnumerator<AvatarContainerChild> GetEnumerator()
    {
        return avatarList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}