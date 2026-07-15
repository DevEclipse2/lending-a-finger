using UnityEngine;

public class pop : MonoBehaviour
{
    [SerializeField]
    ParticleSystem splatter;
   public void doPop()
    {
        splatter.Emit(16);

    }
}
