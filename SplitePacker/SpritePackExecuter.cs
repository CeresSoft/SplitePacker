//=============================================================================
// システム名称　　　： スプライトパッカー
// サブシステム名　　： 
// 機能名　　　　　　： 
// ソースファイル名　： SpritePackExecuter.cs
//-----------------------------------------------------------------------------
// 機能概要　　　　　： 画像パック処理クラス
//-----------------------------------------------------------------------------
// 改訂履歴    区分  改訂番号  社名)担当   内容
// 2014.06.02  新規  ----      CS)杉原
//=============================================================================

using SplitePacker.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitePacker
{
    /// <summary>
    /// 画像パック処理クラス
    /// </summary>
    public class SpritePackExecuter
    {
        /// <summary>
        /// LOG4NETのロガー
        /// </summary>
        static log4net.ILog LOGGER = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// パック画像横幅
        /// </summary>
        public int Width= 0;

        /// <summary>
        /// パック画像縦幅
        /// </summary>
        public int Height = 0;

        /// <summary>
        /// パック画像保存パス
        /// </summary>
        public string Path = String.Empty;

        /// <summary>
        /// スプライト画像パスリスト
        /// </summary>
        public System.Collections.IList Items = null;

        /// <summary>
        /// バックグラウンドワーカー
        /// </summary>
        public BackgroundWorker BG = null;

        /// <summary>
        /// メッセージ出力
        /// </summary>
        /// <param name="message">表示メッセージ</param>
        /// <remarks>メッセージの内容をInfoログとして出力します</remarks>
        private void ReportProgress(string message)
        {
            LOGGER.Info(message);
            if (this.BG != null)
            {
                this.BG.ReportProgress(0, message);
            }
        }

        /// <summary>
        /// パック実行
        /// </summary>
       /// <returns>実行結果のエラーメッセージ（成功の場合は空文字を返す)</returns>
        public string exec()
        {
            //ビットマップ生成
            using (Bitmap bmp = new Bitmap(this.Width, this.Height))
            {
                //グラフィック生成
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    ImagePackResult ret = this.ImagePack(g);
                    switch (ret)
                    {
                        case ImagePackResult.INVALID_PATH:
                            return Resources.IDS_BG_PACK_INVALID_PATH;
                        case ImagePackResult.OVERFLOWS:
                            return Resources.IDS_BG_PACK_OVERFLOW;
                        default:
                            //何もしない(=成功)
                            break;
                    }
                }

                //画像保存メッセージ
                this.ReportProgress(Resources.IDS_BG_PACK_SAVE);

                //画像を保存
                bmp.Save(this.Path);
            }

            //正常終了
            return string.Empty;
        }

        /// <summary>
        /// 画像パック戻り値
        /// </summary>
        private enum ImagePackResult
        {
            /// <summary>
            /// 成功
            /// </summary>
            SUCCESS,

            /// <summary>
            /// 無効なパス
            /// </summary>
            INVALID_PATH,

            /// <summary>
            /// 画像が入りきらない
            /// </summary>
            OVERFLOWS,

        }

        /// <summary>
        /// 画像パック
        /// </summary>
        /// <param name="g">Graphics</param>
        /// <returns>ImagePackResult値</returns>
        private ImagePackResult ImagePack(Graphics g)
        {
            int x = 0;
            int y = 0;
            int maxH = 0;

            foreach (object obj in this.Items)
            {
                string path = obj as string;
                if (string.IsNullOrEmpty(path))
                {
                    //異常データ
                    return ImagePackResult.INVALID_PATH;
                }

                //ステータスバーに表示
                string str = string.Format(Resources.IDS_BG_PACK_FILE, System.IO.Path.GetFileName(path));
                this.ReportProgress(str);

                using (FileStream fs = File.OpenRead(path))
                {
                    using (Image img = Image.FromStream(fs))
                    {
                        int w = img.Size.Width;
                        int h = img.Size.Height;

                        //描写後の右下位置を計算
                        int nx = x + w;
                        int ny = y + h;

                        //横幅が入るかチェック
                        if (nx > this.Width)
                        {
                            //描写位置を改行位置にする
                            x = 0;
                            y = y + maxH;
                            //最大高さを初期化
                            maxH = 0;

                            //改行後の位置から再計算
                            nx = x + w;
                            ny = y + h;

                            //横幅が入らない場合はすでにエラー
                            if (nx > this.Width)
                            {
                                return ImagePackResult.OVERFLOWS;
                            }

                            //縦幅をチェック
                            if (ny > this.Height)
                            {
                                return ImagePackResult.OVERFLOWS;
                            }
                        }
                        else
                        {
                            //縦幅をチェック
                            if (ny > this.Height)
                            {
                                //入らない場合はエラー
                                return ImagePackResult.OVERFLOWS;
                            }
                        }

                        //最大高さを保存
                        maxH = Math.Max(maxH, h);

                        //描写
                        g.DrawImage(img, x, y);

                        //次の描写位置
                        x = nx;
                    }
                }
            }

            return ImagePackResult.SUCCESS;
        }

    }
}
