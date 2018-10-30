using System;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Util;
using Android.Views;
using Android.Widget;
using Com.Beardedhen.Androidbootstrap;

namespace HHT
{
    public class MailManageSelectFragment : BaseFragment
    {
        private readonly string TAG = "MailManageSelectFragment";

        private ISharedPreferences prefs;
        private ISharedPreferencesEditor editor;

        private View view;
        private BootstrapEditText etHaisoDate, etBin;
        
        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            view = inflater.Inflate(Resource.Layout.fragment_mail_manage_select, container, false);
            prefs = PreferenceManager.GetDefaultSharedPreferences(Context);
            editor = prefs.Edit();

            // コンポーネント初期化
            InitComponent();
            
            return view;
        }

        // コンポーネント初期化
        private void InitComponent()
        {
            bool registFlg = prefs.GetBoolean("registFlg", true);

            if (registFlg)
            {
                SetTitle("メールバッグ登録");
            }
            else
            {
                SetTitle("メールバッグ削除");
            }
            
            etBin = view.FindViewById<BootstrapEditText>(Resource.Id.et_mailRegistSelect_bin);

            BootstrapButton btnConfirm = view.FindViewById<BootstrapButton>(Resource.Id.btn_mailRegistSelect_confirm);
            btnConfirm.Click += delegate { Confirm(); };

            etHaisoDate = view.FindViewById<BootstrapEditText>(Resource.Id.et_mailRegistSelect_haiso);
            etHaisoDate.FocusChange += (sender, e) => {
                if (e.HasFocus)
                {
                    etHaisoDate.Text = etHaisoDate.Text.Replace("/", "");
                    etHaisoDate.SetSelection(etHaisoDate.Text.Length);
                }
                else
                {
                    try
                    {
                        etHaisoDate.Text = CommonUtils.GetDateYYYYMMDDwithSlash(etHaisoDate.Text);
                    }
                    catch
                    {
                        ShowDialog("エラー", "日付を正しく入力してください。", () => {
                            etHaisoDate.Text = "";
                            etHaisoDate.RequestFocus();
                        });
                    }
                }
            };

            etHaisoDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            etBin.RequestFocus();

        }
        
        private void Confirm()
        {   
            // Input Check
            if(etHaisoDate.Text == "")
            {
                ShowDialog("エラー", "日付を入力してください。", () => {
                    etHaisoDate.RequestFocus();
                });
                return;
            }

            if (etBin.Text == "")
            {
                ShowDialog("エラー", "便を入力してください。", () => {
                    etBin.RequestFocus();
                });
                return;
            }

            // 登録済みメールバッグ数を取得
            string souko_cd = prefs.GetString("souko_cd", "");
            string haisoDate = etHaisoDate.Text.Replace("/", "");

            try
            {
                int cnt = WebService.RequestMAIL020(souko_cd, haisoDate, etBin.Text);
                editor.PutString("haiso_date", haisoDate);
                editor.PutString("bin_no", etBin.Text);
                editor.PutInt("mail_back", cnt);
                editor.Apply();
                
                StartFragment(FragmentManager, typeof(MailManageInputFragment));
            }
            catch
            {
                ShowDialog("エラー", "登録済みメールバッグ数取得に失敗しました。", () => {});
                Log.Error(TAG, "登録済みメールバッグ数取得に失敗しました。");
            }
        }

        public override bool OnKeyDown(Keycode keycode, KeyEvent paramKeyEvent)
        {
            if (keycode == Keycode.F4)
            {
                Confirm();
            }

            return true;
        }
    }
}