using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace ColorBindings
{
    public class ButtonColorBinding : ColorBindingBase
    {
        [SerializeField]
        private BindableColor background;

        [SerializeField]
        private bool overrideBackgroundAlpha;

        [SerializeField]
        [AlphaOverride(nameof(overrideBackgroundAlpha))]
        private float backgroundAlpha = 1f;

        [Header("States")]
        [SerializeField]
        private BindableColor normal;

        [SerializeField]
        private bool overrideNormalAlpha;

        [SerializeField]
        [AlphaOverride(nameof(overrideNormalAlpha))]
        private float normalAlpha = 1f;

        [SerializeField]
        private BindableColor highlighted;

        [SerializeField]
        private bool overrideHighlightedAlpha;

        [SerializeField]
        [AlphaOverride(nameof(overrideHighlightedAlpha))]
        private float highlightedAlpha = 1f;

        [SerializeField]
        private BindableColor pressed;

        [SerializeField]
        private bool overridePressedAlpha;

        [SerializeField]
        [AlphaOverride(nameof(overridePressedAlpha))]
        private float pressedAlpha = 1f;

        [SerializeField]
        private BindableColor selected;

        [SerializeField]
        private bool overrideSelectedAlpha;

        [SerializeField]
        [AlphaOverride(nameof(overrideSelectedAlpha))]
        private float selectedAlpha = 1f;

        [SerializeField]
        private BindableColor disabled;

        [SerializeField]
        private bool overrideDisabledAlpha;

        [SerializeField]
        [AlphaOverride(nameof(overrideDisabledAlpha))]
        private float disabledAlpha = 1f;

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
            buttonColors.normalColor = GetColor(normal, overrideNormalAlpha, normalAlpha);
            buttonColors.highlightedColor = GetColor(highlighted, overrideHighlightedAlpha, highlightedAlpha);
            buttonColors.pressedColor = GetColor(pressed, overridePressedAlpha, pressedAlpha);
            buttonColors.selectedColor = GetColor(selected, overrideSelectedAlpha, selectedAlpha);
            buttonColors.disabledColor = GetColor(disabled, overrideDisabledAlpha, disabledAlpha);
            button.colors = buttonColors;

            button.targetGraphic.color = GetColor(background, overrideBackgroundAlpha, backgroundAlpha);
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
