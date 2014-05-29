using SplitePacker.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SplitePacker
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// 最低コマンドライン数
        /// </summary>
        private const int COMMAND_ARGS_MIN = 2;

        /// <summary>
        /// 最低コマンドライン数
        /// </summary>
        private const int COMMAND_ARGS_FILENAME = 1;

        /// <summary>
        /// ２のべき乗計算用
        /// </summary>
        private const int EXP_TWO = 2;

        /// <summary>
        /// 余り０
        /// </summary>
        private const int MOD_ZERO = 0;

        /// <summary>
        /// LOG4NETのロガー
        /// </summary>
        private static log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// 操作ファイル
        /// </summary>
        private string _documentPath = Settings.Default.PackDefault;

        /// <summary>
        /// 変更フラグ
        /// </summary>
        private bool _update = false;

        /// <summary>
        /// オリジナルのタイトル
        /// </summary>
        private string _originalTitle = string.Empty;

        /// <summary>
        /// バックグラウンド実行中フラグ
        /// </summary>
        protected bool isRun
        {
            get
            {
                return (!this.Enabled);
            }
            set
            {
                this.Enabled = !value;
            }
        }


        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// メッセージ表示
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        protected void ShowMessage(string message, Exception ex)
        {
            try
            {
                LOGGER.Warn(message, ex);
                MessageBox.Show(
                    this,
                    message+"("+ex.Message+")",
                    this._originalTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception)
            {
                //表示時の例外は握りつぶす
                LOGGER.Error(Resources.IDS_MAIN_MSG_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// メッセージ表示
        /// </summary>
        /// <param name="message"></param>
        protected void ShowMessage(string message)
        {
            try
            {
                LOGGER.Info(message);
                MessageBox.Show(
                    this,
                    message,
                    this._originalTitle,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                //表示時の例外は握りつぶす
                LOGGER.Error(Resources.IDS_MAIN_MSG_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// 作成クリック処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            try
            {
                LOGGER.Info(Resources.IDS_MAIN_CREATE_CLICK);

                //実行中判定
                if(this.isRun) {
                    //実行中なら何もしない
                    LOGGER.Info(Resources.IDS_MAIN_PACKKING);
                    return;
                }

                //入力チェック
                string path = this.textPath.Text;
                if (string.IsNullOrEmpty(path))
                {
                    this.ShowMessage(Resources.IDS_MAIN_START_PATH_EMPTY);
                    return;
                }
                ListBox.ObjectCollection items = this.listBox1.Items;
                int n = 0;
                if (items != null)
                {
                    n = items.Count;
                }
                if(n <= 0)
                {
                    this.ShowMessage(Resources.IDS_MAIN_START_IMAGE_EMPTY);
                    return;
                }

                //2の累乗判定
                int w = (int)this.numWidth.Value;
                int h = (int)this.numHeight.Value;
                bool bWidthNoTwoExp = !this.isTwoExp(w);
                bool bHeigthNoTwoExp = !this.isTwoExp(h);
                if (bWidthNoTwoExp || bHeigthNoTwoExp)
                {
                    string msg = Resources.IDS_MAIN_CREATE_NO_TWO_EXP_A;

                    if (!bWidthNoTwoExp)
                    {
                        //横幅は２の累乗なので縦幅がNG
                        msg = Resources.IDS_MAIN_CREATE_NO_TWO_EXP_H;
                    }

                    if (!bHeigthNoTwoExp)
                    {
                        //縦幅は２の累乗なので横幅がNG
                        msg = Resources.IDS_MAIN_CREATE_NO_TWO_EXP_W;
                    }

                    
                    
                    //横幅又は縦幅が２の累乗でない場合
                    DialogResult dr = MessageBox.Show(
                        this,
                        msg,
                        this._originalTitle,
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Question,
                        MessageBoxDefaultButton.Button2);
                    if (dr != System.Windows.Forms.DialogResult.OK)
                    {
                        return;
                    }
                }


                //バックグラウンド用引数生成
                SpritePackExecuter spe = new SpritePackExecuter();
                spe.Width = w;
                spe.Height = h;
                spe.Path = path;
                spe.Items = items;
                spe.BG = this.backgroundWorker1;

                //バックグラウンド開始
                this.backgroundWorker1.RunWorkerAsync(spe);

                //実行中に設定(バックグラウンド開始で例外が出た場合は実行中とならないように
                //バックグラウンド開始後に設定する)
                this.isRun = true;
            }
            catch (Exception ex)
            {
                this.ShowMessage(Resources.IDS_MAIN_START_EXCEPTION, ex);
                return;
            }
        }

        /// <summary>
        /// リストボックスDragEnter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_DragEnter(object sender, DragEventArgs e)
        {
            try
            {
                //沢山よばれる可能性があるのでログは出力しない
                //LOGGER.Info(Resources.IDS_MAIN_DRAG_ENETR);

                //実行中以外
                if(!this.isRun)
                {
                    //FileDrop型の場合
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        //受け入れる
                        e.Effect = DragDropEffects.All;
                        return;
                    }
                }

                //それ以外は受け入れない
                e.Effect = DragDropEffects.None;
            }
            catch(Exception ex)
            {
                //連続して発生する可能性があるので単に握りつぶす
                //this.ShowMessage(Resources.IDS_MAIN_DRAG_ENTER_EXCEPTION, ex);
                LOGGER.Warn(Resources.IDS_MAIN_DRAG_ENTER_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// リストボックスDrop処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBox1_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                LOGGER.Info(Resources.IDS_MAIN_DRAG_DROP);

                //実行中の場合
                if (this.isRun)
                {
                    //何もしない
                    LOGGER.Info(Resources.IDS_MAIN_PACKKING);
                    //エラーメッセージも邪魔なので表示しない
                    return;
                }

                //ドロップされたデータがFileDrop型か調べる
                if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    //異なる場合は何もしない
                    this.ShowMessage(Resources.IDS_MAIN_DROP_FORMAT_FAILED);
                    return;
                }

                //FileDropから文字列配列を取得
                string[] fileNames = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (fileNames == null)
                {
                    //取得できなかった場合は何もしない
                    return;
                }

                //リストボックス取得
                ListBox list = (ListBox)sender;

                //Dropされたファイルを追加する
                foreach (string fileName in fileNames)
                {
                    //追加時に例外が発生した場合何を追加しようとしたか分かるように先にログを出力
                    LOGGER.InfoFormat(Resources.IDS_MAIN_DROP_ADD, fileName);
                    list.Items.Add(fileName);
                }

                //タイトルアップデート
                this.UpdateTitle(true);
            }
            catch (Exception ex)
            {
                this.ShowMessage(Resources.IDS_MAIN_DRAG_DROP_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// バックグラウンド実行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                LOGGER.Info(Resources.IDS_BG_START);

                //引数取得
                SpritePackExecuter spe = e.Argument as SpritePackExecuter;
                if (spe == null)
                {
                    //失敗で終了
                    e.Result = Resources.IDS_BG_ARGS_EMPTY;
                    return;
                }

                //パック画像生成
                e.Result = spe.exec();
            }
            finally
            {
                //例外発生時もログ出力されるようfinallyに記述
                LOGGER.Info(Resources.IDS_BG_END);
            }
        }

        /// <summary>
        /// 画像パック完了
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                LOGGER.Info(Resources.IDS_MAIN_COMPLETED);

                //実行中を解除
                this.isRun = false;

                //ステータスバー初期化
                this.toolStripStatusLabel1.Text = string.Empty;

                //例外判定
                if(e.Error != null){
                    this.ShowMessage(Resources.IDS_MAIN_END_PROC_EXCEPTION, e.Error);
                    return;
                }

                //エラーメッセージ表示
                string resultMessage = e.Result as string;
                if (!string.IsNullOrEmpty(resultMessage))
                {
                    this.ShowMessage(resultMessage);
                    return;
                }

                //成功メッセージ表示
                this.ShowMessage(Resources.IDS_MAIN_END_SUCCESS);
            }
            catch (Exception ex)
            {
                //例外は握りつぶす
                this.ShowMessage(Resources.IDS_MAIN_END_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// ファイル選択ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSeletctFile_Click(object sender, EventArgs e)
        {
            try
            {
                LOGGER.Info(Resources.IDS_MAIN_SELECT_FILE_CLICK);

                //実行中の場合
                if (this.isRun)
                {
                    //何もしない
                    LOGGER.Info(Resources.IDS_MAIN_PACKKING);
                    return;
                }

                //出力ファイル選択
                using(SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.FileName = this.textPath.Text;
                    sfd.Filter = Settings.Default.ImageFilter;
                    sfd.FilterIndex = 1;
                    sfd.OverwritePrompt = true;

                    DialogResult ret = sfd.ShowDialog();
                    if(ret == DialogResult.OK)
                    {
                        //パス変更
                        this.textPath.Text = sfd.FileName;
                    }
                }

            }
            catch (Exception ex)
            {
                //例外は握りつぶす
                this.ShowMessage(Resources.IDS_MAIN_FILE_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// 初期化ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClear_Click(object sender, EventArgs e)
        {
            try
            {
                LOGGER.Info(Resources.IDS_MAIN_CLEAR_CLICK);

                //実行中の場合
                if (this.isRun)
                {
                    //何もしない
                    LOGGER.Info(Resources.IDS_MAIN_PACKKING);
                    return;
                }

                //アイテムクリアー
                this.listBox1.Items.Clear();

                //タイトルアップデート
                this.UpdateTitle(true);
            }
            catch (Exception ex)
            {
                //例外は握りつぶす
                this.ShowMessage(Resources.IDS_MAIN_CLEAR_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// バックグラウンドのProgressChange
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            try
            {
                //メッセージ取得
                string strMsg = string.Empty;
                object obj = e.UserState;
                if(obj != null)
                {
                    strMsg = obj.ToString();
                }
                //メッセージをステータスに表示
                this.toolStripStatusLabel1.Text = strMsg;
            }
            catch(Exception ex)
            {
                //例外は握りつぶす
                LOGGER.Warn(Resources.IDS_MAIN_PROGRESS_EXCEPTION, ex);
                //バックグラウンド実行中なのでメッセージは表示しない
            }

        }

        /// <summary>
        /// 保存ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonSave_Click(object sender, EventArgs e)
        {
            try
            {
                LOGGER.Info(Resources.IDS_MAIN_SELECT_SAVE_CLICK);

                //実行中の場合
                if (this.isRun)
                {
                    //何もしない
                    LOGGER.Info(Resources.IDS_MAIN_PACKKING);
                    return;
                }

                //出力パス
                string path = this._documentPath;

                //出力ファイル選択
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.FileName = path;
                    sfd.Filter = Settings.Default.PackFilter;
                    sfd.FilterIndex = 1;
                    sfd.OverwritePrompt = true;

                    DialogResult ret = sfd.ShowDialog();
                    if (ret != DialogResult.OK)
                    {
                        return;
                    }
                    path = sfd.FileName;
                }

                //パス無し判定
                if (string.IsNullOrEmpty(path))
                {
                    this.ShowMessage(Resources.IDS_MAIN_SAVE_PATH_EMPTY);
                    return;
                }

                //保存データ作成
                SpritePackerFileData df = new SpritePackerFileData();
                df.width = (int)this.numWidth.Value;
                df.height = (int)this.numHeight.Value;
                df.path = this.textPath.Text;

                ListBox.ObjectCollection items = this.listBox1.Items;
                foreach(object obj in items )
                {
                    df.splites.Add(obj.ToString());
                }

                //XML出力
                XmlSerializer serializer = new XmlSerializer(typeof(SpritePackerFileData));
                using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8))
                {
                    serializer.Serialize(sw, df);
                    sw.Close();
                }

                //成功したらドキュメントパスを変更
                this._documentPath = path;
                this._update = false;
                this.UpdateTitle();
            }
            catch(Exception ex)
            {
                this.ShowMessage(Resources.IDS_MAIN_SAVE_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// 読込ボタンクリック
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonLoad_Click(object sender, EventArgs e)
        {
            try
            {
                LOGGER.Info(Resources.IDS_MAIN_SELECT_LOAD_CLICK);

                //実行中の場合
                if (this.isRun)
                {
                    //何もしない
                    LOGGER.Info(Resources.IDS_MAIN_PACKKING);
                    return;
                }

                //変更されている場合は保存しなくて良いか確認する
                if (this._update)
                {
                    DialogResult dr = MessageBox.Show(
                        this,
                        Resources.IDS_MAIN_LOAD_NOT_SAVED,
                        this._originalTitle,
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button2);
                    if(dr != System.Windows.Forms.DialogResult.Yes)
                    {
                        //YES以外は終了
                        return;
                    }
                }

                //読込パス
                string path = this._documentPath;

                //ファイル選択
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.FileName = path;
                    ofd.Filter = Settings.Default.PackFilter;
                    ofd.FilterIndex = 1;

                    DialogResult ret = ofd.ShowDialog();
                    if (ret != DialogResult.OK)
                    {
                        return;
                    }
                    path = ofd.FileName;
                }

                this.Load(path);
            }
            catch (Exception ex)
            {
                this.ShowMessage(Resources.IDS_MAIN_LOAD_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// 読込処理
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void Load(string path)
        {
            //パス無し判定
            if (string.IsNullOrEmpty(path))
            {
                this.ShowMessage(Resources.IDS_MAIN_LOAD_PATH_EMPTY);
                return;
            }

            //XML読込
            SpritePackerFileData df = null;
            XmlSerializer serializer = new XmlSerializer(typeof(SpritePackerFileData));
            using (StreamReader sr = new StreamReader(path, Encoding.UTF8))
            {
                df = serializer.Deserialize(sr) as SpritePackerFileData;
                sr.Close();
            }

            if (df == null)
            {
                //読込失敗
                this.ShowMessage(
                    String.Format(Resources.IDS_MAIN_LOAD_FAILED, path));
                return;
            }

            //データ復元
            this.numWidth.Value = df.width;
            this.numHeight.Value = df.height;
            this.textPath.Text = df.path;

            ListBox.ObjectCollection items = this.listBox1.Items;
            items.Clear();
            if (df.splites != null)
            {
                foreach (string s in df.splites)
                {
                    items.Add(s);
                }
            }

            //成功したらドキュメントパスを変更
            this._documentPath = path;
            this.UpdateTitle(false);

            //終了
            return;
        }

        /// <summary>
        /// タイトル更新処理
        /// </summary>
        /// <param name="bUpdate"></param>
        private void UpdateTitle(bool bUpdate)
        {
            this._update = bUpdate;
            this.UpdateTitle();
        }

        /// <summary>
        /// タイトル更新処理
        /// </summary>
        private void UpdateTitle()
        {
            string sMark = string.Empty;
            if (this._update)
            {
                sMark = Resources.IDS_MAIN_TITLE_UPDATE_MARK;
            }

            String fname = Path.GetFileName(this._documentPath);

            this.Text = string.Format(Resources.IDS_MAIN_TITLE_FORMAT,
                fname,
                this._originalTitle,
                sMark);
        }

        /// <summary>
        /// 初期表示時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Shown(object sender, EventArgs e)
        {
            try
            {
                //オリジナルのタイトルを保存
                this._originalTitle = this.Text;

                //コマンドラインの処理
                string[] cmds = System.Environment.GetCommandLineArgs();
                if (cmds.GetLength(0) >= Form1.COMMAND_ARGS_MIN)
                {
                    //ファイルが指定されている場合は読込む
                    this.Load(cmds[Form1.COMMAND_ARGS_FILENAME]);
                }
                else
                {
                    //タイトルアップデート
                    this.UpdateTitle();
                }
            }
            catch (Exception ex)
            {
                LOGGER.Warn(Resources.IDS_MAIN_SHOWN_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// パス変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textPath_TextChanged(object sender, EventArgs e)
        {
            try
            {
                //タイトルアップデート
                this.UpdateTitle(true);
            }
            catch (Exception ex)
            {
                LOGGER.Warn(Resources.IDS_MAIN_PATH_CHANGE_EXCEPTION, ex);
            }
        }


        /// <summary>
        /// 横幅変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numWidth_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //タイトルアップデート
                this.UpdateTitle(true);
            }
            catch (Exception ex)
            {
                LOGGER.Warn(Resources.IDS_MAIN_WIDTH_CHANGE_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// 縦幅変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void numHeight_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                //タイトルアップデート
                this.UpdateTitle(true);
            }
            catch (Exception ex)
            {
                LOGGER.Warn(Resources.IDS_MAIN_HEIGHT_CHANGE_EXCEPTION, ex);
            }
        }

        /// <summary>
        /// 2の累乗判定
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private bool isTwoExp(int v)
        {
            int vv = v;
            while (vv >= Form1.EXP_TWO)
            {
                //２で割り切れるか
                int n = vv % Form1.EXP_TWO;
                if (n != Form1.MOD_ZERO)
                {
                    //割り切れない場合は２の累乗以外
                    return false;
                }
                //計算値更新
                vv = vv / Form1.EXP_TWO;
            }
            //ここまで来たら２の累乗
            return true;
        }

        /// <summary>
        /// フォームClosing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                //更新されていない場合は何もしない
                if (!this._update)
                {
                    return;
                }

                DialogResult dr = MessageBox.Show(
                    this,
                    Resources.IDS_MAIN_CLOSING_NO_SAVE,
                    this._originalTitle,
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Exclamation,
                    MessageBoxDefaultButton.Button2);
                if(dr == DialogResult.Yes)
                {
                    //YESならそのまま終了
                    return;
                }

                //NOならクローズを中断する
                e.Cancel = true;
            }
            catch (Exception ex)
            {
                this.ShowMessage(Resources.IDS_MAIN_CLOSING_EXCEPTION, ex);
            }
        }
    }
}
