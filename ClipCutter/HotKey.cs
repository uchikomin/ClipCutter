﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClipCutter
{
    // 参考
    // https://anis774.net/codevault/hotkey.html


    /// <summary>
    /// グローバルホットキーを登録するクラス。
    /// 使用後は必ずDisposeすること。
    /// </summary>
    public class HotKey : IDisposable
    {
        HotKeyForm form;
        /// <summary>
        /// ホットキーが押されると発生する。
        /// </summary>
        public event EventHandler HotKeyPush;

        /// <summary>
        /// ホットキーを指定して初期化する。
        /// 使用後は必ずDisposeすること。
        /// </summary>
        /// <param name="modKey">修飾キー</param>
        /// <param name="key">キー</param>
        public HotKey(ModKeys modKey, Keys key)
        {
            form = new HotKeyForm(modKey, key, raiseHotKeyPush);
        }

        /// <summary>
        /// イベント発動
        /// </summary>
        private void raiseHotKeyPush()
        {
            if (HotKeyPush != null)
            {
                HotKeyPush(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            form.Dispose();
        }

        private class HotKeyForm : Form
        {
            [DllImport("user32.dll")]
            extern static int RegisterHotKey(IntPtr HWnd, int ID, ModKeys MOD_KEY, Keys KEY);

            [DllImport("user32.dll")]
            extern static int UnregisterHotKey(IntPtr HWnd, int ID);

            /// <summary>
            /// ホットキーの WndMessage
            /// </summary>
            const int WM_HOTKEY = 0x0312;

            /// <summary>
            /// ホットキーのID
            /// </summary>
            readonly int id;

            /// <summary>
            /// ホットキー押下時の処理
            /// </summary>
            readonly ThreadStart proc;


            public HotKeyForm(ModKeys modKey, Keys key, ThreadStart proc)
            {
                this.proc = proc;
                for (int i = 0x0000; i <= 0xbfff; i++)
                {
                    if (RegisterHotKey(this.Handle, i, modKey, key) != 0)
                    {
                        id = i;
                        break;
                    }
                }
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                if (m.Msg == WM_HOTKEY)
                {
                    if ((int)m.WParam == id)
                    {
                        proc();
                    }
                }
            }

            protected override void Dispose(bool disposing)
            {
                UnregisterHotKey(this.Handle, id);
                base.Dispose(disposing);
            }
        }
    }

    /// <summary>
    /// HotKeyクラスの初期化時に指定する修飾キー
    /// </summary>
    public enum ModKeys : int
    {
        /// <summary>
        /// ALT キー
        /// </summary>
        ALT     = 0x0001,

        /// <summary>
        /// コントロールキー
        /// </summary>
        CONTROL = 0x0002,

        /// <summary>
        /// SHIFT キー
        /// </summary>
        SHIFT   = 0x0004,
    }
}
