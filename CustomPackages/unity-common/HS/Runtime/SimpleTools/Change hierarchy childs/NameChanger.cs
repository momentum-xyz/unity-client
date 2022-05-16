using UnityEngine;

public class NameChanger : MonoBehaviour
{
    public string contains;
    public string ChildName;

    //To clear the Hierarchy you will need clean names.
    [ContextMenu("ChangeName")]
    void ChangeName()
    {
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).name.Contains(contains))
                transform.GetChild(i).name = ChildName;
    }
}
