using UnityEngine;

public class MurcielagoAnim : MonoBehaviour
{
    void Start()
    {
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.Play("volando");
        }
        else
        {
            Debug.LogWarning("No hay Animator en " + gameObject.name);
        }
    }
}
