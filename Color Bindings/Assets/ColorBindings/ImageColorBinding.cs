using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBindings
{
    public class ImageColorBinding : ColorBindingBase
    {
        [SerializeField]
        private BindableColor color;

        private Image image;

        protected override bool Initialize()
        {
            image = GetComponent<Image>();

            if (image == null)
            {
                Debug.LogError($"No {nameof(Image)} component found on '{gameObject.name}'", gameObject);
                return false;
            }

            return true;
        }

        protected override void SetColor()
        {
#if UNITY_EDITOR
            Undo.RecordObject(image, nameof(Image));
            PrefabUtility.RecordPrefabInstancePropertyModifications(image);
#endif

            image.color = color.Value;
        }

#if UNITY_EDITOR
        protected override void ClearColorInternal()
        {
            image.color = Color.clear;
        }
#endif
    }
}