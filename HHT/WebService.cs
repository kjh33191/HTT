﻿
using Android.Util;
using HHT.Resources.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HHT
{
    public class WebService
    {
        public readonly static string TAG = "WebService";
        
        public readonly static string HOST_ADDRESS = "172.26.190.133";
        //public readonly static string HOST_ADDRESS = "192.168.0.19";
        public readonly static string WEB_SERVICE_URL = "http://" + HOST_ADDRESS + ":8787/";
        
        public class LOGIN {
            readonly string LOGIN001 = WEB_SERVICE_URL + "login/RequestLogin001";
            public readonly static string LOGIN010 = WEB_SERVICE_URL + "login/RequestLogin010";
            public readonly static string LOGIN020 = WEB_SERVICE_URL + "login/RequestLogin020";
            public readonly static string LOGIN030 = WEB_SERVICE_URL + "login/RequestLogin030";
            public readonly static string LOGIN040 = WEB_SERVICE_URL + "login/RequestLogin040";
            public readonly static string LOGIN050 = WEB_SERVICE_URL + "login/RequestLogin050";
        };

        public class KOSU
        {
            public readonly static string KOSU010 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu010";
            public readonly static string KOSU020 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu020";
            public readonly static string KOSU030 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu030";
            public readonly static string KOSU040 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu040";
            public readonly static string KOSU050 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu050";
            public readonly static string KOSU060 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu060";
            public readonly static string KOSU065 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu065";
            public readonly static string KOSU070 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu070";
            public readonly static string KOSU080 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu080";
            public readonly static string KOSU131 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu131";
            public readonly static string KOSU150 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu150";
            public readonly static string KOSU160 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu160";

            public readonly static string KOSU200 = WEB_SERVICE_URL + "KosuKenpin/RequestKosu200";
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


            public readonly static string TUMIKOMI230 = WEB_SERVICE_URL + "RequestTumikomi230"; // 積込検品用Proc(配車テーブル実績数更新) sagyou5


            public readonly static string TUMIKOMI300 = WEB_SERVICE_URL + "RequestTumikomi300"; // 該当店舗の各マテハン数を取得(定番コース)
            public readonly static string TUMIKOMI312 = WEB_SERVICE_URL + "RequestTumikomi312"; // Back
            public readonly static string TUMIKOMI314 = WEB_SERVICE_URL + "RequestTumikomi314"; // 積込検品用Proc(配車テーブル実績数更新) sagyou7
            public readonly static string KOSU050 = WEB_SERVICE_URL + "RequestLogin040?driver_cd={0}&souko_cd={1}&htt_id={2}";
            public readonly static string KOSU060 = WEB_SERVICE_URL + "RequestLogin050";
            public readonly static string KOSU065 = WEB_SERVICE_URL + "RequestLogin050";
        };

        //***************　ログイン　***************//
        public static LOGIN010 RequestLogin010(string soukoCd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "souko_cd", soukoCd }
            };

            string resultData = CommonUtils.Post(LOGIN.LOGIN010, param);
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

            string resultJson = CommonUtils.Post(LOGIN.LOGIN020, param);
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

            string resultJson = CommonUtils.Post(LOGIN.LOGIN030, param);
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

            string resultJson = CommonUtils.Post(LOGIN.LOGIN040, param);
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
            string resultJson = CommonUtils.Post(LOGIN.LOGIN050, new Dictionary<string, string>());
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

        //***************　個数　***************//
        public static int RequestKosu010(string tokuisaki_cd)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "tokuisaki_cd",  tokuisaki_cd }
            };

            string resultJson = CommonUtils.Post(KOSU.KOSU010, param);
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


            string resultJson = CommonUtils.Post(KOSU.KOSU020, param);
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

        // RequestKosu030()は使わない

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

            string resultJson = CommonUtils.Post(KOSU.KOSU040, param);
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

            string resultJson = CommonUtils.Post(KOSU.KOSU050, param);
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

        public static List<Todokesaki> RequestKosu060(string soukoCd, string kitakuCd, string syukaDate, string binNo)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  soukoCd},
                { "kitaku_cd", kitakuCd},
                { "syuka_date", syukaDate},
                { "bin_no", binNo}
            };

            string resultJson = CommonUtils.Post(KOSU.KOSU060, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<Todokesaki>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static List<Todokesaki> RequestKosu065(string soukoCd, string kitakuCd, string syukaDate, string binNo)
        {
            Dictionary<string, string> param = new Dictionary<string, string>
            {
                { "kenpin_souko",  soukoCd},
                { "kitaku_cd", kitakuCd},
                { "syuka_date", syukaDate},
                { "bin_no", binNo}
            };

            string resultJson = CommonUtils.Post(KOSU.KOSU065, param);
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                return response.GetDataObject<List<Todokesaki>>();
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }


        public static KOSU070 RequestKosu070(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(KOSU.KOSU070, param);
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
        public static void RequestKosu080(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(KOSU.KOSU080, param);
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

        public static KOSU070 RequestKosu150(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(KOSU.KOSU150, param);
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
        public static void RequestKosu160(Dictionary<string, string> param)
        {
            string resultJson = CommonUtils.Post(KOSU.KOSU160, param);
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

        // 紐付作業取消ー未完了
        public static int RequestKosu210()
        {
            string resultJson = CommonUtils.Post(KOSU.KOSU210, new Dictionary<string, string>());
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



        public static List<KOSU200> ExecuteKosu200(string vendorCode)
        {
            
            //string url = string.Format(@KOSU.KOSU200, vendorCode);

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


        //***************　積込　***************//
        public static TUMIKOMI010 RequestTumikomi010(string kenpin_souko, string kitaku_cd, string syuka_date, string nohin_date, string course)
        {
            Dictionary<string, string> param = new Dictionary<string, string> {
                {"kenpin_souko",kenpin_souko},
                {"kitaku_cd",kitaku_cd},
                {"syuka_date",syuka_date},
                {"nohin_date",nohin_date},
                {"course",course}
            };

            string resultJson = CommonUtils.Post(TUMIKOMI.TUMIKOMI010, new Dictionary<string, string>());
            ResponseData response = JsonConvert.DeserializeObject<ResponseData>(resultJson);

            if (response.status == "0")
            {
                TUMIKOMI010 result = response.GetDataObject<TUMIKOMI010>();
                return result;
            }
            else
            {
                Log.Error(TAG, response.message);
                throw new Exception(response.message);
            }
        }

        public static int RequestTumikomi230()
        {
            string resultJson = CommonUtils.Post(TUMIKOMI.TUMIKOMI230, new Dictionary<string, string>());
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


    }

}