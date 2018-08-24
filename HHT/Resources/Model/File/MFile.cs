
namespace HHT.Resources.Model
{
    // 積込検品情報保持ファイル
    // m_<<Handy:serialId>>.txt
    public class MFile
    {
        public string kenpin_souko { get; set; }
        public string kitaku_cd { get; set; }
        public string syuka_date { get; set; }
        public string bin_no { get; set; }
        public string course { get; set; }
        public string driver_cd { get; set; }
        public string butsuryu_no { get; set; }
        public string nohin_yti_time { get; set; }
        public string tokuisaki_cd { get; set; }
        public string todokesaki_cd { get; set; }
        public string tokuisaki_rk { get; set; }
        public string vendor_cd { get; set; }
        public string vendor_nm { get; set; }
        public string default_vendor { get; set; }
        public string default_vendor_nm { get; set; }
        public string bunrui { get; set; }
        public string kamotsu_no { get; set; }
        public string matehan { get; set; }
        public string category { get; set; }
        public string category_nm { get; set; }
        public string state { get; set; }

    }
}