using System;
using System.Collections.Generic;
using UnityEngine;

public class ColourChanger : MonoBehaviour
{
    [Serializable]
    struct TextureInfo
    {
        public Texture player, forkliftLeft, forkliftRight;
    }

    [SerializeField] List<TextureInfo> playerMaterials;
    [SerializeField] Renderer bodyRenderer, forkliftLeftRenderer, forkliftRightRenderer;

    public void setPlayerMaterial(int num)
    {
        bodyRenderer.material.SetTexture("_BaseMap", playerMaterials[num].player);
        forkliftLeftRenderer.material.SetTexture("_BaseMap", playerMaterials[num].forkliftLeft);
        forkliftRightRenderer.material.SetTexture("_BaseMap", playerMaterials[num].forkliftRight);
    }
}
