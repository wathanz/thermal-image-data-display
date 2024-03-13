using System;
using System.Windows;

namespace DemoAppNet.Views;
internal class ExceptionHandler {
    public static void TryCatchDisplayException(Action ac, Window window) {
        try {
            ac();
        }
        catch (Exception ex) {
            ShowMessage(ex, window);
        }
    }

    public static void ShowMessage(Exception ex, Window window) {
        var exceptionInfo = ex.ToString();
        if (window != null)
            MessageBox.Show(window, exceptionInfo);
        else
            MessageBox.Show(exceptionInfo);
    }

    public static void TryCatchDisplayException(Action ac) {
        TryCatchDisplayException(ac, null);
    }
}