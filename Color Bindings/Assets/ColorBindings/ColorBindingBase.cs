using UnityEngine;

namespace ColorBindings
{
    public abstract class ColorBindingBase : MonoBehaviour
    {
        private void Awake()
        {
            if (Initialize())
                SetColor();
        }

        protected abstract bool Initialize();

        protected abstract void SetColor();

#if UNITY_EDITOR
        /// <summary>
        /// Used for internal editor tools.
        /// </summary>
        protected abstract void ClearColorInternal();

        /// <summary>
        /// Used for internal editor tools.
        /// </summary>
        public void ClearColor()
        {
            if (Initialize())
                ClearColorInternal();
        }
#endif

        [ContextMenu("Update Color")]
        public void UpdateColor()
        {
            if (Initialize())
                SetColor();
        }
    }
}