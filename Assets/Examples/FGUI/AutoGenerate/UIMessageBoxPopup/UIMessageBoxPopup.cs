using FairyGUI;

namespace MyUnityProject
{
    public partial class UIMessageBoxPopup : AUIWrap
    {
        public override string PackageName => "UIMessageBoxPopup";
        public override string Url => "ui://qhlqzoiqlveb0";
        public override string Componentname => "UIMessageBoxPopup";
        [FGUIComponentRoot]
        public GComponent FGUIObj { get; set; }
        [FGUIObject]
        public Controller mode { get; set; }
        [FGUIObject]
        public GTextField txtTitle { get; set; }
        [FGUIObject]
        public GLabel txtContent { get; set; }
        [FGUIObject]
        public GButton btnConfirm { get; set; }
        [FGUIObject]
        public GButton btnCancel { get; set; }

        protected override void AddButtonCallbacks()
        {
            SetButtonEvent(btnConfirm,() => OnClick_btnConfirm());
            SetButtonEvent(btnCancel,() => OnClick_btnCancel());
        }

        partial void OnClick_btnConfirm();
        partial void OnClick_btnCancel();
    }
}
