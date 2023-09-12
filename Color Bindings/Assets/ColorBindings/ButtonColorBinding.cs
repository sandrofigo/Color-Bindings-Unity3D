using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBindings
{
    public class ButtonColorBinding : ColorBindingBase
    {
        [SerializeField]
        private BindableColor background;

        [Header("States")]
        [SerializeField]
        private BindableColor normal;

        [SerializeField]
        private BindableColor highlighted;

        [SerializeField]
        private BindableColor pressed;

        [SerializeField]
        private BindableColor selected;

        [SerializeField]
        private BindableColor disabled;

        private Button button;

        protected override bool Initialize()
        {
            button = GetComponent<Button>();

            if (button == null)
            {
                Debug.LogError($"No {nameof(Button)} component found on '{gameObject.name}'!", gameObject);
                return false;
            }

            return true;
        }

        protected override void SetColor()
        {
#if UNITY_EDITOR
            Undo.RecordObject(button, nameof(Button));
            Undo.RecordObject(button.targetGraphic, nameof(Button));
            PrefabUtility.RecordPrefabInstancePropertyModifications(button);
            PrefabUtility.RecordPrefabInstancePropertyModifications(button.targetGraphic);
#endif

            ColorBlock buttonColors = button.colors;
            buttonColors.normalColor = normal.Value;
            buttonColors.highlightedColor = highlighted.Value;
            buttonColors.pressedColor = pressed.Value;
            buttonColors.selectedColor = selected.Value;
            buttonColors.disabledColor = disabled.Value;
            button.colors = buttonColors;

            button.targetGraphic.color = background.Value;
        }

#if UNITY_EDITOR
        protected override void ClearColorInternal()
        {
            ColorBlock buttonColors = button.colors;
            buttonColors.normalColor = Color.clear;
            buttonColors.highlightedColor = Color.clear;
            buttonColors.pressedColor = Color.clear;
            buttonColors.selectedColor = Color.clear;
            buttonColors.disabledColor = Color.clear;
            button.colors = buttonColors;

            button.targetGraphic.color = Color.clear;
        }
#endif
    }
}