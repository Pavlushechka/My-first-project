using System.Collections.Generic;
using UnityEngine;

public class BuildGhost : MonoBehaviour
{
    public Color validColor = new Color(0f, 1f, 0f, 0.4f);
    public Color invalidColor = new Color(1f, 0f, 0f, 0.4f);

    private List<Renderer> renderers = new List<Renderer>();
    private List<Material> materials = new List<Material>();

    void Awake()
    {
        renderers.AddRange(GetComponentsInChildren<Renderer>());

        // Создаём копии материалов, чтобы не менять оригиналы
        foreach (var r in renderers)
        {
            var mats = new List<Material>();
            foreach (var m in r.sharedMaterials)
            {
                var copy = new Material(m);
                SetupTransparent(copy);
                mats.Add(copy);
            }
            r.materials = mats.ToArray();
            materials.AddRange(mats);
        }

        // Отключаем коллайдеры у призрака
        foreach (var c in GetComponentsInChildren<Collider>())
            c.enabled = false;
    }

    void SetupTransparent(Material mat)
    {
        // Настраиваем шейдер URP/Lit на прозрачность
        mat.SetFloat("_Surface", 1f);           // 1 = Transparent
        mat.SetFloat("_Blend", 0f);             // 0 = Alpha
        mat.SetFloat("_AlphaClip", 0f);
        mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetFloat("_ZWrite", 0f);
        mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        mat.EnableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
    }

    public void SetValid(bool valid)
    {
        Color color = valid ? validColor : invalidColor;

        foreach (var mat in materials)
        {
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color"))
                mat.SetColor("_Color", color);
        }
    }

    public void SetPositionAndRotation(Vector3 pos, Quaternion rot)
    {
        transform.position = pos;
        transform.rotation = rot;
    }
}