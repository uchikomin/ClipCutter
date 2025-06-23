using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipCutter
{
    /// <summary>
    /// メイン処理クラス
    /// </summary>
    internal class MainClass
    {
        /// <summary>
        /// ホットキー
        /// </summary>
        private HotKey hotKey;

        /// <summary>
        /// トレイアイコン
        /// </summary>
        private NotifyIcon trayIcon;

        /// <summary>
        /// 終了時に確認するフラグ
        /// </summary>
        public bool ConfirmOnExit { set; get; } = false;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        internal MainClass()
        {
            // ホットキーの初期化
            InitHotKey();
            // トレイアイコンの初期化
            InitTrayIcon();
        }

        /// <summary>
        /// ロード完了時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitHotKey()
        {
            hotKey = new HotKey(ModKeys.CONTROL, Keys.Q);
            hotKey.HotKeyPush += (s, e) => { PushedHotKey(); };
        }

        /// <summary>
        /// ホットキー押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PushedHotKey()
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
                var remainText = string.Join("\r\n", lineList.ToArray());

                // クリップボードに先頭行を設定する
                Clipboard.SetText(firstLine);
                // 少し待つ
                Thread.Sleep(50);
                // Ctrl+V を送信する
                SendKeys.SendWait("^(v)");

                // 残りの文字列をクリップボードに設定する
                Clipboard.SetText(remainText);
            }
            catch (Exception ex)
            {
                // nothing to do
                MessageBox.Show("失敗: " + ex.Message);
            }
        }

        // 参考
        // https://qiita.com/kob58im/items/8bca7a87890a66f8a425

        /// <summary>
        /// トレイアイコンを初期化
        /// </summary>
        private void InitTrayIcon()
        {
            // トレイアイコンを生成
            trayIcon = new NotifyIcon();
            // トレイアイコンを初期化
            trayIcon.Icon = Properties.Resources.AppIcon;
            trayIcon.Visible = true;
            trayIcon.Text = "Clip Cutter";

            // コンテキストメニューを生成
            var menu = new ContextMenuStrip();
            // コンテキストメニューを初期化
            menu.Items.AddRange(new ToolStripMenuItem[]
            {
                new ToolStripMenuItem("設定(&S)", null, (s, e)=>{ OnSetting(); }, "設定"),
                new ToolStripMenuItem("終了(&X)", null, (s, e)=>{ OnExit(); }, "終了"),
            });

            // トレイアイコンをダブルクリックした場合
            trayIcon.DoubleClick += (s, e) => { OnSetting(); };
            // トレイアイコンのコンテキストメニューを設定
            trayIcon.ContextMenuStrip = menu;
        }

        /// <summary>
        /// 設定を開いたときの処理
        /// </summary>
        private void OnSetting()
        {
            // 設定画面
            var setting = new FormMain();
            // 設定画面を表示
            if (setting.ShowDialog() != DialogResult.OK)
            {
                return;
            }
        }

        /// <summary>
        /// 終了を選択肢たときの処理
        /// </summary>
        private void OnExit()
        {
            // 終了確認
            if (ConfirmOnExit && 
                MessageBox.Show("ClipCutterを終了しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            {
                return;
            }

            // トレイアイコンをクリア
            trayIcon.Visible = false;
            trayIcon.Dispose();
            trayIcon = null;

            // ホットキーをクリア
            hotKey.Dispose();
            hotKey = null;

            // アプリケーションを終了
            Application.Exit();
        }
    }
}
