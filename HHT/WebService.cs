
using HHT.Resources.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HHT
{
    public class WebService
    {
        public readonly static string WEB_SERVICE_URL = "http://192.168.0.18:8787/";

        public class LOGIN {
            public string LOGIN001 = WEB_SERVICE_URL + "ReqeustLogin001?hht_id={0}";
            public readonly static string LOGIN010 = WEB_SERVICE_URL + "ReqeustLogin010?souko_cd={0}";
            public readonly static string LOGIN020 = WEB_SERVICE_URL + "ReqeustLogin020?hht_id={0}";
            public readonly static string LOGIN030 = WEB_SERVICE_URL + "ReqeustLogin030?driver_cd={0}";
            public readonly static string LOGIN040 = WEB_SERVICE_URL + "ReqeustLogin040?driver_cd={0}&souko_cd={1}&htt_id={2}";
            public readonly static string LOGIN050 = WEB_SERVICE_URL + "ReqeustLogin050";
        };

        public class KOSU
        {
            public readonly static string KOSU010 = WEB_SERVICE_URL + "ReqeustKosu010?hht_id={0}";
            public readonly static string KOSU020 = WEB_SERVICE_URL + "ReqeustLogin010?souko_cd={0}";
            public readonly static string KOSU030 = WEB_SERVICE_URL + "ReqeustLogin020?hht_id={0}";
            public readonly static string KOSU040 = WEB_SERVICE_URL + "ReqeustLogin030?driver_cd={0}";
            public readonly static string KOSU050 = WEB_SERVICE_URL + "ReqeustLogin040?driver_cd={0}&souko_cd={1}&htt_id={2}";
            public readonly static string KOSU060 = WEB_SERVICE_URL + "ReqeustLogin050";
            public readonly static string KOSU065 = WEB_SERVICE_URL + "ReqeustLogin050";
        };

        public class IDOU
        {
            public readonly static string IDOU010 = WEB_SERVICE_URL + "ReqeustIdou010?kenpin_souko={0}&kitaku_cd={1}&kamotsu_no={2}";
            public readonly static string KOSU020 = WEB_SERVICE_URL + "ReqeustLogin010?souko_cd={0}";
            public readonly static string KOSU030 = WEB_SERVICE_URL + "ReqeustLogin020?hht_id={0}";
            public readonly static string KOSU040 = WEB_SERVICE_URL + "ReqeustLogin030?driver_cd={0}";
            public readonly static string KOSU050 = WEB_SERVICE_URL + "ReqeustLogin040?driver_cd={0}&souko_cd={1}&htt_id={2}";
            public readonly static string KOSU060 = WEB_SERVICE_URL + "ReqeustLogin050";
            public readonly static string KOSU065 = WEB_SERVICE_URL + "ReqeustLogin050";
        };

        public class TUMIKOMI
        {
            public readonly static string TUMIKOMI010 = WEB_SERVICE_URL + "ReqeustTumikomi010"; // 便検索 
            public readonly static string TUMIKOMI020 = WEB_SERVICE_URL + "ReqeustTumikomi020"; // 該当コース内の店舗一覧取得
            public readonly static string TUMIKOMI030 = WEB_SERVICE_URL + "ReqeustTumikomi030"; // 該当店舗が何コース分あるか取得(1件であれば定番コース。2件以上あれば増便コース)
            public readonly static string TUMIKOMI040 = WEB_SERVICE_URL + "ReqeustTumikomi040"; // 該当店舗の各マテハン数を取得(定番コース)
            public readonly static string TUMIKOMI300 = WEB_SERVICE_URL + "ReqeustTumikomi300"; // 該当店舗の各マテハン数を取得(定番コース)
            public readonly static string KOSU050 = WEB_SERVICE_URL + "ReqeustLogin040?driver_cd={0}&souko_cd={1}&htt_id={2}";
            public readonly static string KOSU060 = WEB_SERVICE_URL + "ReqeustLogin050";
            public readonly static string KOSU065 = WEB_SERVICE_URL + "ReqeustLogin050";
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
            string url = "http://192.168.0.18:8787/login/GetSoukoName";
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

        public static KOSU131 ExecuteKosu131(string soukoCd, string kitaku_cd, string syukaDate, string vendorCd)
        {
            string url = string.Format(@"http://192.168.0.33:8787/GetSoukoName?souko_cd={0}", soukoCd);

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "souko_cd", soukoCd }
            };

            //string url = string.Format(@WEB_SERVICE_URL + LOGIN010, soukoCd);
            //string resultData = await CommonUtils.PostAsync(url, param);
            string resultData = CommonUtils.GetJsonData(url);
            resultData =
            "{" +
                "state: '00'," +
                "vendor_nm : 'アルフレッサヘルスケア（パピコム）'," +
             "},"
             ;
            
            KOSU131 resultDataSet = JsonConvert.DeserializeObject<KOSU131>(resultData);
            return resultDataSet;
        }

        internal static bool ExecuteKosu020(string text)
        {
            return true;
        }

        // 得意先コード検索
        internal static bool ExecuteKosu010(string text)
        {
            return true;
        }

        /*
        public async Task<string> GetTodokesakiName(string soukoCd)
        {
            string url = string.Format(@WEB_SERVICE_ADDRESS + KOSU020, soukoCd);
            string resultData = await CommonUtils.GetJsonDataAsync(url);
            Premises premises = JsonConvert.DeserializeObject<Premises>(resultData);
            return premises.souko_nm;
        }
        */

        internal static void ReqeustTUMIKOMI230(Dictionary<string, string> param)
        {
            
        }

    }

}