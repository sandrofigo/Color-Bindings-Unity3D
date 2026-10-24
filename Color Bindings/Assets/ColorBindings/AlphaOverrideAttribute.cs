using UnityEngine;

namespace ColorBindings
{
    /// <summary>
    /// Draws a float field as an alpha slider, which is only visible when the given bool field is enabled.
    /// </summary>
    public class AlphaOverrideAttribute : PropertyAttribute
    {
        public string ToggleField { get; }

        public AlphaOverrideAttribute(string toggleField)
        {
            ToggleField = toggleField;
        }
    }
}
