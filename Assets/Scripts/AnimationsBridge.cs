using UnityEngine;

public class AnimationsBridge : MonoBehaviour
{
    private Pop pop;

    private void Awake()
    {
        pop = GetComponentInParent<Pop>();
    }
    public void DestroyIt()
    {
        pop.DestroyPop();
    }
}
