
namespace HHT.Resources.Model
{
    // 積込検品情報保持ファイル
    // mb_<<Handy:serialId>>.txt
    public class MbFile
    {
        public string tokuisaki_cd { get; set; }
        public string todokesaki_cd { get; set; }
        public string mail_key_kbn { get; set; }
        public string kanri_no { get; set; }
        public string tokuisaki_rk { get; set; }
        public string default_vendor { get; set; }
        public string vendor_nm { get; set; }
    }
}