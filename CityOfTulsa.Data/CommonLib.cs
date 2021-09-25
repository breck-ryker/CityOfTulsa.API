using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CityOfTulsaData {
   
   static class CommonLib {
   }

   public static class StringExtensions {

      public static bool IsNumeric(this string text) {
         double result = 0;
         return IsNumeric(text, out result);
      }

      public static bool IsNumeric(this string text, out double result) {
         result = System.Double.MinValue;
         if (string.IsNullOrWhiteSpace(text)) {
            return false;
         }
         bool bResult = System.Double.TryParse(text, out result);
         return bResult;
      }

      public static string Replace(
         this string text,
         string target,
         string replaceWithText,
         StringComparison stringComparisonType
      ) {

         // Check inputs.
         if (text == null) {
            // Same as original .NET C# string.Replace behavior.
            throw new ArgumentNullException(nameof(text));
         }
         else if (text.Length == 0) {
            // Same as original .NET C# string.Replace behavior.
            return text;
         }
         else if (target == null) {
            // Same as original .NET C# string.Replace behavior.
            throw new ArgumentNullException(nameof(target));
         }
         else if (target.Length == 0) {
            // Same as original .NET C# string.Replace behavior.
            throw new ArgumentException("String cannot be of zero length.");
         }

         // Prepare string builder for storing the processed string.
         // Note: StringBuilder has a better performance than String by 30-40%.
         System.Text.StringBuilder sbResult = new System.Text.StringBuilder(text.Length);

         // Analyze the replacement: replace or remove.
         bool bReplacementIsNullOrEmpty = string.IsNullOrEmpty(replaceWithText);

         // Replace all values.
         const int nValueNotFound = -1;
         int nFoundAt;
         int nStartSearchIndex = 0;

         while ((nFoundAt = text.IndexOf(target, nStartSearchIndex, stringComparisonType)) != nValueNotFound) {

            // Append all characters until the found replacement.
            int nCharsUntilReplacement = (nFoundAt - nStartSearchIndex);
            bool nothingToAppend = (nCharsUntilReplacement == 0);

            if (!(nothingToAppend)) {
               sbResult.Append(text, nStartSearchIndex, nCharsUntilReplacement);
            }

            // Process the replacement.
            if (!(bReplacementIsNullOrEmpty)) {
               sbResult.Append(replaceWithText);
            }

            // Prepare start index for the next search.
            // This needed to prevent infinite loop, otherwise method always start search 
            // from the start of the string. For example: if an oldValue == "EXAMPLE", newValue == "example"
            // and comparisonType == "any ignore case" will conquer to replacing:
            // "EXAMPLE" to "example" to "example" to "example" … infinite loop.
            nStartSearchIndex = (nFoundAt + target.Length);

            if (nStartSearchIndex == text.Length) {
               // It is end of the input string: no more space for the next search.
               // The input string ends with a value that has already been replaced. 
               // Therefore, the string builder with the result is complete and no further action is required.
               return sbResult.ToString();
            }
         }

         // Append the last part to the result.
         int nCharsUntilStringEnd = (text.Length - nStartSearchIndex);

         sbResult.Append(text, nStartSearchIndex, nCharsUntilStringEnd);

         return sbResult.ToString();
      }

      public static bool ContainsControllerActionNames(this string url, string controllerName, string actionName) {

         if (string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(controllerName)) {
            return false;
         }

         string[] aryURLParts = url.ToLower().Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

         if (aryURLParts.Contains(controllerName.ToLower()) && (string.IsNullOrWhiteSpace(actionName) || aryURLParts.Contains(actionName.ToLower()))) {
            return true;
         }
         else {
            return false;
         }
      }

      public static bool ToBoolean(this string text) {

         if (string.IsNullOrWhiteSpace(text)) {
            return false;
         }

         switch (text.Trim().ToLower()) {
            case "1":
            case "y":
            case "t":
            case "yes":
               return true;
            case "0":
            case "n":
            case "f":
            case "no":
               return false;
            default:
               if (text.IsNumeric()) {
                  if (text.ToInteger() > 0) {
                     return true;
                  }
                  else {
                     return false;
                  }
               }
               return Convert.ToBoolean(text);
         }
      }

      public static string ToSpacedOutTextByCaps(this string text) {

         string sReturnValue = string.Concat(text.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

         char cCurrentChar, cNextChar, cPrevChar = Char.MinValue, cPrevNonWhitespaceChar = Char.MinValue;

         System.Text.StringBuilder sb = new System.Text.StringBuilder();

         char[] aryChars = sReturnValue.ToCharArray();

         for (int charIndex = 0; charIndex < aryChars.Length; charIndex++) {

            cCurrentChar = aryChars[charIndex];

            if ((charIndex + 1) < aryChars.Length) {
               cNextChar = aryChars[(charIndex + 1)];
            }
            else {
               cNextChar = Char.MinValue;
            }

            if (cPrevChar == Char.MinValue) {
               sb.Append(cCurrentChar.ToString());
            }
            else if (cPrevChar == ' ') {
               if (
                  Char.IsUpper(cCurrentChar)
                  &&
                  cPrevNonWhitespaceChar != Char.MinValue
                  &&
                  Char.IsUpper(cPrevNonWhitespaceChar)
                  &&
                  (
                     cNextChar == ' ' || Char.IsUpper(cNextChar)
                  )
               ) {
                  sb.Remove((sb.Length - 1), 1);
                  sb.Append(cCurrentChar.ToString());
               }
               else {
                  sb.Append(cCurrentChar.ToString());
               }
            }
            else {
               sb.Append(cCurrentChar.ToString());
            }

            if (cCurrentChar != ' ') {
               cPrevNonWhitespaceChar = cCurrentChar;
            }

            cPrevChar = cCurrentChar;
         }

         return sb.ToString();
      }

      public static string SetDelimiter(this string text, string delimiter) {

         return text.SetDelimiter(delimiter, false);
      }

      public static string SetDelimiter(this string text, string delimiter, bool includeComma) {

         if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(delimiter)) {
            return text;
         }

         text = text.Replace("<br/>", delimiter).Replace("<br />", delimiter).Replace("\r\n", delimiter);

         if (includeComma) {
            text = text.Replace(",", delimiter);
         }

         return text;
      }

      public static string FromBRTagsToNewlines(this string text) {
         if (text == null) {
            return null;
         }
         return text.Replace("<br/>", "\r\n").Replace("<br>", "\r\n").Replace("<br />", "\r\n").Replace("<br >", "\r\n");
      }

      public static string FromNewlinesToBRTags(this string text) {
         if (text == null) {
            return null;
         }
         return text.Replace("\r\n", "<br/>").Replace("\r", "<br/>").Replace("\n", "<br/>");
      }

      public static string ReplaceBRTags(this string text, string replacement) {
         if (text == null) {
            return null;
         }
         return text.Replace("<br/>", replacement).Replace("<br>", replacement).Replace("<br />", replacement).Replace("<br >", replacement);
      }

      public static string ReplaceNewlines(this string text, string replacement) {
         if (text == null) {
            return null;
         }
         return text.Replace("\r\n", replacement).Replace("\r", replacement).Replace("\n", replacement);
      }

      public static string Right(this string str, int length) {
         if (str == null) {
            return null;
         }
         if (str.Length < length) {
            return str;
         }
         return str.Substring(str.Length - length, length);
      }

      public static string TrimInsignificantTrailingZeros(
         this string text
      ) {
         if (string.IsNullOrWhiteSpace(text)) {
            return text;
         }

         int decimalPosition = text.IndexOf('.');

         if (decimalPosition >= 0) {
            int nFirstZeroPos = text.IndexOf('0', decimalPosition);
            if (nFirstZeroPos >= 0) {
               return text.TrimEnd().TrimEnd('0').Trim().TrimEnd('.').Trim();
            }
         }
         return text;
      }

      public static string TrimInsignificantTrailingZeros(
         this string text,
         bool bBlankIfZero
      ) {
         if (string.IsNullOrWhiteSpace(text)) {
            return text;
         }

         int decimalPosition = text.IndexOf('.');

         if (decimalPosition >= 0) {
            int nFirstZeroPos = text.IndexOf('0', decimalPosition);
            if (nFirstZeroPos >= 0) {
               text = text.TrimEnd().TrimEnd('0').Trim().TrimEnd('.').Trim();
            }
         }

         if (bBlankIfZero && text == "0") {
            text = "";
         }

         return text;
      }

      public static bool IsInteger(this string text) {
         return IsInteger(text, out _);
      }

      public static bool IsInteger(this string text, out int result) {
         result = System.Int32.MinValue;
         if (string.IsNullOrWhiteSpace(text)) {
            return false;
         }
         bool bResult = System.Int32.TryParse(text, out result);
         return bResult;
      }

      public static int ToInteger(this string text) {
         return ToInteger(text, System.Int32.MinValue, out _);
      }

      public static int ToInteger(this string text, int defaultValue) {
         return ToInteger(text, defaultValue, out _);
      }

      public static int ToInteger(this string text, int defaultValue, out int result) {
         bool bResult = System.Int32.TryParse((text ?? ""), out result);
         if (bResult) {
            return result;
         }
         else {
            return defaultValue;
         }
      }

      public static Int16 ToInteger16(this string text) {
         return ToInteger16(text, System.Int16.MinValue, out _);
      }

      public static Int16 ToInteger16(this string text, Int16 defaultValue) {
         return ToInteger16(text, defaultValue, out _);
      }

      public static Int16 ToInteger16(this string text, Int16 defaultValue, out Int16 result) {
         bool bResult = System.Int16.TryParse((text ?? ""), out result);
         if (bResult) {
            return result;
         }
         else {
            return defaultValue;
         }
      }

      public static double ToDouble(this string text) {
         return ToDouble(text, System.Double.MinValue, out _);
      }

      public static double ToDouble(this string text, double defaultValue) {
         return ToDouble(text, defaultValue, out _);
      }

      public static double ToDouble(this string text, double defaultValue, out double result) {
         bool bResult = System.Double.TryParse((text ?? ""), out result);
         if (bResult) {
            return result;
         }
         else {
            return defaultValue;
         }
      }

      public static bool IsPositiveInteger(this string text) {
         return IsPositiveInteger(text, out _);
      }

      public static bool IsPositiveInteger(this string text, out int result) {
         bool bResult = System.Int32.TryParse(text, out result);
         if (bResult && result > 0 && result != System.Int32.MaxValue) {  // sure, maxvalue is positive, but it's not a valid value here
            return true;
         }
         else {
            return false;
         }
      }

      public static bool IsNonNegativeInteger(this string text) {
         return IsNonNegativeInteger(text, out _);
      }

      public static bool IsNonNegativeInteger(this string text, out int result) {
         bool bResult = System.Int32.TryParse(text, out result);
         if (bResult && result >= 0 && result != System.Int32.MaxValue) {  // sure, maxvalue is positive, but it's not a valid value here
            return true;
         }
         else {
            return false;
         }
      }

      public static bool HasValues(this List<string> listStringItems) {

         if (listStringItems == null || listStringItems.Count == 0) {
            return false;
         }
         else {
            foreach (string s in listStringItems) {
               if (!(string.IsNullOrWhiteSpace(s))) {
                  return true;
               }
            }

            return false;
         }
      }
   }

   public static class IntegerExtensions {

      public static int RoundToNearest(this int val, int roundToTarget) {
         return ((int)Math.Round(val / (roundToTarget * 1.0), MidpointRounding.AwayFromZero)) * roundToTarget;
      }

      public static string ToThousandsSeparatorFormat(
         this int val
      ) {
         return String.Format("{0:N0}", val);
      }

      public static Int32 ToValidValue(this Int32 val) {

         if (val == System.Int32.MinValue) {
            return 0;
         }
         else {
            return val;
         }
      }

      public static Int32 ToValidNonNegativeValue(this Int32 val) {

         if (val < 0) {
            return 0;
         }
         else {
            return val;
         }
      }

      public static string ToValidValueText(this Int32 val) {

         if (val == System.Int32.MinValue) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }

      public static string ToValidNonNegativeValueText(this Int32 val) {

         if (val < 0) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }

      public static Int32 ToValidInteger(this Int32 val) {

         return val.ToValidInteger(0);
      }

      public static Int32 ToValidInteger(this Int32 val, int defaultValue) {

         if (val == System.Int32.MinValue || val == System.Int32.MaxValue) {
            return defaultValue;
         }
         else {
            return val;
         }
      }

      public static Int32 ToValidNonNegativeInteger(this Int32 val) {

         return val.ToValidNonNegativeInteger(0);
      }

      public static Int32 ToValidNonNegativeInteger(this Int32 val, int defaultValue) {

         if (!(val.IsValidInteger()) || val < 0) {
            return defaultValue;
         }
         else {
            return val;
         }
      }

      public static Int16 ToValidInteger(this Int16 val) {

         return val.ToValidInteger(0);
      }

      public static Int16 ToValidInteger(this Int16 val, Int16 defaultValue) {

         if (val == System.Int16.MinValue || val == System.Int16.MaxValue) {
            return defaultValue;
         }
         else {
            return val;
         }
      }

      public static Int16 ToValidNonNegativeInteger(this Int16 val) {

         return val.ToValidNonNegativeInteger(0);
      }

      public static Int16 ToValidNonNegativeInteger(this Int16 val, Int16 defaultValue) {

         if (!(val.IsValidInteger()) || val < 0) {
            return defaultValue;
         }
         else {
            return val;
         }
      }

      public static Int16 ToValidPositiveInteger(this Int16 val) {

         return val.ToValidPositiveInteger(0);
      }

      public static Int16 ToValidPositiveInteger(this Int16 val, Int16 defaultValue) {

         if (val <= 0 || !(val.IsValidInteger())) {
            return defaultValue;
         }
         else {
            return val;
         }
      }

      public static bool IsValidInteger(this Int16 val) {
         if (val == System.Int16.MinValue || val == System.Int16.MaxValue) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidInteger(this Int32 val) {
         if (val == System.Int32.MinValue || val == System.Int32.MaxValue) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidInteger(this Int64 val) {
         if (val == System.Int64.MinValue || val == System.Int64.MaxValue) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidNonNegativeInteger(this Int64 val) {
         if (val < 0 || !(val.IsValidInteger())) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidPositiveInteger(this Int64 val) {
         if (val <= 0 || !(val.IsValidInteger())) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidNonNegativeInteger(this Int32 val) {
         if (val < 0 || !(val.IsValidInteger())) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidPositiveInteger(this Int32 val) {
         if (val <= 0 || !(val.IsValidInteger())) {
            return false;
         }
         else {
            return true;
         }
      }

      public static string ToValidIntegerText(this Int32 val) {

         if (!(val.IsValidInteger())) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }

      public static string ToValidNonNegativeIntegerText(this Int32 val) {

         if (val < 0 || !(val.IsValidInteger())) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }

      public static string ToValidPositiveIntegerText(this Int32 val) {

         if (val <= 0 || !(val.IsValidInteger())) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }

      public static bool IsValidPositiveInteger(this Int16 val) {

         if (val <= 0 || !(val.IsValidInteger())) {
            return false;
         }
         else {
            return true;
         }
      }

      public static string ToValidIntegerText(this Int16 val) {

         if (!(val.IsValidInteger())) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }

      public static string ToValidNonNegativeIntegerText(this Int16 val) {

         if (val < 0 || !(val.IsValidInteger())) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }

      public static string ToValidPositiveIntegerText(this Int16 val) {

         if (val <= 0 || !(val.IsValidInteger())) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }
   }

   public static class DoubleExtensions {

      public static int ToValidInteger(this System.Double val) {

         if (val == System.Double.MinValue || val == System.Double.MaxValue) {
            return 0;
         }
         else {
            return Convert.ToInt32(Math.Round(val, 0, MidpointRounding.AwayFromZero));
         }
      }

      public static System.Double RoundTo(this System.Double val, int decimalPlaces) {

         return Math.Round(val, decimalPlaces, MidpointRounding.AwayFromZero);
      }

      public static bool IsValidValue(this System.Double val) {

         if (val == System.Double.MinValue || val == System.Double.MaxValue) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidNonNegativeValue(this System.Double val) {

         if (val.IsValidValue() && val >= 0) {
            return true;
         }
         else {
            return false;
         }
      }

      public static bool IsValidPositiveValue(this System.Double val) {

         if (val.IsValidValue() && val > 0) {
            return true;
         }
         else {
            return false;
         }
      }

      public static bool IsValidNonZeroValue(this System.Double val) {

         if (val.IsValidValue() && val != 0) {
            return true;
         }
         else {
            return false;
         }
      }

      public static System.Double ToValidValue(this System.Double val) {

         return val.ToValidValue(0);
      }

      public static System.Double ToValidValue(this System.Double val, double defaultValue) {

         if (val == System.Double.MinValue || val == System.Double.MaxValue) {
            return defaultValue;
         }
         else {
            return val;
         }
      }

      public static System.Double ToValidNonNegativeValue(this System.Double val) {

         return val.ToValidNonNegativeValue(0);
      }

      public static System.Double ToValidNonNegativeValue(this System.Double val, double defaultValue) {

         if (val < 0 || val == double.MaxValue) {
            return defaultValue;
         }
         else {
            return val;
         }
      }

      public static int ToValidNonNegativeInteger(this System.Double val) {

         if (val < 0) {
            return 0;
         }
         else {
            return Convert.ToInt32(Math.Round(val, 0, MidpointRounding.AwayFromZero));
         }
      }

      public static string ToValidValueText(this System.Double val) {

         if (val == System.Double.MinValue || val == System.Double.MaxValue) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }

      public static string ToValidNonNegativeValueText(this System.Double val) {

         if (val < 0) {
            return string.Empty;
         }
         else {
            return val.ToString();
         }
      }

      public static string ToValidNonNegativeIntegerText(this System.Double val) {

         int nValue = val.ToValidNonNegativeInteger();

         if (nValue < 0) {
            return string.Empty;
         }
         else {
            return nValue.ToString();
         }
      }

      public static string ToThousandsSeparatorFormat(
         this System.Double val
      ) {
         return String.Format("{0:N}", val);
      }

      public static string ToDecimalFormat(
         this double val,
         int decimalPlaces
      ) {
         return val.ToString("0." + new string('0', decimalPlaces));
      }

      public static string ToDecimalFormat(
         this double val,
         int decimalPlaces,
         bool includeThousandsSeparator
      ) {
         if (includeThousandsSeparator) {
            return String.Format("{0:N" + decimalPlaces.ToString() + "}", val);
         }
         else {
            return val.ToString("0." + new string('0', decimalPlaces));
         }
      }

      public static string RoundAndFormat(
         this System.Double val,
         int decimalPlaces,
         bool includeThousandsSeparator
      ) {

         if (includeThousandsSeparator) {
            return String.Format("{0:N" + decimalPlaces.ToString() + "}", Math.Round(val, decimalPlaces, MidpointRounding.AwayFromZero));
         }
         else {
            return Math.Round(val, decimalPlaces, MidpointRounding.AwayFromZero).ToString("0." + new string('0', decimalPlaces));
         }
      }

      public static string RoundAndFormat(
         this System.Double val,
         int decimalPlaces,
         bool includeThousandsSeparator,
         bool trimInsignificantTrailingZeros
      ) {

         string sReturnValue = null;

         if (includeThousandsSeparator) {
            sReturnValue = String.Format("{0:N" + decimalPlaces.ToString() + "}", Math.Round(val, decimalPlaces, MidpointRounding.AwayFromZero));
         }
         else {
            sReturnValue = Math.Round(val, decimalPlaces, MidpointRounding.AwayFromZero).ToString("0." + new string('0', decimalPlaces));
         }

         if (trimInsignificantTrailingZeros) {

            sReturnValue = sReturnValue.TrimInsignificantTrailingZeros();
         }

         return sReturnValue;
      }
   }

   public static class DateTimeExtensions {

      public static DateTime RoundUp(this System.DateTime dt, System.TimeSpan d) {
         // Examples:
         //var dt1 = RoundUp(DateTime.Parse("2011-08-11 16:59"), TimeSpan.FromMinutes(15));
         //// dt1 == {11/08/2011 17:00:00}

         //var dt2 = RoundUp(DateTime.Parse("2011-08-11 17:00"), TimeSpan.FromMinutes(15));
         //// dt2 == {11/08/2011 17:00:00}

         //var dt3 = RoundUp(DateTime.Parse("2011-08-11 17:01"), TimeSpan.FromMinutes(15));
         //// dt3 == {11/08/2011 17:15:00}
         ///
         return new System.DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
      }

      public static System.TimeSpan FromNow(this System.DateTime dt) {

         if (!(dt.IsValidValue())) {
            return System.TimeSpan.MinValue;
         }

         return (dt - System.DateTime.Now);
      }

      public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek) {
         int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
         return dt.AddDays(-1 * diff).Date;
      }

      public static DateTime FirstDayOfWeek(this DateTime dt) {
         var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
         var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
         if (diff < 0) {
            diff += 7;
         }
         return dt.AddDays(-diff).Date;
      }

      public static DateTime LastDayOfWeek(this DateTime dt) {
         return dt.FirstDayOfWeek().AddDays(6);
      }

      public static DateTime FirstDayOfMonth(this DateTime dt) {
         return new DateTime(dt.Year, dt.Month, 1);
      }

      public static DateTime LastDayOfMonth(this DateTime dt) {
         return dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);
      }

      public static DateTime FirstDayOfNextMonth(this DateTime dt) {
         return dt.FirstDayOfMonth().AddMonths(1);
      }

      public static string ToValidValueText(this DateTime dt, string formatText) {
         if (dt == System.DateTime.MinValue || dt == System.DateTime.MaxValue) {
            return string.Empty;
         }
         else {
            return dt.ToString(formatText);
         }
      }

      public static bool IsValidValue(this DateTime dt) {
         if (dt == System.DateTime.MinValue || dt == System.DateTime.MaxValue) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidValue(this TimeSpan ts) {
         if (ts == System.TimeSpan.MinValue || ts == System.TimeSpan.MaxValue) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidFutureValue(this DateTime dt) {
         if (dt == System.DateTime.MinValue || dt == System.DateTime.MaxValue || dt <= System.DateTime.Now) {
            return false;
         }
         else {
            return true;
         }
      }
   }

   public static class DateTimeOffsetExtensions {

      public static DateTimeOffset RoundUp(this System.DateTimeOffset dto, System.TimeSpan d) {
         // Examples:
         //var dt1 = RoundUp(DateTime.Parse("2011-08-11 16:59"), TimeSpan.FromMinutes(15));
         //// dt1 == {11/08/2011 17:00:00}

         //var dt2 = RoundUp(DateTime.Parse("2011-08-11 17:00"), TimeSpan.FromMinutes(15));
         //// dt2 == {11/08/2011 17:00:00}

         //var dt3 = RoundUp(DateTime.Parse("2011-08-11 17:01"), TimeSpan.FromMinutes(15));
         //// dt3 == {11/08/2011 17:15:00}
         ///
         return new System.DateTime((dto.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dto.DateTime.Kind);
      }

      public static System.TimeSpan FromNow(this System.DateTimeOffset dto) {

         if (!(dto.IsValidValue())) {
            return System.TimeSpan.MinValue;
         }

         return (dto - System.DateTime.Now);
      }

      public static DateTimeOffset StartOfWeek(this DateTimeOffset dto, DayOfWeek startOfWeek) {
         int diff = (7 + (dto.DayOfWeek - startOfWeek)) % 7;
         return dto.AddDays(-1 * diff).Date;
      }

      public static DateTimeOffset FirstDayOfWeek(this DateTimeOffset dto) {
         var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
         var diff = dto.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
         if (diff < 0) {
            diff += 7;
         }
         return dto.AddDays(-diff).Date;
      }

      public static string ToValidValueText(this DateTimeOffset dto, string formatText) {
         if (dto == System.DateTimeOffset.MinValue || dto == System.DateTimeOffset.MaxValue) {
            return string.Empty;
         }
         else {
            return dto.ToString(formatText);
         }
      }

      public static bool IsValidValue(this DateTimeOffset dto) {
         if (dto == System.DateTimeOffset.MinValue || dto == System.DateTimeOffset.MaxValue) {
            return false;
         }
         else {
            return true;
         }
      }

      public static bool IsValidFutureValue(this DateTimeOffset dto) {
         if (dto == System.DateTimeOffset.MinValue || dto == System.DateTimeOffset.MaxValue || dto <= System.DateTimeOffset.Now) {
            return false;
         }
         else {
            return true;
         }
      }
   }

   public static class Extensions {

      public static bool IsDefault<T>(this T value) where T : struct {
         bool bIsDefault = value.Equals(default(T));
         return bIsDefault;
      }

      public static bool IsEqualTo(this List<string> list1, List<string> list2) {
         return (list1.Count == list2.Count && list1.Intersect(list2).Count() == list1.Count);
      }
   }
}
