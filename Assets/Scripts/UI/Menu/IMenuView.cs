using UnityEngine.UI;

namespace UI.Menu
{
    public interface IMenuView : IView
    {
        Button StartButton  { get; }
        Button ExitButton { get; }
    }
}