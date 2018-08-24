
namespace HHT.Resources.Model
{
    // 積込検品情報保持ファイル
    // tokui_<<Handy:serialId>>.txt
    public class TokuiFile
    {
        public string tokuisaki_cd { get; set; }
        public string todokesaki_cd { get; set; }
        public string tokuisaki_nm { get; set; }
        public string tokuisaki_rk { get; set; }
        public string default_vendor { get; set; }

    }
}