using UnityEngine;
using UnityEngine.UI;

namespace UI.Menu
{
    public sealed class MenuView : BaseView, IMenuView
    {
        [field: SerializeField] public Button StartButton  { get; private set; }
        [field: SerializeField] public Button ExitButton { get; private set; }
    }
}