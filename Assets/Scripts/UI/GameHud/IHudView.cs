using UnityEngine.UI;

namespace UI.GameHud
{
    public interface IHudView :  IView
    {
        Button StartFightButton  { get; }
        Button RespawnButton { get; }
    }
}