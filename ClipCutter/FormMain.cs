using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Clipboard = System.Windows.Clipboard;

namespace ClipCutter
{
    public partial class FormMain : Form
    {
        /// <summary>
        /// ホットキー
        /// </summary>
        private HotKey hotKey;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FormMain()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ロード完了時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormMain_Load(object sender, EventArgs e)
        {
            //hotKey = new HotKey(ModKeys.CONTROL | ModKeys.SHIFT, Keys.V);
            hotKey = new HotKey(ModKeys.CONTROL, Keys.Q);
            hotKey.HotKeyPush += HotKey_HotKeyPush;
        }

        /// <summary>
        /// ホットキー押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotKey_HotKeyPush(object sender, EventArgs e)
        {
            try
            {
                // 現在のクリップボードの文字列を取得する
                var text = Clipboard.GetText();
                // 文字列がなければ何もしない
                if (string.IsNullOrEmpty(text))
                {
                    return;
                }

                // 改行コードで分割する
                var lines = text.Split(new[] { "\r\n" }, StringSplitOptions.None);
                // 最初の行
                var firstLine = lines[0];
                // リスト化
                List<string> lineList = new List<string>(lines);
                // 先頭行を削除
                lineList.RemoveAt(0);
                // 残りの文字列
                var restText = string.Join("\r\n", lineList.ToArray());

                // クリップボードに先頭行を設定する
                Clipboard.SetText(firstLine);
                // 少し待つ
                Thread.Sleep(50);
                // Ctrl+V を送信する
                SendKeys.SendWait("^(v)");

                // 残りの文字列をクリップボードに設定する
                Clipboard.SetText(restText);
            }
            catch (Exception ex)
            {
                // nothing to do
                MessageBox.Show("失敗: " + ex.Message);
            }
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ホットキーがあれば
            if (hotKey != null)
            {
                // 破棄
                hotKey.Dispose();
                hotKey = null;
            }
        }
    }
}
