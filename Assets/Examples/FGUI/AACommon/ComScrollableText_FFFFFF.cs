using FairyGUI;

namespace MyUnityProject
{
    public partial class ComScrollableText_FFFFFF : AUIWrap
    {
        public override string PackageName => "AACommon";
        public override string Url => "ui://wtx7q5i9lveb3";
        public override string Componentname => "ComScrollableText_FFFFFF";
        [FGUIComponentRoot]
        public GLabel FGUIObj { get; set; }
        [FGUIObject]
        public GTextField title { get; set; }

        protected override void AddButtonCallbacks()
        {
        }

    }
}
