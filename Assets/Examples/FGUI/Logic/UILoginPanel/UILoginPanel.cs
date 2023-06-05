using FairyGUI;
using UnityEngine;

namespace MyUnityProject
{
    public partial class UILoginPanel
    {
        protected override void OnInit() { }
        protected override void OnShow() { }
        protected override void OnHide() { }
        protected override void OnDestroy() { }

        partial void OnClick_btnLogin()
        {
            Debug.Log("123123");
        }
    }
}
