//=============================================================================
// システム名称　　　： スプライトパッカー
// サブシステム名　　： 
// 機能名　　　　　　： 
// ソースファイル名　： VisualButton.cs
//-----------------------------------------------------------------------------
// 機能概要　　　　　： 画像ボタン
//-----------------------------------------------------------------------------
// 改訂履歴    区分  改訂番号  社名)担当   内容
// 2014.06.02  新規  ----      CS)杉原
//=============================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SplitePacker.Properties;

namespace SplitePacker
{
    /// <summary>
    /// 画像ボタン
    /// </summary>
    public partial class VisualButton : UserControl
    {
        /// <summary>
        /// LOG4NETのロガー
        /// </summary>
        private static log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// クリックイベント
        /// </summary>
        public event EventHandler ButtonClick;

        /// <summary>
        /// ボタンダウン画像パス
        /// </summary>
        private string _DownImagePath = string.Empty;

        /// <summary>
        /// ボタンダウン画像パス
        /// </summary>
        public string DownImagePath
        {
            get
            {
                return this._DownImagePath;
            }
            set
            {
                this._DownImagePath = value;
                this._DownImage = this.LoadImage(this._DownImagePath);
            }
        }

        /// <summary>
        /// ボタンアップ画像パス
        /// </summary>
        private string _UpImagePath = string.Empty;

        /// <summary>
        /// ボタンアップ画像パス
        /// </summary>
        public string UpImagePath
        {
            get
            {
                return this._UpImagePath;
            }
            set
            {
                this._UpImagePath = value;
                this._UpImage = this.LoadImage(this._UpImagePath);
            }
        }

        /// <summary>
        /// ボタン無効画像パス
        /// </summary>
        private string _DisableImagePath = string.Empty;

        /// <summary>
        /// ボタン無効画像パス
        /// </summary>
        public string DisableImagePath
        {
            get
            {
                return this._DisableImagePath;
            }
            set
            {
                this._DisableImagePath = value;
                this._DisableImage = this.LoadImage(this._DisableImagePath);
            }
        }

        /// <summary>
        /// ボタンダウン画像
        /// </summary>
        private Image _DownImage = null;

        /// <summary>
        /// ボタンダウン画像
        /// </summary>
        private Image _UpImage = null;

        /// <summary>
        /// ボタンダウン画像
        /// </summary>
        private Image _DisableImage = null;

        /// <summary>
        /// ボタンダウンフラグ
        /// </summary>
        private bool _buttonDown = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public VisualButton()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ボタンクリックのリレー
        /// </summary>
        /// <param name="sender">送信元</param>
        /// <param name="e">イベント データを格納しているEventArgs</param>
        private void vbutton_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.ButtonClick != null)
                {
                    this.ButtonClick(sender, e);
                }
            }
            catch (Exception ex)
            {
                LOGGER.Warn(Resources.IDS_VBTN_CLICK_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// マウスダウンイベント
        /// </summary>
        /// <param name="sender">送信元</param>
        /// <param name="e">イベント データを格納しているMouseEventArgs</param>
        private void vbutton_MouseDown(object sender, MouseEventArgs e)
        {
            try
            {
                this._buttonDown = true;
                this.ChangeImage();
            }
            catch (Exception ex)
            {
                LOGGER.Warn(Resources.IDS_VBTN_MDOWN_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// マウスアップイベント
        /// </summary>
        /// <param name="sender">送信元</param>
        /// <param name="e">イベント データを格納しているMouseEventArgs</param>
        private void vbutton_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                this._buttonDown = false;
                this.ChangeImage();
            }
            catch (Exception ex)
            {
                LOGGER.Warn(Resources.IDS_VBTN_MUP_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// 有効/無効切替イベント
        /// </summary>
        /// <param name="sender">送信元</param>
        /// <param name="e">イベント データを格納しているEventArgs</param>
        private void VisualButton_EnabledChanged(object sender, EventArgs e)
        {
            try
            {
                this.ChangeImage();
            }
            catch (Exception ex)
            {
                LOGGER.Warn(Resources.IDS_VBTN_ENABLE_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// ロードイベント
        /// </summary>
        /// <param name="sender">送信元</param>
        /// <param name="e">イベント データを格納しているEventArgs</param>
        private void VisualButton_Load(object sender, EventArgs e)
        {
            try
            {
                this.ChangeImage();
            }
            catch (Exception ex)
            {
                LOGGER.Warn(Resources.IDS_VBTN_LOAD_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// 有効/無効切替
        /// </summary>
        private void ChangeImage()
        {
            if (this.Enabled)
            {
                //有効の場合はボタンの状態を確認する
                if (this._buttonDown)
                {
                    //ダウン中はボタンダウン画像を設定(nullでもそのまま設定)
                    this.vbutton.BackgroundImage = this._DownImage;
                }
                else
                {
                    //ダウン中以外の場合はボタンアップ画像を設定(nullでもそのまま設定)
                    this.vbutton.BackgroundImage = this._UpImage;
                }
            }
            else
            {
                //無効の場合は無効画像を設定(nullでもそのまま設定)
                this.vbutton.BackgroundImage = this._DisableImage;
            }
        }

        /// <summary>
        /// 画像読込
        /// </summary>
        /// <param name="path">画像パス</param>
        /// <returns>Image (読込めない場合はnullを返す)</returns>
        private Image LoadImage(string path)
        {
            try
            {
                //空文字の場合は無条件にnullを返す
                if (string.IsNullOrEmpty(path))
                {
                    LOGGER.Info(Resources.IDS_VBTN_LOAD_IMG_IS_NULL);
                    return null;
                }

#if CHG_20140616
                //画像読込
                LOGGER.Info(string.Format(Resources.IDS_VBTN_LOAD_IMG_OPEN, path));
                using (System.IO.FileStream st = System.IO.File.OpenRead(path))
#else
                //フルパスにする
                string exeDir = System.IO.Path.GetDirectoryName(Application.ExecutablePath);
                string fullpath = System.IO.Path.Combine(exeDir, path);

                //画像読込
                LOGGER.Info(string.Format(Resources.IDS_VBTN_LOAD_IMG_OPEN, fullpath));
                using (System.IO.FileStream st = System.IO.File.OpenRead(fullpath))
#endif
                {
                    Image result = Image.FromStream(st);
                    LOGGER.Info(Resources.IDS_VBTN_LOAD_IMG_SUCCESS);
                    return result;
                }
            }
            catch (Exception ex)
            {
                LOGGER.Warn(string.Format(Resources.IDS_VBTN_LOAD_IMG_EXCEPTION, path), ex);
            }
            //ここまで来たらnullで終了(例外の時のみ来る)
            return null;
        }

    }
}
