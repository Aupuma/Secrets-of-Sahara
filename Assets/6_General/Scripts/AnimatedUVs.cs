using UnityEngine;
using System.Collections;

public class AnimatedUVs : MonoBehaviour
{
    public Vector2 uvAnimationRate = new Vector2(1.0f, 0.0f);

    Vector2 uvOffset = Vector2.zero;
    MeshRenderer rendr;

    private void Start()
    {
        rendr.GetComponent<MeshRenderer>();
    }

    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * Time.deltaTime);
        if (rendr.enabled)
        {
            rendr.material.mainTextureOffset = uvOffset;
        }
    }
}