using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace DrawToolsLib {
    public static class FontConversions {
        public static string FontStyleToString(FontStyle value) {
            string result;

            try {
                result = (string)(new FontStyleConverter().ConvertToString(value));
            }
            catch (NotSupportedException) {
                result = "";
            }

            return result;
        }

        public static FontStyle FontStyleFromString(string value) {
            FontStyle result;

            try {
                result = (FontStyle)new FontStyleConverter().ConvertFromString((value));
            }
            catch (NotSupportedException) {
                result = FontStyles.Normal;
            }
            catch (FormatException) {
                result = FontStyles.Normal;
            }

            return result;
        }

        public static string FontWeightToString(FontWeight value) {
            string result;

            try {
                result = (string)(new FontWeightConverter().ConvertToString(value));
            }
            catch (NotSupportedException) {
                result = "";
            }

            return result;
        }

        public static FontWeight FontWeightFromString(string value) {
            FontWeight result;

            try {
                result = (FontWeight)new FontWeightConverter().ConvertFromString((value));
            }
            catch (NotSupportedException) {
                result = FontWeights.Normal;
            }
            catch (FormatException) {
                result = FontWeights.Normal;
            }

            return result;
        }

        public static string FontStretchToString(FontStretch value) {
            string result;

            try {
                result = (string)(new FontStretchConverter().ConvertToString(value));
            }
            catch (NotSupportedException) {
                result = "";
            }

            return result;
        }


        public static FontStretch FontStretchFromString(string value) {
            FontStretch result;

            try {
                result = (FontStretch)new FontStretchConverter().ConvertFromString((value));
            }
            catch (NotSupportedException) {
                result = FontStretches.Normal;
            }
            catch (FormatException) {
                result = FontStretches.Normal;
            }

            return result;
        }

    }
}