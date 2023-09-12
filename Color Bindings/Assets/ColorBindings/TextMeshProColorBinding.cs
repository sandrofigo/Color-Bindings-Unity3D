using TMPro;
using UnityEditor;
using UnityEngine;

namespace ColorBindings
{
    public class TextMeshProColorBinding : ColorBindingBase
    {
        [SerializeField]
        private BindableColor color;

        private TextMeshProUGUI text;

        protected override bool Initialize()
        {
            text = GetComponent<TextMeshProUGUI>();

            if (text == null)
            {
                Debug.LogError($"No {nameof(TextMeshProUGUI)} component found on '{gameObject.name}'!", gameObject);
                return false;
            }

            return true;
        }

        protected override void SetColor()
        {
#if UNITY_EDITOR
            Undo.RecordObject(text, nameof(TextMeshProUGUI));
            PrefabUtility.RecordPrefabInstancePropertyModifications(text);
#endif

            text.color = color.Value;
        }

#if UNITY_EDITOR
        protected override void ClearColorInternal()
        {
            text.color = Color.clear;
        }
#endif
    }
}