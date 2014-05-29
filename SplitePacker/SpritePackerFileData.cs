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
