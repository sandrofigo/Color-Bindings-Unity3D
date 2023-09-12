using UnityEditor;
using UnityEngine;

namespace ColorBindings.Editor
{
    [CustomEditor(typeof(BindableColor))]
    public class BindableColorEditor : UnityEditor.Editor
    {
        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            var bindableColor = (BindableColor)target;

            if (bindableColor == null)
                return null;

            var tex = new Texture2D(width, height);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tex.SetPixel(x, y, bindableColor.Value);
                }
            }

            tex.Apply();

            return tex;
        }
    }
}