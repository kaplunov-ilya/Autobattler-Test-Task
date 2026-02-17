using UnityEngine;
using UnityEngine.UI;

namespace UI.GameHud
{
    public sealed class HudView : BaseView, IHudView
    {
        [field: SerializeField] public Button StartFightButton { get; private set; }
        [field: SerializeField] public Button RespawnButton { get; private set; }
    }
}