using FairyGUI;

namespace MyUnityProject
{
    public partial class ComScrollableText_000000 : AUIWrap
    {
        public override string PackageName => "AACommon";
        public override string Url => "ui://wtx7q5i9lveb2";
        public override string Componentname => "ComScrollableText_000000";
        [FGUIComponentRoot]
        public GLabel FGUIObj { get; set; }
        [FGUIObject]
        public GTextField title { get; set; }

        protected override void AddButtonCallbacks()
        {
        }

    }
}
