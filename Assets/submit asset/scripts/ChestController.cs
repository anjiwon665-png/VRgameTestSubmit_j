using UnityEngine;

public class ChestController : MonoBehaviour
{
    public Animation anim;

    public void OpenChest()
    {
        anim.Play("Open");
    }
}