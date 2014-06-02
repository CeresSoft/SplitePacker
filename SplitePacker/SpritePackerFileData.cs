//=============================================================================
// システム名称　　　： スプライトパッカー
// サブシステム名　　： 
// 機能名　　　　　　： 
// ソースファイル名　： SpritePackerFileData.cs
//-----------------------------------------------------------------------------
// 機能概要　　　　　： SpritePackerファイルデータ
//-----------------------------------------------------------------------------
// 改訂履歴    区分  改訂番号  社名)担当   内容
// 2014.06.02  新規  ----      CS)杉原
//=============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SplitePacker
{
    /// <summary>
    /// SpritePackerファイルデータ
    /// </summary>
    public class SpritePackerFileData
    {
        /// <summary>
        /// パック横幅
        /// </summary>
        public int width;

        /// <summary>
        /// パック縦幅
        /// </summary>
        public int height;

        /// <summary>
        /// パック画像ファイル名
        /// </summary>
        public string path;

        /// <summary>
        /// スプライト画像
        /// </summary>
        public List<string> splites = new List<string>();
    }
}
