using UnityEngine;

namespace UI
{
    public class BaseView : MonoBehaviour, IView
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}