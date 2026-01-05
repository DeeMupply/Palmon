using UnityEngine;
using UnityEngine.Rendering;

public partial class Player
{
    [Header("Materials")]
    [SerializeField] private Material opaqueMaterial;
    [SerializeField] private Material transparentMaterial;
    
    [Header("Invisibility")]
    [Range(0f, 1f)]
    [SerializeField] private float invisibleAlpha = 0.7f;
    [SerializeField] private SkinnedMeshRenderer smr;
    MaterialPropertyBlock mpb;

    private bool isInvisible = false;
    public bool IsInvisible => isInvisible;

    private void InitializeInvisibility()
    {
        mpb = new MaterialPropertyBlock();
    }
    
    public void SetInvisibleState(bool invisible)
    {
        isInvisible = invisible;
        if (invisible)
        {
            // swap ONCE
            smr.sharedMaterial = transparentMaterial;

            // disable shadows
            smr.shadowCastingMode = ShadowCastingMode.Off;

            // apply alpha
            mpb.Clear();
            mpb.SetColor("_BaseColor", new Color(1f, 1f, 1f, invisibleAlpha));
            smr.SetPropertyBlock(mpb);
        }
        else
        {
            // clear per-instance overrides
            mpb.Clear();
            smr.SetPropertyBlock(mpb);

            // swap back
            smr.sharedMaterial = opaqueMaterial;

            // restore shadows
            smr.shadowCastingMode = ShadowCastingMode.On;
        }
    }
}