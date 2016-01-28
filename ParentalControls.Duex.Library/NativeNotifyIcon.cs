using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParentalControls.Duex.Library
{
    /// <summary>
    /// A workaround to have a context menu show above the taskbar.
    /// </summary>
    public class NativeContextMenu : Control
    {
        /*
            case WM_COMMAND:
            {
            if(wParam >= IDM_CONTEXT_LINE && wParam <= IDM_CONTEXT_HELP) {
            sprintf(msg, "Clicked on %s", context_menu[wParam -IDM_CONTEXT_LINE]);
            MessageBox(hWnd, msg, "Popup Menu", MB_OK);
            }
            DefWindowProc(hWnd, message, wParam, lParam);
            * break;
            }
         */

        protected override void WndProc(ref Message m)
        {

            if (m.Msg == WindowsNativeMessages.WM_COMMAND)
            {
                Console.WriteLine("Clicked on {0}.", m.WParam);
            }

            base.WndProc(ref m);
        }

        List<NativeContextMenuItem> list = new List<NativeContextMenuItem>();

        public List<NativeContextMenuItem> List
        {
            get { return list; }
            set { list = value; }
        }

        [DllImport("user32.dll")]
        static extern IntPtr CreatePopupMenu();

        [DllImport("user32", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool InsertMenu(IntPtr hmenu, uint position, uint flags,
               uint item_id, [MarshalAs(UnmanagedType.LPTStr)]string item_text);

        [DllImport("user32.dll")]
        static extern bool InsertMenuItem(IntPtr hMenu, uint uItem, bool fByPosition,
           [In] ref MENUITEMINFO lpmii);

        [DllImport("user32.dll")]
        static extern bool TrackPopupMenu(IntPtr hMenu, uint uFlags, int x, int y,
           int nReserved, IntPtr hWnd, IntPtr prcRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool PostMessage(int hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        static extern bool DestroyMenu(IntPtr hMenu);

        [DllImport("user32.dll")]
        static extern bool GetMenuInfo(IntPtr hmenu, out MENUINFO lpcmi);

        public NativeContextMenu()
        {
            
        }

        public bool Show(Point p, NotifyIconShowFlags flags)
        {
            IntPtr menu = CreatePopupMenu();
            foreach (NativeContextMenuItem item in list)
                if (!InsertMenu(menu, (uint)item.Position, (uint)item.Flags, (uint)item.Position, item.Text))
                    return false;
            SetForegroundWindow(Handle);
            bool b = TrackPopupMenu(menu, (uint)(flags), p.X, p.Y, 0, Handle, IntPtr.Zero);
            PostMessage(WindowsNativeMessages.WM_NULL, 0, 0, 0);
            bool b2 = DestroyMenu(menu); // This just makes me think that it hides the menu automaticly.
            return b;
        }

        public bool Show(Point p)
        {
            IntPtr menu = CreatePopupMenu();
            foreach (NativeContextMenuItem item in list)
                if (!InsertMenu(menu, (uint)item.Position, (uint)item.Flags, (uint)item.Position, item.Text))
                    return false;
            SetForegroundWindow(Handle);
            MENUINFO menuinfo;
            bool r = GetMenuInfo(menu, out menuinfo);
            Console.WriteLine("Returned: {0} MENUINFO:{1}", r, menuinfo);
            bool b = TrackPopupMenu(menu, (uint)(NotifyIconShowFlags.TPM_LEFTBUTTON | NotifyIconShowFlags.TPM_LEFTALIGN), p.X, p.Y, 0, Handle, IntPtr.Zero);
            PostMessage(WindowsNativeMessages.WM_NULL, 0, 0, 0); 
            bool b2 = DestroyMenu(menu); // This just makes me think that it hides the menu automaticly.
            return b;
        }

    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MENUINFO
    {
        public UInt32 cbSize;
        public UInt32 fMask;
        public UInt32 dwStyle;
        public uint cyMax;
        public IntPtr hbrBack;
        public UInt32 dwContextHelpID;
        public UIntPtr dwMenuData;
    }

    public class WindowsNativeMessages
    {
        public const int WM_NULL = 0x0000;
        public const int WM_COMMAND = 0x0111;
    }

    public enum ContextMenuItemFlags : long
    {
        MF_BYCOMMAND = 0x00000000L,
        MF_BYPOSITION = 0x00000400L,

        MF_BITMAP = 0x00000004L,
        MF_CHECKED = 0x00000008L,
        MF_DISABLED = 0x00000002L,
        MF_ENABLED = 0x00000000L,
        MF_GRAYED = 0x00000001L,
        MF_MENUBARBREAK = 0x00000020L,
        MF_MENUBREAK = 0x00000040L,
        MF_OWNERDRAW = 0x00000100L,
        MF_POPUP = 0x00000010L,
        MF_SEPARATOR = 0x00000800L,
        MF_STRING = 0x00000000L,
        MF_UNCHECKED = 0x00000000L
    }

    public enum NotifyIconShowFlags : long
    {
        TPM_CENTERALIGN = 0x0004L,
        TPM_LEFTALIGN = 0x0000L,
        TPM_RIGHTALIGN = 0x0008L,

        TPM_BOTTOMALIGN = 0x0020L,
        TPM_TOPALIGN = 0x0000L,
        TPM_VCENTERALIGN = 0x0010L,

        TPM_NONOTIFY = 0x0080L,
        TPM_RETURNCMD = 0x0100L,

        TPM_LEFTBUTTON = 0x0000L,
        TPM_RIGHTBUTTON = 0x0002L,

        TPM_HORNEGANIMATION = 0x0800L,
        TPM_HORPOSANIMATION = 0x0400L,
        TPM_NOANIMATION = 0x4000L,
        TPM_VERNEGANIMATION = 0x2000L,
        TPM_VERPOSANIMATION = 0x1000L
    }

    public struct NativeContextMenuItem
    {
        public int Position;
        public int Flags;
        public string Text;

        public NativeContextMenuItem(int position, int flags, string text)
        {
            Position = position;
            Flags = flags;
            Text = text;
        }

        public NativeContextMenuItem(int position, ContextMenuItemFlags flags, string text)
        {
            Position = position;
            Flags = (int)flags;
            Text = text;
        }

        public NativeContextMenuItem(int position, string text)
        {
            Position = position;
            Flags = (int)ContextMenuItemFlags.MF_STRING;
            Text = text;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MENUITEMINFO
    {
        public uint cbSize;
        public uint fMask;
        public uint fType;
        public uint fState;
        public uint wID;
        public IntPtr hSubMenu;
        public IntPtr hbmpChecked;
        public IntPtr hbmpUnchecked;
        public IntPtr dwItemData;
        public string dwTypeData;
        public uint cch;
        public IntPtr hbmpItem;

        // return the size of the structure
        public static uint sizeOf
        {
            get { return (uint)Marshal.SizeOf(typeof(MENUITEMINFO)); }
        }
    }

    /// <summary>
    /// A workaround to always show an icon.
    /// </summary>
    public class NativeNotifyIcon : Control
    {

        #region P/Invoke

        public const Int32 WM_MYMESSAGE = 0x8000; //WM_APP
        public const Int32 NOTIFYICON_VERSION_4 = 0x4;

        //messages
        public const Int32 WM_CONTEXTMENU = 0x7B;
        public const Int32 NIN_BALLOONHIDE = 0x403;
        public const Int32 NIN_BALLOONSHOW = 0x402;
        public const Int32 NIN_BALLOONTIMEOUT = 0x404;
        public const Int32 NIN_BALLOONUSERCLICK = 0x405;
        public const Int32 NIN_KEYSELECT = 0x403;
        public const Int32 NIN_SELECT = 0x400;
        public const Int32 NIN_POPUPOPEN = 0x406;
        public const Int32 NIN_POPUPCLOSE = 0x408;

        public const Int32 NIIF_USER = 0x4;
        public const Int32 NIIF_NONE = 0x0;
        public const Int32 NIIF_INFO = 0x1;
        public const Int32 NIIF_WARNING = 0x2;
        public const Int32 NIIF_ERROR = 0x3;
        public const Int32 NIIF_LARGE_ICON = 0x20;

        public enum NotifyFlags
        {
            NIF_MESSAGE = 0x01, NIF_ICON = 0x02, NIF_TIP = 0x04, NIF_INFO = 0x10, NIF_STATE = 0x08,
            NIF_GUID = 0x20, NIF_SHOWTIP = 0x80
        }

        public enum NotifyCommand { NIM_ADD = 0x0, NIM_DELETE = 0x2, NIM_MODIFY = 0x1, NIM_SETVERSION = 0x4 }
        [StructLayout(LayoutKind.Sequential)]
        public struct NOTIFYICONDATA
        {
            public Int32 cbSize;
            public IntPtr hWnd;
            public Int32 uID;
            public NotifyFlags uFlags;
            public Int32 uCallbackMessage;
            public IntPtr hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public String szTip;
            public Int32 dwState;
            public Int32 dwStateMask;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public String szInfo;
            public Int32 uVersion;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            public String szInfoTitle;
            public Int32 dwInfoFlags;
            public Guid guidItem; //> IE 6
            public IntPtr hBalloonIcon;
        }

        [DllImport("shell32.dll")]
        protected static extern System.Int32 Shell_NotifyIcon(NotifyCommand cmd, ref NOTIFYICONDATA data);


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public Int32 left;
            public Int32 top;
            public Int32 right;
            public Int32 bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        protected struct NOTIFYICONIDENTIFIER
        {
            public Int32 cbSize;
            public IntPtr hWnd;
            public Int32 uID;
            public Guid guidItem;
        }

        //Works with Shell32.dll (version 6.1 or later)
        [DllImport("shell32.dll", SetLastError = true)]
        protected static extern int Shell_NotifyIconGetRect([In]ref NOTIFYICONIDENTIFIER identifier, [Out]out RECT iconLocation);

        #endregion

        public NativeNotifyIcon(IntPtr icon)
        {
            identifier = Guid.NewGuid();

            NOTIFYICONDATA data = new NOTIFYICONDATA();

            data.cbSize = Marshal.SizeOf(data);
            data.hWnd = this.Handle;
            data.guidItem = identifier;
            data.uCallbackMessage = WM_MYMESSAGE; //This is the message sent to our app
            data.hIcon = icon;
            data.szTip = "Test123";

            data.uFlags = NotifyFlags.NIF_ICON | NotifyFlags.NIF_GUID | NotifyFlags.NIF_MESSAGE | NotifyFlags.NIF_TIP |
                          NotifyFlags.NIF_SHOWTIP;

            Shell_NotifyIcon(NotifyCommand.NIM_ADD, ref data);

            data.uVersion = NOTIFYICON_VERSION_4;
            Shell_NotifyIcon(NotifyCommand.NIM_SETVERSION, ref data);
        }

        private Guid identifier;

        public Guid ID
        {
            get { return identifier; }
        }

        public new Point Location
        {
            get
            {
                RECT rect = new RECT();
                NOTIFYICONIDENTIFIER notifyIcon = new NOTIFYICONIDENTIFIER();

                notifyIcon.cbSize = Marshal.SizeOf(notifyIcon);
                //only guid is needed
                notifyIcon.guidItem = identifier;

                int hresult = Shell_NotifyIconGetRect(ref notifyIcon, out rect);
                if (hresult == 0)
                    return new Point(rect.left, rect.top);
                throw new Win32Exception("Shell_NotifyIconGetRect failed with code of \"" + hresult + "\"");
            }
        }

        public Rectangle Rectangle
        {
            get
            {
                RECT rect = new RECT();
                NOTIFYICONIDENTIFIER notifyIcon = new NOTIFYICONIDENTIFIER();

                notifyIcon.cbSize = Marshal.SizeOf(notifyIcon);
                //only guid is needed
                notifyIcon.guidItem = identifier;

                int hresult = Shell_NotifyIconGetRect(ref notifyIcon, out rect);
                if(hresult == 0)
                    return new Rectangle(rect.left, rect.top, rect.left - rect.right, rect.top - rect.bottom);
                throw new Win32Exception("Shell_NotifyIconGetRect failed with code of \""+hresult+"\"");
            }
        }

        ~NativeNotifyIcon()
        {
            Dispose();
        }

        public bool ShowBalloon(string text, string title, IntPtr icon)
        {
            NOTIFYICONDATA data;
            data = new NOTIFYICONDATA();

            data.cbSize = Marshal.SizeOf(data);
            data.guidItem = identifier;

            //Set custom icon for balloon or NIIF_NONE for no icon. You can use all the other 
            //NIIF_... for system icons
            data.dwInfoFlags = NIIF_USER;
            data.hBalloonIcon = icon;
            //text in balloon
            data.szInfo = text;
            //balloon title
            data.szInfoTitle = title;
            //set the flags to be modified
            data.uFlags = NotifyFlags.NIF_INFO | NotifyFlags.NIF_SHOWTIP | NotifyFlags.NIF_GUID;

            int result = Shell_NotifyIcon(NotifyCommand.NIM_MODIFY, ref data);
            return (result == 0);
        }

        protected override void Dispose(bool mau)
        {
            if (mau)
            {
                NOTIFYICONDATA data = new NOTIFYICONDATA();
                data.cbSize = Marshal.SizeOf(data);
                data.uFlags = NotifyFlags.NIF_GUID;
                data.guidItem = identifier;

                Shell_NotifyIcon(NotifyCommand.NIM_DELETE, ref data);
            }
            base.Dispose(mau);
        }

        protected virtual void Dispose()
        {
            base.Dispose(true);
        }

        public NativeContextMenu ContextMenuStrip
        {
            get;
            set;
        }

        protected override void WndProc(ref Message m)
        {
            
            if (m.Msg == WM_MYMESSAGE)
            {
                //(Int32)m.LParam & 0x0000FFFF get the low 2 bytes of LParam, we dont need the high ones. 
                //(Int32)m.WParam & 0x0000FFFF is the X coordinate and 
                //((Int32)m.WParam & 0xFFFF0000) >> 16 the Y
                switch ((Int32)m.LParam & 0x0000FFFF)
                {
                    case NIN_BALLOONHIDE:

                        break;
                    case NIN_BALLOONSHOW:

                        break;
                    case NIN_BALLOONTIMEOUT:

                        break;
                    case NIN_BALLOONUSERCLICK:
                        //user clicked on balloon

                        break;
                    case NIN_SELECT:
                        //user left click on icon

                        break;
                    case WM_CONTEXTMENU:
                        //user right click on icon
                        if (ContextMenuStrip != null)
                            Console.WriteLine(ContextMenuStrip.Show(Location));
                        break;

                    //get what mouse messages you want
                    //case WM_LBUTTONDOWN:
                    //....

                    default:

                        break;
                }
            }

            base.WndProc(ref m);
        }
    }

}
