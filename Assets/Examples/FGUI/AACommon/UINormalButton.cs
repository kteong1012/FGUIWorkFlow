using FairyGUI;

namespace MyUnityProject
{
    public partial class UINormalButton : AUIWrap
    {
        public override string PackageName => "AACommon";
        public override string Url => "ui://wtx7q5i9km5b1";
        public override string Componentname => "UINormalButton";
        [FGUIComponentRoot]
        public GButton FGUIObj { get; set; }
        [FGUIObject]
        public Controller button { get; set; }
        [FGUIObject]
        public GTextField title { get; set; }

        protected override void AddButtonCallbacks()
        {
        }

    }
}
