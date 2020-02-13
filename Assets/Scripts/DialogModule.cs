using UnityEngine;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System;
 
public class DialogModule : MonoBehaviour {
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    // #elif UNITY_STANDALONE_OSX
    // [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern double show_message(string str);
    #endif
 
    public double ShowMessage(string Str) {
 
        #if UNITY_STANDALONE
        return show_message(Str);
        #else
        return -1;
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern double show_question(string str);
    #endif
 
    double ShowQuestion(string Str) {
 
        #if UNITY_STANDALONE
        return show_question(Str);
        #else
        return -1;
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern double show_error(string str, double abort);
    #endif
 
    double ShowError(string Str, double Abort) {
 
        #if UNITY_STANDALONE
        return show_error(Str, Abort);
        #else
        return -1;
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_string(string str, string def);
    #endif
 
    string GetString(string Str, string Def) {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_string(Str, Def));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_password(string str, string def);
    #endif
 
    string GetPassword(string Str, string Def) {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_password(Str, Def));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern double get_integer(string str, double def);
    #endif
 
    double GetInteger(string Str, double Def) {
 
        #if UNITY_STANDALONE
        return get_integer(Str, Def);
        #else
        return -1;
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern double get_passcode(string str, double def);
    #endif
 
    double GetPasscode(string Str, double Def) {
 
        #if UNITY_STANDALONE
        return get_passcode(Str, Def);
        #else
        return -1;
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_open_filename(string filter, string fname);
    #endif
 
    string GetOpenFileName(string Filter, string FName) {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_open_filename(Filter, FName));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_open_filename_ext(string filter, string fname, string dir, string title);
    #endif
 
    string GetOpenFileNameExt(string Filter, string FName, string Dir, string Title) {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_open_filename_ext(Filter, FName, Dir, Title));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_open_filenames(string filter, string fname);
    #endif
 
    string GetOpenFileNames(string Filter, string FName)
    {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_open_filenames(Filter, FName));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_open_filenames_ext(string filter, string fname, string dir, string title);
    #endif
 
    string GetOpenFileNamesExt(string Filter, string FName, string Dir, string Title)
    {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_open_filenames_ext(Filter, FName, Dir, Title));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_save_filename(string filter, string fname);
    #endif
 
    string GetSaveFileName(string Filter, string FName) {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_save_filename(Filter, FName));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_save_filename_ext(string filter, string fname, string dir, string title);
    #endif
 
    string GetSaveFileNameExt(string Filter, string FName, string Dir, string Title) {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_save_filename_ext(Filter, FName, Dir, Title));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_directory(string dname);
    #endif
 
    string GetDirectory(string DName) {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_directory(DName));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern IntPtr get_directory_alt(string capt, string root);
    #endif
 
    string GetDirectoryAlt(string Capt, string Root) {
 
        #if UNITY_STANDALONE
        return System.Runtime.InteropServices.Marshal.PtrToStringAnsi(get_directory_alt(Capt, Root));
        #else
        return "NaN";
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern double get_color(double defcol);
    #endif
 
    double GetColor(double DefCol) {
 
        #if UNITY_STANDALONE
        return get_color(DefCol);
        #else
        return -1;
        #endif
 
    }
 
    #if UNITY_STANDALONE_WIN
    [DllImport("DialogModule.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_OSX
    [DllImport("DialogModule.bundle", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #elif UNITY_STANDALONE_LINUX
    [DllImport("DialogModule.so", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
    #endif
 
    #if UNITY_STANDALONE
    private static extern double get_color_ext(double defcol, string title);
    #endif
 
    double GetColorExt(double DefCol, string Title)
    {
 
        #if UNITY_STANDALONE
        return get_color_ext(DefCol, Title);
        #else
        return -1;
        #endif
 
    }
 
    void Start() {
 
    }
 
    void Update() {
 
    }
}
 