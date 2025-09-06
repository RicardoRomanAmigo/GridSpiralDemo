using UnityEngine;

public class Pop : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    public void DestroyPop()
    {
        Destroy(gameObject);
    }
}
