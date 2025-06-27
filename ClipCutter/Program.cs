using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipCutter
{
    internal static class Program
    {
        /// <summary>
        /// Mutex 名
        /// </summary>
        private static readonly string AppMutexName = "4c439d3c-9b29-463d-8f55-5dc36dbbaa4a";

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 新規作成フラグ
            bool createdNew = false;

            // Mutex 作成
            using (var mutex = new Mutex(true, AppMutexName, out createdNew))
            {
                // すでに作成されている場合
                if (!createdNew)
                {
                    // 
                    MessageBox.Show("すでに起動しています");
                    // 終了
                    return;
                }

                // メイン処理を初期化
                new MainClass();

                // アプリケーション実行
                Application.Run();
            }
        }
    }
}
