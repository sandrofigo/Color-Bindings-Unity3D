using UnityEngine;

namespace ColorBindings
{
    [CreateAssetMenu(menuName = "UI/Bindable Color")]
    public class BindableColor : ScriptableObject
    {
        [SerializeField]
        private Color color;

        public Color Value => color;
    }
}