﻿using Android.OS;
using Android.Util;
using HHT.Resources.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace HHT
{
    public class WebService
    {
        public readonly static string TAG = "WebService";

        private static string HOST_ADDRESS = "192.168.0.13";
        private static string PORT = "8787";
        private static string WEB_SERVICE_URL = "http://" + HOST_ADDRESS + ":" + PORT + "/";

        public class LOGIN
        {
            public readonly static string LOGIN010 = "login/RequestLogin010";
            public readonly static string LOGIN020 = "login/RequestLogin020";
            public readonly static string LOGIN030 = "login/RequestLogin030";
            public readonly static string LOGIN040 = "login/RequestLogin040";
            public readonly static string LOGIN050 = "login/RequestLogin050";
        };

        public class KOSU
        {
            public readonly static string KOSU010 = "KosuKenpin/RequestKosu010";
            public readonly static string KOSU020 = "KosuKenpin/RequestKosu020";
            public readonly static string KOSU030 = "KosuKenpin/RequestKosu030";
            public readonly static string KOSU040 = "KosuKenpin/RequestKosu040";
            public readonly static string KOSU050 = "KosuKenpin/RequestKosu050";
            public readonly static string KOSU060 = "KosuKenpin/RequestKosu060";
            public readonly static string KOSU065 = "KosuKenpin/RequestKosu065";
            public readonly static string KOSU070 = "KosuKenpin/RequestKosu070";
            public readonly static string KOSU080 = "KosuKenpin/RequestKosu080";
            public readonly static string KOSU085 = "KosuKenpin/RequestKosu085";

            public readonly static string KOSU095 = "KosuKenpin/RequestKosu095";
            public readonly static string KOSU110 = "KosuKenpin/RequestKosu110";
            public readonly static string KOSU115 = "KosuKenpin/RequestKosu115";
            public readonly static string KOSU120 = "KosuKenpin/RequestKosu120";
            public readonly static string KOSU125 = "KosuKenpin/RequestKosu125";
            public readonly static string KOSU131 = "KosuKenpin/RequestKosu131";
            public readonly static string KOSU150 = "KosuKenpin/RequestKosu150";
            public readonly static string KOSU160 = "KosuKenpin/RequestKosu160";
            public readonly static string KOSU165 = "KosuKenpin/RequestKosu165";
            public readonly static string KOSU170 = "KosuKenpin/RequestKosu170";
            public readonly static string KOSU180 = "KosuKenpin/RequestKosu180";
            public readonly static string KOSU185 = "KosuKenpin/RequestKosu185";
            public readonly static string KOSU190 = "KosuKenpin/RequestKosu190";

            public readonly static string KOSU200 = "KosuKenpin/RequestKosu200";
            public readonly static string KOSU210 = "KosuKenpin/RequestKosu210";

            public readonly static string KOSU220 = "KosuKenpin/RequestKosu220";
            public readonly static string KOSU230 = "KosuKenpin/RequestKosu230";
            public readonly static string KOSU240 = "KosuKenpin/RequestKosu240";
            
        };

        public class IDOU
        {
            public readonly static string IDOU010 = "Tumikomi/RequestIDOU010";
            public readonly static string IDOU020 = "Tumikomi/RequestIDOU020";
            public readonly static string IDOU030 = "Tumikomi/RequestIDOU030";
            public readonly static string IDOU031 = "Tumikomi/RequestIDOU031";
            public readonly static string IDOU033 = "Tumikomi/RequestIDOU033";
            public readonly static string IDOU040 = "Tumikomi/RequestIDOU040";
            public readonly static string IDOU050 = "Tumikomi/RequestIDOU050";
            public readonly static string IDOU060 = "Tumikomi/RequestIDOU060";
            public readonly static string IDOU070 = "Tumikomi/RequestIDOU070";
            public readonly static string IDOU080 = "Tumikomi/RequestIDOU080";
            public readonly static string IDOU090 = "Tumikomi/RequestIDOU090";
        };

        public class TUMIKOMI
        {
            public readonly static string TUMIKOMI010 = "TumikomiKenpin/RequestTsumikomi010"; // 便検索 
            public readonly static string TUMIKOMI020 = "TumikomiKenpin/RequestTsumikomi020"; // 該当コース内の店舗一覧取得
            public readonly static string TUMIKOMI030 = "TumikomiKenpin/RequestTsumikomi030"; // 該当店舗が何コース分あるか取得(1件であれば定番コース。2件以上あれば増便コース)
            public readonly static string TUMIKOMI040 = "TumikomiKenpin/RequestTsumikomi040"; // 該当店舗の各マテハン数を取得(定番コース)
            public readonly static string TUMIKOMI050 = "TumikomiKenpin/RequestTsumikomi050"; // 該当店舗の各マテハン数を取得(定番コース)
            public readonly static string TUMIKOMI060 = "TumikomiKenpin/RequestTsumikomi060"; // 積込検品用Proc sagyou5
            public readonly static string TUMIKOMI070 = "TumikomiKenpin/RequestTsumikomi070"; // 貨物Noスキャン時、各分類のカウントを取得
            public readonly static string TUMIKOMI080 = "TumikomiKenpin/RequestTsumikomi080"; // 積込検品用Proc sagyou7
            public readonly static string TUMIKOMI090 = "TumikomiKenpin/RequestTsumikomi090"; // Back
            public readonly static string TUMIKOMI100 = "TumikomiKenpin/RequestTsumikomi100"; // m_file取得
            public readonly static string TUMIKOMI110 = "TumikomiKenpin/RequestTsumikomi110"; // m_file取得

            public readonly static string TUMIKOMI140 = "TumikomiKenpin/RequestTsumikomi140"; // mb_file取得
            public readonly static string TUMIKOMI160 = "TumikomiKenpin/RequestTsumikomi160"; // so_file取得
            public readonly static string TUMIKOMI180 = "TumikomiKenpin/RequestTsumikomi180"; // ps_file取得
            public readonly static string TUMIKOMI190 = "TumikomiKenpin/RequestTsumikomi190"; // FTP_file取得

            public readonly static string TUMIKOMI210 = "TumikomiKenpin/RequestTsumikomi210"; // 積込検品用Proc(配車テーブル実績数更新) sagyou5

            public readonly static string TUMIKOMI230 = "TumikomiKenpin/RequestTsumikomi230"; // 積込検品用Proc(配車テーブル実績数更新) sagyou5

            public readonly static string TUMIKOMI260 = "TumikomiKenpin/RequestTsumikomi260"; // mate_file取得
            public readonly static string TUMIKOMI270 = "TumikomiKenpin/RequestTsumikomi270"; // tokui_file取得

            public readonly static string TUMIKOMI300 = "RequestTumikomi300"; // 該当店舗の各マテハン数を取得(定番コース)

            // 未利用
            public readonly static string TUMIKOMI310 = "RequestTumikomi310"; // 該当店舗の各マテハン数を取得(定番コース)
            public readonly static string TUMIKOMI312 = "RequestTumikomi312"; // Back
            public readonly static string TUMIKOMI314 = "RequestTumikomi314"; // 積込検品用Proc(配車テーブル実績数更新) sagyou7
        };

        public class MATEHAN
        {
            public readonly static string MATE010 = "MateHan/RequestMATE010";
            public readonly static string MATE020 = "MateHan/RequestMATE020";
            public readonly static string MATE030 = "MateHan/RequestMATE030";
            public readonly static string MATE040 = "MateHan/RequestMATE040";
            public readonly static string MATE050 = "MateHan/RequestMATE050";
            public readonly static string MATE060 = "MateHan/RequestMATE060";
        };

        public class SEND_DATA
        {
            public readonly static string SEND010 = "SendData/RequestSEND010";
        }

        public class TIDOU
        {
            public readonly static string TIDOU001 = "TenpoIdou/RequestTIDOU001";
            public readonly static string TIDOU002 = "TenpoIdou/RequestTIDOU002";
            public readonly static string TIDOU010 = "TenpoIdou/RequestTIDOU010";
        }

        public class MAIL
        {
            public readonly static string MAIL010 = "MailBack/RequestMAIL010";
            public readonly static string MAIL020 = "MailBack/RequestMAIL020";
            public readonly static string MAIL030 = "MailBack/RequestMAIL030";
            public readonly static string MAIL040 = "MailBack/RequestMAIL040";
        }

        #region ログイン=====================================================================

        public static LOGIN010 RequestLogin010(string soukoCd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "souko_cd", soukoCd }
            };

            string resultData = CommonUtils.Post(WEB_SERVICE_URL + LOGIN.LOGIN010, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultData);

            if (response.status == "0")
            {
                return response.GetDataObject<LOGIN010>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static LOGIN020 RequestLogin020(string hht_id)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "hht_id",  hht_id }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + LOGIN.LOGIN020, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<LOGIN020>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static LOGIN030 RequestLogin030(string driverCd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "tantohsya_cd",  driverCd }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + LOGIN.LOGIN030, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<LOGIN030>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static void RequestLogin040(string driverCd, string soukoCd, string hht_id)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "driver_cd",  driverCd },
                { "souko_cd",  soukoCd },
                { "hht_id",  hht_id }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + LOGIN.LOGIN040, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<Tanto> RequestLogin050()
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + LOGIN.LOGIN050, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<Tanto>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }
        #endregion

        #region 個数=====================================================================

        public static int RequestKosu010(string tokuisaki_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "tokuisaki_cd",  tokuisaki_cd }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU010, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                int cnt = int.Parse(result["cnt"]);
                return cnt;
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static int RequestKosu020(string tokuisaki_cd, string todokesaki_cd)
        {

            Dictionary<string, string> param = new Dictionary<string, string>
                {
                    { "tokuisaki_cd",  tokuisaki_cd},
                    { "todokesaki_cd", todokesaki_cd}
                };


            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU020, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                int cnt = int.Parse(result["cnt"]);
                return cnt;
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static int RequestKosu040(string soukoCd, string kitakuCd, string syukaDate, string binNo, string tokuisaki_cd, string todokesaki_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  soukoCd },
                { "kitaku_cd", kitakuCd },
                { "syuka_date",  syukaDate },
                { "bin_no",  binNo },
                { "tokuisaki_cd",  tokuisaki_cd },
                { "todokesaki_cd",  todokesaki_cd }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU040, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                int state = int.Parse(result["state"]);
                return state;
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static KOSU050 RequestKosu050(string tokuisaki_cd, string todokesaki_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "tokuisaki_cd",  tokuisaki_cd},
                { "todokesaki_cd", todokesaki_cd}
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU050, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU050>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<KOSU060> RequestKosu060(string soukoCd, string kitakuCd, string syukaDate, string binNo)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  soukoCd},
                { "kitaku_cd", kitakuCd},
                { "syuka_date", syukaDate},
                { "bin_no", binNo}
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU060, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<KOSU060>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<KOSU060> RequestKosu065(string soukoCd, string kitakuCd, string syukaDate, string binNo)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  soukoCd},
                { "kitaku_cd", kitakuCd},
                { "syuka_date", syukaDate},
                { "bin_no", binNo}
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU065, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<KOSU060>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static KOSU070 RequestKosu070(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU070, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 紐付作業取消ー未完了
        public static KOSU070 RequestKosu080(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU080, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 紐付作業取消ー未完了
        public static KOSU070 RequestKosu085(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU085, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 紐付作業取消ー未完了
        public static List<KOSU095> RequestKosu095(string kenpin_souko, string kitaku_cd, string syuka_date)
        {

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko},
                {"kitaku_cd", kitaku_cd},
                {"syuka_date", syuka_date},
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU095, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<KOSU095>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 紐付完了
        public static KOSU110 RequestKosu110(string kenpin_souko, string kitaku_cd, string syuka_date
            , string bin_no, string tokuisaki_cd, string todokesaki_cd)
        {

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko},
                {"kitaku_cd", kitaku_cd},
                {"syuka_date", syuka_date},
                {"bin_no", bin_no},
                {"tokuisaki_cd", tokuisaki_cd},
                {"todokesaki_cd", todokesaki_cd},
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU110, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU110>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 紐付完了
        public static KOSU110 RequestKosu115(string kenpin_souko, string kitaku_cd, string syuka_date, string vendor_cd)
        {

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko},
                {"kitaku_cd", kitaku_cd},
                {"syuka_date", syuka_date},
                {"vendor_cd", vendor_cd},
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU115, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU110>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 未検一覧取得（届先で検索）
        public static List<KOSU120> RequestKosu120(string kenpin_souko, string kitaku_cd, string syuka_date, string tokuisaki_cd, string todokesaki_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko},
                {"kitaku_cd", kitaku_cd},
                {"syuka_date", syuka_date},
                {"tokuisaki_cd", tokuisaki_cd},
                {"todokesaki_cd", todokesaki_cd},
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU120, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<KOSU120>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 未検一覧取得（ベンダーで検索）
        public static List<KOSU120> RequestKosu125(string kenpin_souko, string kitaku_cd, string syuka_date, string vendor_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko},
                {"kitaku_cd", kitaku_cd},
                {"syuka_date", syuka_date},
                {"vendor_cd", vendor_cd},
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU125, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<KOSU120>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static KOSU131 RequestKosu131(string soukoCd, string kitakuCd, string syukaDate, string venderCd)
        {

            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  soukoCd},
                { "kitaku_cd",  kitakuCd},
                { "syuka_date",  syukaDate},
                { "vendor_cd",  venderCd}
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU131, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU131>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static KOSU070 RequestKosu150(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU150, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 紐付作業取消ー未完了
        public static KOSU070 RequestKosu160(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU160, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 紐付作業取消ー未完了
        public static KOSU070 RequestKosu165(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU165, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // バラ検品：バーコード 
        public static KOSU070 RequestKosu170(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU170, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static KOSU070 RequestKosu180(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU180, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// バラ検品：バーコード 
        public static KOSU070 RequestKosu185(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU185, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// バラ検品：バーコード 
        public static List<KOSU190> RequestKosu190()
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU190, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<KOSU190>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }


        public static List<KOSU200> RequestKosu200(string vendorCode)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU200, new Dictionary<string, string> { { "vendor_cd", vendorCode } });
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<KOSU200>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 個数上限値
        public static int RequestKosu210()
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU210, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                return int.Parse(result["kohmoku"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // ベンダー検索(マテハン積付)
        public static string RequestKosu220(string vendor_cd)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU220, new Dictionary<string, string> { { "vendor_cd", vendor_cd } });
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                return result["vendor_nm"];
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // ベンダー指定検品から店舗取得
        public static KOSU230 RequestKosu230(string souko_cd, string kitaku_cd, string syuka_date, string kamotsu_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"souko_cd", souko_cd },
                {"kitaku_cd", kitaku_cd },
                {"syuka_date", syuka_date },
                {"kamotsu_no", kamotsu_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU230, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<KOSU230>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // ベンダー指定検品満タン処理後の未検品店舗件数 
        public static int RequestKosu240(string souko_cd, string kitaku_cd, string syuka_date)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"souko_cd", souko_cd },
                {"kitaku_cd", kitaku_cd },
                {"syuka_date", syuka_date }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + KOSU.KOSU240, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                return int.Parse(result["cnt"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        #endregion

        #region 積込=====================================================================

        public static TUMIKOMI010 RequestTumikomi010(string kenpin_souko, string kitaku_cd, string syuka_date, string course)
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                {"kenpin_souko",kenpin_souko},
                {"kitaku_cd",kitaku_cd},
                {"syuka_date",syuka_date},
                {"course",course}
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI010, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                List<TUMIKOMI010> result = response.GetDataObject<List<TUMIKOMI010>>();
                return result.Count > 0 ? result[0] : null;
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<TUMIKOMI020> RequestTumikomi020(string kenpin_souko, string kitaku_cd, string syuka_date, string bin_no, string course)
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                {"kenpin_souko",kenpin_souko},
                {"kitaku_cd",kitaku_cd},
                {"syuka_date",syuka_date},
                {"bin_no",bin_no},
                {"course",course}
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI020, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<TUMIKOMI020>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static int RequestTumikomi030(string kenpin_souko, string kitaku_cd, string syuka_date, string tokuisaki_cd, string todokesaki_cd, string bin_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                {"kenpin_souko",kenpin_souko},
                {"kitaku_cd",kitaku_cd},
                {"syuka_date",syuka_date},
                {"tokuisaki_cd",tokuisaki_cd},
                {"todokesaki_cd",todokesaki_cd},
                {"bin_no",bin_no}
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI030, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return int.Parse((response.GetDataObject<List<Dictionary<string,string>>>())[0]["cnt"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<TUMIKOMI040> RequestTumikomi040(string souko_cd, string kitaku_cd, string syuka_date, string tokuisaki_cd, string todokesaki_cd, string bin_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  souko_cd},
                { "kitaku_cd", kitaku_cd },
                { "syuka_date", syuka_date },
                { "tokuisaki_cd", tokuisaki_cd },
                { "todokesaki_cd", todokesaki_cd },
                { "bin_no", bin_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI040, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<TUMIKOMI040>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static int RequestTumikomi050(string souko_cd, string kitaku_cd, string syuka_date, string tokuisaki_cd, string todokesaki_cd, string bin_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  souko_cd},
                { "kitaku_cd", kitaku_cd },
                { "syuka_date", syuka_date },
                { "tokuisaki_cd", tokuisaki_cd },
                { "todokesaki_cd", todokesaki_cd },
                { "bin_no", bin_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI050, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return int.Parse(response.GetDataObject()["kosu_kei"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static TUMIKOMI060 RequestTumikomi060(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI060, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TUMIKOMI060>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static TUMIKOMI070 RequestTumikomi070(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI070, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TUMIKOMI070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static TUMIKOMI080 RequestTumikomi080(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI080, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TUMIKOMI080>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static TUMIKOMI090 RequestTumikomi090(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI090, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TUMIKOMI090>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }
        
        public static List<MFile> RequestTumikomi100(string kenpin_souko, string kitaku_cd, string syuka_date, string bin_no, string course, string tokuisaki_cd, string todokesaki_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  kenpin_souko},
                { "kitaku_cd", kitaku_cd },
                { "syuka_date", syuka_date },
                { "tokuisaki_cd", tokuisaki_cd },
                { "todokesaki_cd", todokesaki_cd },
                { "bin_no", bin_no },
                { "course", course }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI100, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<MFile>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static TUMIKOMI110 RequestTumikomi110(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi110", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TUMIKOMI110>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static int RequestTumikomi120(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi120", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return int.Parse(response.GetDataObject()["cnt"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // パスワード
        public static string RequestTumikomi130(string souko_cd, string hiduke)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "souko_cd", souko_cd},
                { "hiduke", hiduke}
            };

            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi130", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                List<Dictionary<string, string>> result = response.GetDataObject<List<Dictionary<string, string>>>();
                if (result.Count == 0)
                {
                    return "";
                }

                return result[0]["pass"];
                // return response.GetDataObject()["pass"];
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<MbFile> RequestTumikomi140(string souko_cd, string kitaku_cd, string haisoh_date, string bin_no, string course)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"souko_cd", souko_cd },
                {"kitaku_cd", kitaku_cd },
                {"haisoh_date", haisoh_date },
                {"bin_no", bin_no },
                {"course", course },
            };
            
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI140, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<MbFile>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static TUMIKOMI150 RequestTumikomi150(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi150", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TUMIKOMI150>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static SoFile RequestTumikomi160(string souko_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"souko_cd", souko_cd }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI160, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<SoFile>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static string RequestTumikomi170(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi170", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject()["kamotsu_no"];
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static PsFile RequestTumikomi180(string souko_cd, string hiduke)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("souko_cd", souko_cd);
            param.Add("hiduke", hiduke);

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI180, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<PsFile>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static FtpFile RequestTumikomi190()
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI190, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<FtpFile>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static MTumikomiProc RequestTumikomi200(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi200", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static MTumikomiProc RequestTumikomi210(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi210", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }
        // 積込検品用Proc
        public static MTumikomiProc RequestTumikomi220(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi220", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static int RequestTumikomi230(string kenpin_souko, string kitaku_cd, string syuka_date, string nohin_date, string bin_no, string course)
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                {"kenpin_souko",kenpin_souko},
                {"kitaku_cd",kitaku_cd},
                {"syuka_date",syuka_date},
                {"nohin_date",nohin_date},
                {"bin_no",bin_no},
                {"course",course}
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI230, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                return int.Parse(result["cnt"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static MTumikomiProc RequestTumikomi240(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi240", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static int RequestTumikomi250(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post("TumikomiKenpin/RequestTsumikomi250", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return int.Parse(response.GetDataObject()["cnt"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<MateFile> RequestTumikomi260()
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI260, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<MateFile>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<TokuiFile> RequestTumikomi270()
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI270, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<TokuiFile>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }
        
        public static List<TUMIKOMI040> RequestTumikomi300(string souko_cd, string kitaku_cd, string syuka_date, string tokuisaki_cd, string todokesaki_cd, string bin_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  souko_cd},
                { "kitaku_cd", kitaku_cd },
                { "syuka_date", syuka_date },
                { "tokuisaki_cd", tokuisaki_cd },
                { "todokesaki_cd", todokesaki_cd },
                { "bin_no", bin_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI300, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<TUMIKOMI040>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static TUMIKOMI310 RequestTumikomi310(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TUMIKOMI.TUMIKOMI310, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TUMIKOMI310>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static MTumikomiProc RequestTumikomi311(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + "TumikomiKenpin/RequestTsumikomi311", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static MTumikomiProc RequestTumikomi312(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + "TumikomiKenpin/RequestTsumikomi312", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static MTumikomiProc RequestTumikomi313(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + "TumikomiKenpin/RequestTsumikomi313", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static MTumikomiProc RequestTumikomi314(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + "TumikomiKenpin/RequestTsumikomi314", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc
        public static MTumikomiProc RequestTumikomi319(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + "TumikomiKenpin/RequestTsumikomi319", param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        // 積込検品用Proc(TEMP)
        public static MTumikomiProc CallTumiKomiProc(string kbn, Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + "TumikomiKenpin/RequestTsumikomi" + kbn, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MTumikomiProc>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        #endregion

        #region データ送信=====================================================================

        public static int RequestSend010(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + SEND_DATA.SEND010, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                return int.Parse(result["poRet"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        #endregion

        #region マテハン=====================================================================

        public static string RequestMate010(string vendor_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"vendor_cd", vendor_cd }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MATEHAN.MATE010, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                return result["vendor_nm"];
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }


        public static List<KOSU190> RequestMate020(string nyuka_souko)
        {
            //MATE020は利用しない。KOSU190と同様
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"nyuka_souko", nyuka_souko }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MATEHAN.MATE020, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<KOSU190>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<MATE030> RequestMate030(string vendor_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"vendor_cd", vendor_cd }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MATEHAN.MATE030, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<MATE030>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static string RequestMate040(string name_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"name_cd", name_cd }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MATEHAN.MATE040, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                return result["code_name"];
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static int RequestMate050(string system_kbn)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"system_kbn", system_kbn }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MATEHAN.MATE050, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                Dictionary<string, string> result = response.GetDataObject();
                return int.Parse(result["kohmoku"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static MATE060 RequestMate060(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MATEHAN.MATE060, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<MATE060>();        
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        #endregion

        #region 移動先店舗登録=====================================================================

        public static TIDOU001 RequestTidou001(string kamotsu_no, string syuka_date)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kamotsu_no", kamotsu_no },
                {"syuka_date", syuka_date },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TIDOU.TIDOU001, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TIDOU001>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }
        
        public static TIDOU002 RequestTidou002(string tokuisaki_cd, string todokesaki_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"tokuisaki_cd", tokuisaki_cd },
                {"todokesaki_cd", todokesaki_cd },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TIDOU.TIDOU002, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TIDOU002>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static TIDOU010 RequestTidou010(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + TIDOU.TIDOU010, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<TIDOU010>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        #endregion

        #region 積替検品=====================================================================

        /// <summary>
        /// 単品移動確定
        /// </summary>
        /// <param name="kenpin_souko"></param>
        /// <param name="kitaku_cd"></param>
        /// <param name="kamotsu_no"></param>
        /// <returns></returns>
        public static List<IDOU010> RequestIdou010(string kenpin_souko, string kitaku_cd, string kamotsu_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko },
                {"kitaku_cd", kitaku_cd },
                {"kamotsu_no", kamotsu_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU010, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<IDOU010>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// 全品移動入力
        /// </summary>
        /// <param name="kenpin_souko"></param>
        /// <param name="kitaku_cd"></param>
        /// <param name="kamotsu_no"></param>
        /// <returns></returns>
        public static List<IDOU020> RequestIdou020(string kenpin_souko, string kitaku_cd, string kamotsu_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko },
                {"kitaku_cd", kitaku_cd },
                {"kamotsu_no", kamotsu_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU020, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<IDOU020>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// マテハン移動入力
        /// </summary>
        /// <param name="kenpin_souko"></param>
        /// <param name="kitaku_cd"></param>
        /// <param name="kamotsu_no"></param>
        /// <returns></returns>
        public static List<IDOU030> RequestIdou030(string kenpin_souko, string kitaku_cd, string kamotsu_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko },
                {"kitaku_cd", kitaku_cd },
                {"kamotsu_no", kamotsu_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU030, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<IDOU030>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// ベンダー確認・ベンダー名取得
        /// </summary>
        /// <param name="kenpin_souko"></param>
        /// <param name="kitaku_cd"></param>
        /// <param name="kamotsu_no"></param>
        /// <returns></returns>
        public static IDOU031 RequestIdou031(string kenpin_souko, string kitaku_cd, string kamotsu_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko },
                {"kitaku_cd", kitaku_cd },
                {"kamotsu_no", kamotsu_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU031, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<IDOU031>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// 届先確認
        /// </summary>
        /// <param name="kenpin_souko"></param>
        /// <param name="kitaku_cd"></param>
        /// <param name="kamotsu_no"></param>
        /// <returns></returns>
        public static IDOU033 RequestIdou033(string kenpin_souko, string kitaku_cd, string kamotsu_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"kenpin_souko", kenpin_souko },
                {"kitaku_cd", kitaku_cd },
                {"kamotsu_no", kamotsu_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU033, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<IDOU033>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// 積込移動用Proc
        /// 00:「2.単品移動入力」単品移動で移動元の確認 
        /// </summary>
        /// <param name="pTerminalID"></param>
        /// <param name="pProgramID"></param>
        /// <param name="pSagyosyaCD"></param>
        /// <param name="pSoukoCD"></param>
        /// <param name="pMotoKamotsuNo"></param>
        /// <param name="pGyomuKbn"></param>
        /// <returns></returns>
        public static IDOU040 RequestIdou040(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU040, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<IDOU040>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// 積込移動用Proc
        /// 01:「3.単品移動確定」単品移動で移動を確定する 
        /// </summary>
        /// <param name="pTerminalID"></param>
        /// <param name="pProgramID"></param>
        /// <param name="pSagyosyaCD"></param>
        /// <param name="pSoukoCD"></param>
        /// <param name="pMotoKamotsuNo"></param>
        /// <param name="pSakiKamotsuNo"></param>
        /// <param name="pGyomuKbn"></param>
        /// <param name="pVendorCd"></param>
        /// <returns></returns>
        public static IDOU050 RequestIdou050(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU050, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<IDOU050>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// 積込移動用Proc
        /// 02:「6.全品移動確定」全品の移動を確定する
        /// </summary>
        /// <param name="pTerminalID"></param>
        /// <param name="pProgramID"></param>
        /// <param name="pSagyosyaCD"></param>
        /// <param name="pSoukoCD"></param>
        /// <param name="pMotoKamotsuNo"></param>
        /// <param name="pSakiKamotsuNo"></param>
        /// <param name="pGyomuKbn"></param>
        /// <param name="pVendorCd"></param>
        /// <returns></returns>
        public static IDOU060 RequestIdou060(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU060, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<IDOU060>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// 積込移動用Proc
        /// 03:「9.マテハン移動確定」マテハンの移動を確定する 
        /// </summary>
        /// <param name="pTerminalID"></param>
        /// <param name="pProgramID"></param>
        /// <param name="pSagyosyaCD"></param>
        /// <param name="pSoukoCD"></param>
        /// <param name="pMotoKamotsuNo"></param>
        /// <param name="pSakiKamotsuNo"></param>
        /// <param name="pGyomuKbn"></param>
        /// <param name="pVendorCd"></param>
        /// <returns></returns>
        public static IDOU070 RequestIdou070(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU070, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<IDOU070>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// 積込移動用Proc([単品移動]マテハン連番取得)
        /// 04:「単品マテハン変更」マテハンNo取得
        /// </summary>
        /// <param name="pTerminalID"></param>
        /// <param name="pProgramID"></param>
        /// <param name="pSagyosyaCD"></param>
        /// <param name="pSoukoCD"></param>
        /// <param name="pMotoKamotsuNo"></param>
        /// <param name="pSakiKamotsuNo"></param>
        /// <param name="pGyomuKbn"></param>
        /// <param name="pVendorCd"></param>
        /// <returns></returns>
        public static IDOU080 RequestIdou080(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU080, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<IDOU080>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// 積込移動用Proc([単品移動]マテハン登録)
        /// 05:「単品マテハン変更」単品でマテハンの変更
        /// </summary>
        /// <param name="pTerminalID"></param>
        /// <param name="pProgramID"></param>
        /// <param name="pSagyosyaCD"></param>
        /// <param name="pSoukoCD"></param>
        /// <param name="pMotoKamotsuNo"></param>
        /// <param name="pSakiKamotsuNo"></param>
        /// <param name="pGyomuKbn"></param>
        /// <param name="pVendorCd"></param>
        /// <returns></returns>
        public static IDOU090 RequestIdou090(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + IDOU.IDOU090, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<IDOU090>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        #endregion

        #region メールバック=====================================================================

        /// <summary>
        /// メールバック登録
        /// </summary>
        /// <param name="pSagyosyaCD"></param>
        /// <param name="pSoukoCD"></param>
        /// <param name="pHaisoDate"></param>
        /// <param name="pBinNo"></param>
        /// <param name="pTokuisakiCD"></param>
        /// <param name="pTodokesakiCD"></param>
        /// <param name="pKanriNo"></param>
        /// <returns></returns>
        public static Dictionary<string, string> RequestMAIL010(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MAIL.MAIL010, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// 登録済みメールバッグ数を取得
        /// </summary>
        /// <param name="souko_cd"></param>
        /// <param name="haisoh_date"></param>
        /// <param name="bin_no"></param>
        /// <returns></returns>
        public static int RequestMAIL020(string souko_cd, string haisoh_date, string bin_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"souko_cd", souko_cd },
                {"haisoh_date", haisoh_date },
                {"bin_no", bin_no },
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MAIL.MAIL020, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return int.Parse(response.GetDataObject()["cnt"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// メールバック削除
        /// </summary>
        /// <param name="pSagyosyaCD"></param>
        /// <param name="pSoukoCD"></param>
        /// <param name="pHaisoDate"></param>
        /// <param name="pBinNo"></param>
        /// <param name="pTokuisakiCD"></param>
        /// <param name="pTodokesakiCD"></param>
        /// <param name="pKanriNo"></param>
        /// <returns></returns>
        public static Dictionary<string, string> RequestMAIL030(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MAIL.MAIL030, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        /// <summary>
        /// メールバック削除存在チェック 
        /// </summary>
        /// <param name="kenpin_souko"></param>
        /// <param name="kitaku_cd"></param>
        /// <param name="kamotsu_no"></param>
        /// <returns></returns>
        public static int RequestMAIL040(string kenpin_souko, string kitaku_cd, string kamotsu_no)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                {"souko_cd", kenpin_souko },
                {"kitaku_cd", kitaku_cd },
                {"haisoh_date", kamotsu_no },
                {"tokuisaki_cd", kamotsu_no },
                {"todokesaki_cd", kamotsu_no },
                {"kanri_no", kamotsu_no }
            };

            string resultJson = CommonUtils.Post(WEB_SERVICE_URL + MAIL.MAIL040, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return int.Parse(response.GetDataObject()["cnt"]);
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        #endregion

        public static void SetHostIpAddress(string hostIp, string port)
        {
            HOST_ADDRESS = hostIp;
            PORT = port;
            WEB_SERVICE_URL = "http://" + hostIp + ":" + port  + "/";
        }

        public static string GetHostIpAddress()
        {
            return HOST_ADDRESS;
        }
        public static string GetPort()
        {
            return PORT;
        }

    }

}