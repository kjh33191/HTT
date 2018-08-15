
using HHT.Resources.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HHT
{
    public class WebService
    {
        public readonly static string WEB_SERVICE_URL = "http://192.168.0.19:8787/";
        
        public class LOGIN {
            readonly string LOGIN001 = WEB_SERVICE_URL + "login/RequestLogin001";
            readonly static string LOGIN010 = WEB_SERVICE_URL + "login/RequestLogin010";
            public readonly static string LOGIN020 = WEB_SERVICE_URL + "login/RequestLogin020";
            public readonly static string LOGIN030 = WEB_SERVICE_URL + "login/RequestLogin030";
            public readonly static string LOGIN040 = WEB_SERVICE_URL + "login/RequestLogin040";
            public readonly static string LOGIN050 = WEB_SERVICE_URL + "login/RequestLogin050";
        };

        public class KOSU
        {
            public readonly static string KOSU010 = WEB_SERVICE_URL + "RequestKosu010";
            public readonly static string KOSU020 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu020";
            public readonly static string KOSU030 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu030";
            public readonly static string KOSU040 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu040";
            public readonly static string KOSU050 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu050";
            public readonly static string KOSU060 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu060";
            public readonly static string KOSU065 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu065";
            public readonly static string KOSU070 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu070";
            public readonly static string KOSU131 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu131";
            public readonly static string KOSU150 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu150";
            public readonly static string KOSU210 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu210";
        };

        public class IDOU
        {
            public readonly static string IDOU010 = WEB_SERVICE_URL + "RequestIdou010?kenpin_souko={0}&kitaku_cd={1}&kamotsu_no={2}";
            public readonly static string KOSU020 = WEB_SERVICE_URL + "RequestLogin010?souko_cd={0}";
            public readonly static string KOSU030 = WEB_SERVICE_URL + "RequestLogin020?hht_id={0}";
            public readonly static string KOSU040 = WEB_SERVICE_URL + "RequestLogin030?driver_cd={0}";
            public readonly static string KOSU050 = WEB_SERVICE_URL + "RequestLogin040?driver_cd={0}&souko_cd={1}&htt_id={2}";
            public readonly static string KOSU060 = WEB_SERVICE_URL + "RequestLogin050";
            public readonly static string KOSU065 = WEB_SERVICE_URL + "RequestLogin050";
            
        };

        public class TUMIKOMI
        {
            public readonly static string TUMIKOMI010 = WEB_SERVICE_URL + "RequestTumikomi010"; // 便検索 
            public readonly static string TUMIKOMI020 = WEB_SERVICE_URL + "RequestTumikomi020"; // 該当コース内の店舗一覧取得
            public readonly static string TUMIKOMI030 = WEB_SERVICE_URL + "RequestTumikomi030"; // 該当店舗が何コース分あるか取得(1件であれば定番コース。2件以上あれば増便コース)
            public readonly static string TUMIKOMI040 = WEB_SERVICE_URL + "RequestTumikomi040"; // 該当店舗の各マテハン数を取得(定番コース)
            public readonly static string TUMIKOMI050 = WEB_SERVICE_URL + "RequestTumikomi050"; // 該当店舗の各マテハン数を取得(定番コース)
            public readonly static string TUMIKOMI060 = WEB_SERVICE_URL + "RequestTumikomi060"; // 積込検品用Proc sagyou5
            public readonly static string TUMIKOMI070 = WEB_SERVICE_URL + "RequestTumikomi070"; // 貨物Noスキャン時、各分類のカウントを取得
            public readonly static string TUMIKOMI080 = WEB_SERVICE_URL + "RequestTumikomi080"; // 積込検品用Proc sagyou7
            public readonly static string TUMIKOMI090 = WEB_SERVICE_URL + "RequestTumikomi090"; // Back

            public readonly static string TUMIKOMI210 = WEB_SERVICE_URL + "RequestTumikomi210"; // 積込検品用Proc(配車テーブル実績数更新) sagyou5

            public readonly static string TUMIKOMI300 = WEB_SERVICE_URL + "RequestTumikomi300"; // 該当店舗の各マテハン数を取得(定番コース)
            public readonly static string TUMIKOMI312 = WEB_SERVICE_URL + "RequestTumikomi312"; // Back
            public readonly static string TUMIKOMI314 = WEB_SERVICE_URL + "RequestTumikomi314"; // 積込検品用Proc(配車テーブル実績数更新) sagyou7
            public readonly static string KOSU050 = WEB_SERVICE_URL + "RequestLogin040?driver_cd={0}&souko_cd={1}&htt_id={2}";
            public readonly static string KOSU060 = WEB_SERVICE_URL + "RequestLogin050";
            public readonly static string KOSU065 = WEB_SERVICE_URL + "RequestLogin050";
        };



        private readonly static string LOGIN001 = "Login001?hht_id={0}"; // ハンディマスタデータを取得する。（新規）
        private readonly static string LOGIN010 = "Login010?souko_cd={0}"; // 倉庫マスタデータ取得
        private readonly static string LOGIN020 = "Login020?hht_id={0}"; // 無線管理マスタデータ取得
        private readonly static string LOGIN030 = "Login030?driver_cd={0}"; // 担当者名を取得
        private readonly static string LOGIN040 = "Login040?driver_cd={0}&souko_cd={1}&htt_id={2}"; // 無線管理テーブルへ情報を登録する。
        private readonly static string LOGIN050 = "Login050"; // 担当者マスタデータ取得。

        private readonly static string KOSU010 = "Kosu010?tokuisaki_cd={0}"; // 得意先コード入力
        private readonly static string KOSU020 = "Kosu020?tokuisaki_cd={0}&todokesaki_cd={1}"; // 得意先コード入力
        private readonly static string KOSU030 = "Kosu030?todokesaki_cd={0}"; // 得意先検索
        private readonly static string KOSU040 = "Kosu040?kenpin_souko={0}&kitaku_cd={1}&syuka_date={2}&bin_no={3}&tokuisaki_cd={4}&todokesaki_cd={5}"; // 届先確認
        private readonly static string KOSU050 = "Kosu050?tokuisaki_cd={0}&todokesaki_cd={1}"; // 届先確認・届先名、デフォルトベンダー取得
        private readonly static string KOSU060 = "Kosu060?kenpin_souko={0}&kitaku_cd={1}&syuka_date={2}&bin_no={3}"; // 届先表示
        private readonly static string KOSU065 = "Kosu065?kenpin_souko={0}&kitaku_cd={1}&syuka_date={2}&vendor_cd={3}"; // 届先表示（ベンダー検品用）

        private readonly static string KOSU070_PARAMETERS = "?pTerminalID={0}&pProgramID={1}&pSagyosyaCD={2}&pSoukoCD={3}"
            + "&pSyukaDate={4}&tokuisaki_cd={5}&todokesaki_cd={6}&pVendorCD={7}&pTsumiVendorCD={8}&pKamotsuNo={9}"
            + "&pBinNo={10}&pHHT_No={11}&pMatehan={12}&pJskCaseSu={13}&pJskOriconSu={14}&pJskFuteikeiSu={15}"
            + "&pJskTcSu={16}&pJskMailbinSu={17}&pJskHazaiSu={18}&pJskIdoSu={19}&pJskHenpinSu={20}&pJskHansokuSu={21}";

        private readonly static string KOSU070 = "Kosu070"; //紐付作業・届先指定検品：バーコード proc_hht_kosukenpin
        private readonly static string KOSU075 = "Kosu075"; // 紐付作業・QR用
        private readonly static string KOSU076 = "Kosu076"; // 紐付作業(ベンダー指定)・QR用
        private readonly static string KOSU077 = "Kosu077"; // 紐付作業カテゴリ定特混在チェック・QR用 
        private readonly static string KOSU078 = "Kosu078"; // 紐付作業カテゴリ定特混在チェック(ベンダー指定)・QR用
        private readonly static string KOSU080 = "Kosu080"; // 満タン処理
        private readonly static string KOSU085 = "Kosu085"; // 取消処理

        private readonly static string KOSU090 = "Kosu090?souko_cd={0}&course={1}"; // コース入力 
        private readonly static string KOSU095 = "Kosu095?kenpin_souko={0}&kitaku_cd={1}&syuka_date={2}"; // ベンダー表示
        private readonly static string KOSU100 = "Kosu100?souko_cd={0}&kitaku_cd={1}&syuka_date={2}&nohin_date={3}&bin_no={4}&berth={5}"; // 届先表示・バース選択(未使用)
        
        private readonly static string KOSU110 = "Kosu110?kenpin_souko={0}&kitaku_cd={1}&syuka_date={2}&bin_no={3}&tokuisaki_cd={4}&todokesaki_cd={5}"; // 紐付確認ー紐づけ情報取得（届先）
        private readonly static string KOSU115 = "Kosu115?kenpin_souko={0}&kitaku_cd={1}&syuka_date={2}&vendor_cd={3}"; // 紐付完了
        private readonly static string KOSU120 = ""; 
        private readonly static string KOSU125 = ""; // 紐付完了
        private readonly static string KOSU130 = ""; // 紐付完了
        private readonly static string KOSU131 = ""; // 紐付完了
        private readonly static string KOSU132 = ""; // 紐付完了
        private readonly static string KOSU140 = ""; // 紐付完了
        private readonly static string KOSU150 = ""; // 紐付完了
        private readonly static string KOSU160 = ""; // 紐付完了
        private readonly static string KOSU165 = ""; // 紐付完了
        private readonly static string KOSU170 = ""; // 紐付完了
        private readonly static string KOSU180 = ""; // 紐付完了
        private readonly static string KOSU185 = ""; // 紐付完了
        private readonly static string KOSU190 = ""; // 紐付完了
        private readonly static string KOSU200 = "Kosu200?vendor_cd={0}"; // 紐付完了
        private readonly static string KOSU210 = ""; // 紐付完了
        private readonly static string KOSU220 = ""; // 紐付完了
        private readonly static string KOSU230 = ""; // 紐付完了
        private readonly static string KOSU240 = ""; // 紐付完了

        public static async Task<LOGIN010> ExecuteLogin010(Dictionary<string, string> param)
        {
            string url = "http://192.168.0.19:8787/login/RequestLogin010";
            string resultData = await CommonUtils.PostAsync(url, param);
            LOGIN010 login010 = JsonConvert.DeserializeObject<LOGIN010>(resultData);

            return login010;
        }

        public static LOGIN020 ExecuteLogin020(Dictionary<string, string> param)
        {
            string url = WEB_SERVICE_URL + LOGIN020;
            string resultData = CommonUtils.Post(url, param);
            LOGIN020 login020 = JsonConvert.DeserializeObject<LOGIN020>(resultData);

            return login020;
        }

        public static LOGIN030 ExecuteLogin030(Dictionary<string, string> param)
        {
            string url = WEB_SERVICE_URL + LOGIN030;
            string resultData = CommonUtils.Post(url, param);
            LOGIN030 login030 = JsonConvert.DeserializeObject<LOGIN030>(resultData);

            return login030;
        }

        public static List<KOSU200> ExecuteKosu200(string vendorCode)
        {
            string url = string.Format(@WEB_SERVICE_URL + KOSU200, vendorCode);

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "vendor_cd", vendorCode }
            };

            // string resultData = await CommonUtils.PostAsync(url, param);

            string resultData =
            "[" +
            "{" +
                "matehan: '01'," +
                "matehan_nm : 'キャリー'," +
             "}," +
             "{" +
                "matehan: '02'," +
                "matehan_nm : 'カゴ車'," +
             "}," +
            "{" +
                "matehan: '03'," +
                "matehan_nm : 'カート'," +
             "}," +
             "]"
             ;

            List<KOSU200> resultDataSet = JsonConvert.DeserializeObject<List<KOSU200>>(resultData);
            return resultDataSet;
        }

    }

}