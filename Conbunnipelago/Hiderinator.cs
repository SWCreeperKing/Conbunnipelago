using JetBrains.Annotations;
using UnityEngine;

namespace Conbunnipelago;

public class Hiderinator : MonoBehaviour
{
    /*
     ahhh~ perry the platypus, you are just in time to witness my latest invention-
     THE HIDERINATOR
     it will make whatever it touches, invisible or visible depending on conditions set
     those annoying objects that won't hide/unhide when you tell it to? just zap them with the Hiderinator and bam! full control
     */
    public Func<bool> Condition;

    [CanBeNull] public GameObject[] Children;

    private void Update()
    {
        if (Children is null) Children = gameObject.GetChildren();

        var condition = Condition();
        foreach (var child in Children)
        {
            child.SetActive(condition);
        }
    }
}