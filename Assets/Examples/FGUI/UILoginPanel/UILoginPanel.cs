using FairyGUI;

namespace MyUnityProject
{
    public partial class UILoginPanel : AUIWrap
    {
        public override string PackageName => "UILoginPanel";
        public override string Url => "ui://wotugxqgkm5b0";
        public override string Componentname => "UILoginPanel";
        [FGUIComponentRoot]
        public GComponent FGUIObj { get; set; }
        [FGUIObject]
        public GButton btnLogin { get; set; }
        [FGUIObject]
        public GTextInput inputAccount { get; set; }
        [FGUIObject]
        public GTextInput inputPassword { get; set; }
        [FGUIObject]
        public GTextField txtTips { get; set; }

        protected override void AddButtonCallbacks()
        {
        SetButtonEvent(btnLogin,() => OnClick_btnLogin());
        }

        partial void OnClick_btnLogin();
    }
}
