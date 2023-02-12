using FairyGUI;

namespace MyUnityProject
{
    public partial class UIResUpdatePanel : AUIWrap
    {
        public override string PackageName => "UIResUpdatePanel";
        public override string Url => "ui://qsw3ypu9lveb0";
        public override string Componentname => "UIResUpdatePanel";
        [FGUIComponentRoot]
        public GComponent FGUIObj { get; set; }
        [FGUIObject]
        public GLoader loaderBg { get; set; }
        [FGUIObject]
        public GProgressBar progressBar { get; set; }
        [FGUIObject]
        public GTextField txtTask { get; set; }
        [FGUIObject]
        public GTextField txtProgress { get; set; }

        protected override void AddButtonCallbacks()
        {
        }

    }
}
