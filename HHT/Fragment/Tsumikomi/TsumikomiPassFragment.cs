using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;
using HHT.Resources.DataHelper;
using HHT.Resources.Model;
using System;
using System.Collections.Generic;

namespace HHT
{
    public class TsumikomiPassFragment : BaseFragment
    {
        private readonly string TAG = "TsumikomiPassFragment";

        private View view;
        private EditText etPassword;
        private string passwrod;
        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_tsumikomi_pass, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            SetTitle("積込検品");
            
            BootstrapButton pwdConfirmBtn = view.FindViewById<BootstrapButton>(Resource.Id.btn_tsumikomiManger_pwdConfirm);
            pwdConfirmBtn.Click += delegate { CheckPassword(); };

            string soukoCd = prefs.GetString("souko_cd", "");   //倉庫コード
            string hiduke = prefs.GetString("syuka_date", "");  // 配送日

            passwrod = WebService.RequestTumikomi130(soukoCd, hiduke); // パスワード取得
            etPassword = view.FindViewById<EditText>(Resource.Id.password);
            etPassword.RequestFocus();

            return view;
        }

        private void CheckPassword()
        {
            if (etPassword.Text == "") return;

            if (etPassword.Text != passwrod)
            {
                ShowDialog("エラー", "認証できませんでした。", () => { });
                return;
            }

            string comp_kbn = prefs.GetString("comp_kbn", "0");
            if (comp_kbn  == "0")
            {
                string menuFlg = prefs.GetString("menuFlg", "1");
                string message = (menuFlg == "1") ? "増便を行います。\n" : "強制出発を行います。\n";
                message += "よろしいですか？" + "\n";

                ShowDialog("確認", message, () => {
                    if (menuFlg == "1")
                    {
                        // sagyou12
                    }
                    else
                    {

                    }
                    // menu_flg == 1 sagyou12

                    // menu_flg == 3 

                });
            }
            else if (comp_kbn == "1")
            {
                // 強制的に積込検品を完了する
                CreateTsumiFiles();
            }
            else
            {
                // sagyou9 
            }

        }
        
        // 積込完了時に生成されるファイル（納品で使います。）
        private void CreateTsumiFiles()
        {
            string souko_cd = prefs.GetString("souko_cd", "");
            string kitaku_cd = prefs.GetString("kitaku_cd", "");
            string syuka_date = prefs.GetString("syuka_date", "");
            string tokuisaki_cd = prefs.GetString("tokuisaki_cd", "");
            string todokesaki_cd = prefs.GetString("todokesaki_cd", "");
            string bin_no = prefs.GetString("bin_no", "");
            string course = prefs.GetString("course", "");
            
            // CRATE TUMIKOMI FILE
            // MAIN FILE
            List<MFile> mFiles = WebService.RequestTumikomi100(souko_cd, kitaku_cd, syuka_date, bin_no, course, tokuisaki_cd, todokesaki_cd);
            new MFileHelper().InsertALL(mFiles);

            // It would be useless..
            //PsFile psFile = WebService.RequestTumikomi180();
            PsFile psFile = new PsFile { pass = "" };
            new PsFileHelper().Insert(psFile);

            // MAILBACK FILE 
            List<MbFile> mbFiles = WebService.RequestTumikomi140(souko_cd, kitaku_cd, syuka_date, bin_no, course);
            new MbFileHelper().InsertAll(mbFiles);

            // SOUKO FILE
            SoFile soFile = WebService.RequestTumikomi160(souko_cd);
            new SoFileHelper().Insert(soFile);
            
            // VENDOR FILE
            string nohin_date = DateTime.Now.ToString("yyyyMMdd");
            List<MateFile> mateFile = WebService.RequestTumikomi260(souko_cd, kitaku_cd, syuka_date, nohin_date, bin_no, course);
            new MateFileHelper().InsertAll(mateFile);

            // TOKUISAKI FILE
            List<TokuiFile> tokuiFile = WebService.RequestTumikomi270();
            new TokuiFileHelper().InsertAll(tokuiFile);

            Log.Debug(TAG, "CreateTsumiFiles end");

            Dictionary<string, string> param = new Dictionary<string, string>
                        {
                            { "pTerminalID",  prefs.GetString("terminal_id","")},
                            { "pProgramID", "TUM" },
                            { "pSagyosyaCD", prefs.GetString("sagyousya_cd","") },
                            { "pSoukoCD",  souko_cd},
                            { "pSyukaDate", syuka_date},
                            { "pBinNo", bin_no},
                            { "pCourse", course },
                            { "pTokuisakiCD", tokuisaki_cd },
                            { "pTodokesakiCD", todokesaki_cd },
                            { "pHHT_No", prefs.GetString("hht_no","") }
                        };

            //配車テーブルの該当コースの各数量を実績数で更新する
            var updateResult = WebService.CallTumiKomiProc("210", param);

            if (updateResult.poRet == "0" || updateResult.poRet == "99")
            {
                editor.PutBoolean("tenpo_zan_flg", updateResult.poRet == "99" ? true : false);
                editor.Apply();

                Activity.RunOnUiThread(() =>
                {
                    //	正常登録
                    ShowDialog("報告", "積込検品が\n完了しました。", () => {
                        FragmentManager.PopBackStack(FragmentManager.GetBackStackEntryAt(0).Id, 0);
                    });
                });

            }
            else
            {
                ShowDialog("エラー", "表示データがありません", () => {});
                return;
            }
        }
    }
}
 