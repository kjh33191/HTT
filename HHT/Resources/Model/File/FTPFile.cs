
namespace HHT.Resources.Model
{
    // 積込検品情報保持ファイル
    // ftp_<<Handy:serialId>>.txt
    public class FtpFile
    {
        public string ftp_ip { get; set; }
        public string ftp_user { get; set; }
        public string ftp_pass { get; set; }
        public string tel_number { get; set; }
        public string tel_user { get; set; }
        public string tel_pass { get; set; }
        public string ap_ssid { get; set; }
        public string ap_wpatype { get; set; }
        public string ap_cipher { get; set; }
        public string ap_sec_type { get; set; }
        public string ap_sec_key { get; set; }
        public string ap_wepkey { get; set; }
        public string wifi_ssid { get; set; }
        public string wifi_wpatype { get; set; }
        public string wifi_cipher { get; set; }
        public string wifi_sec_type { get; set; }
        public string wifi_sec_key { get; set; }
        public string wifi_wepkey { get; set; }
        public string bd_address { get; set; }
    }
}