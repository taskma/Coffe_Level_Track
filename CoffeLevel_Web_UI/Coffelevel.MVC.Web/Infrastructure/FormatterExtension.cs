using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using CoffeLevel.Data.Entities;
using System.Web.Mvc;

namespace CoffeLevel.Client.Web.Infrastructure
{


    public static class FormatterExtension
    {

     

        public static string GetControllerAndActionName()
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;
            string name = "";
            if (routeValues != null)
            {
                if (routeValues.ContainsKey("controller"))
                {
                    name += string.Concat(routeValues["controller"].ToString(), "/");
                }
                if (routeValues.ContainsKey("action"))
                {
                    name += routeValues["action"].ToString();
                }
            }
            return name;
        }
        public static HtmlString toRateValue(this decimal? s)
        {
            if (s.HasValue && s.Value > 0) return new HtmlString(string.Concat("% ", String.Format("{0:0.00}",s)));
            return new HtmlString("-");
        }

        public static HtmlString toTurkishYesNo(this bool? s)
        {
            if (s.HasValue && s.Value == false ) return new HtmlString("<span class='text-red'> Hayır </span>");
            return new HtmlString("<span class='text-green'> Evet </span>");
        }

        public static IEnumerable<SelectListItem> toYesNoList(this bool? value)
        {
            List<SelectListItem> typeList = new List<SelectListItem>();
            var selectYes = new SelectListItem { Value = "1", Text = "Evet" };
            var selectNo = new SelectListItem { Value = "0", Text = "Hayır" };

            if (value == null || value == true)
            {
                typeList.Add(selectYes);
                typeList.Add(selectNo);
            }
            else
            {
                typeList.Add(selectNo);
                typeList.Add(selectYes);
            }
            return typeList;
        }

        public static List<string> toDomainNames(this string str)
        {
            CryptoManager crypto = new CryptoManager();
            List<string> domainNames = new List<string>();
            var domains = str.Split(new string[] { "&%&" }, StringSplitOptions.None).ToList();
            if (domains.IsNullOrEmpty())
            {
                domainNames.Add(crypto.Decrypt(str));
            }
            else
            {
                foreach (var item in domains)
                {
                    domainNames.Add(crypto.Decrypt(item));
                }
            }
            return domainNames;
        }

        public static string toSafeString(this string s)
        {
            return s.IsNullOrEmpty() ?  "-" :  s;
        }

     
        

        public static string toAsciiValue(this string str)
        {
            return str.IsNullOrEmpty() ? "" : (Encoding.ASCII.GetBytes(str).Sum(s => s) * 1253).ToString();
        }
        

      

        public static string toEmailOrderId(this Guid s)
        {

            return s.ToString().Split('-').Last();
        }
        

        public static HtmlString toDepositoryName(this string s)
        {
            if (s.Contains("Test Deposu")) return new HtmlString((string.Concat(s, "<div class='smallRedText'>Test Deposu olduğu için, 'Bilgi Epostası' gönderilmeyecek !</div>")));
            return new HtmlString( s);
        }



        public static bool PasswordIsValid(this string pass)
        {
            if (pass.Length < 6) return false;
            byte[] pass_byte = Encoding.ASCII.GetBytes(pass);
            bool HasAlphaNumeric = false;
            bool HasNumeric = false;
            foreach (var item in pass_byte)
            {
                if ( (item >= 97 && item <= 122 ) || (item >= 65 && item <= 90 )  )
                {
                    HasAlphaNumeric = true;
                }
                else if ((item >= 48 && item <= 57))
                {
                    HasNumeric = true;
                }
            }

            return (HasNumeric && HasAlphaNumeric) ? true : false;
        }


        public static string GetDayTurkishName(this DateTime inDate)
        {
            string engName = inDate.DayOfWeek.ToString().ToLower();
            switch (engName)
            {
                case "sunday":
                    return "Pazar";
                case "monday":
                    return "Pazartesi";
                case "tuesday":
                    return "Salı";
                case "wednesday":
                    return "Çarşamba";
                case "thursday":
                    return "Perşembe";
                case "friday":
                    return "Cuma";
                case "saturday":
                    return "Cumartesi";
                default:
                    return "";
            }
        }

        public static string GetMonthTurkishName(this DateTime inDate)
        {
            int month = inDate.Month;
            return GetMonthName(month);
        }

        public static string GetMonthTurkishName(this int month)
        {
            return GetMonthName(month);
        }

        private static string GetMonthName(int month)
        {
            switch (month)
            {
                case 1:
                    return "Ocak";
                case 2:
                    return "Şubat";
                case 3:
                    return "Mart";
                case 4:
                    return "Nisan";
                case 5:
                    return "Mayıs";
                case 6:
                    return "Haziran";
                case 7:
                    return "Temmuz";
                case 8:
                    return "Ağustos";
                case 9:
                    return "Eylül";
                case 10:
                    return "Ekim";
                case 11:
                    return "Kasım";
                case 12:
                    return "Aralık";
                default:
                    return "";
            }
        }

        public static DateTime toSearchDate(this DateTime inDate, int addDay)
        {
            DateTime newDate = inDate.AddDays(addDay);
            return new DateTime(newDate.Year, newDate.Month, newDate.Day);
        }

        public static DateTime toYearSearchDate(this DateTime inDate)
        {
            return new DateTime(inDate.Year, 1, 1);
        }

        public static bool IsValidEmailAddress(this string s)
        {
            if (s.Contains(':')) return false;
            var regex = new Regex(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?");
            return regex.IsMatch(s);
        }

       

        public static string toAmount(this decimal? s)
        {
            if (s == null) return "-";
            return ((decimal)s).toAmount();
        }


        public static string toAmount(this decimal s)
        {
            string amount = s.ToString();
            if (amount.Contains("."))
            {
                string[] parts = amount.Split('.');
                amount = string.Concat(parts[0], ".", parts[1].Substring(0, parts[1].Length > 2 ? 2 : parts[1].Length));
                if (parts[1].Length == 1) amount += "0";

            }
            else
            {
                amount = string.Concat(amount, ".00");
            }
            if (amount.Length > 6) amount = amount.Substring(0, amount.Length - 6) + "," + amount.Substring(amount.Length - 6, 6);

            return string.Concat(amount, " TL");
        }

        public static bool IsValidDateString(this string s)
        {
            string[] dates = s.Split('/');
            if (dates.IsNullOrEmpty() || dates.Length != 3 || !dates[0].ToInteger().IsInRange(1, 31) || !dates[1].ToInteger().IsInRange(1, 12) || !dates[2].ToInteger().IsInRange(1900, 2200))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidMMYYYYDateString(this string s)
        {
            string[] dates = s.Split('/');
            if (dates.IsNullOrEmpty() || dates.Length != 2 || !dates[0].ToInteger().IsInRange(1, 12) || !dates[1].ToInteger().IsInRange(1900, 2200))
            {
                return false;
            }
            return true;
        }

        public static bool IsValidYYYYDateString(this string s)
        {
            string[] dates = s.Split('.');
            if (dates.IsNullOrEmpty() || dates.Length != 2 || !dates[1].ToInteger().IsInRange(1900, 2200))
            {
                return false;
            }
            return true;
        }

        public static string toDDMMYYYYString(this DateTime s)
        {
            return string.Join("/", s.Day.ToString(), s.Month.ToString(), s.Year.ToString());
        }

        public static string toMMYYYYString(this DateTime s)
        {
            return string.Join("/", s.Month.ToString(), s.Year.ToString());
        }

        public static string toYYYYString(this DateTime s)
        {
            return string.Concat( ".", s.Year.ToString());
        }

        public static string toMMDDYYYYString(this DateTime s)
        {
            return string.Join("/", s.Month.ToString(), s.Day.ToString(), s.Year.ToString()); 
        }

        public static DateTime toDateTimeFromMMDDYYYYString(this string s)
        {
            string[] dates = s.Split('/');
            if (dates.IsNullOrEmpty() || dates.Length != 3 || !dates[0].ToInteger().IsInRange(1, 12) || !dates[1].ToInteger().IsInRange(1, 31) || !dates[2].ToInteger().IsInRange(1900, 2200))
            {
                return DateTime.MinValue;
            }
            return  new DateTime(Convert.ToInt32(dates[2]), Convert.ToInt32(dates[0]), Convert.ToInt32(dates[1]));
        }

        public static DateTime toDateTimeFromDDMMYYYYString(this string s)
        {
            string[] dates = s.Split('/');
            if (dates.IsNullOrEmpty() || dates.Length != 3 || !dates[0].ToInteger().IsInRange(1, 31) || !dates[1].ToInteger().IsInRange(1, 12) || !dates[2].ToInteger().IsInRange(1900, 2200))
            {
                return DateTime.MinValue;
            }
            return new DateTime(Convert.ToInt32(dates[2]), Convert.ToInt32(dates[1]), Convert.ToInt32(dates[0]));
        }

        public static DateTime toDateTimeFromMMYYYYString(this string s)
        {
            string[] dates = s.Split('/');
            if (dates.IsNullOrEmpty() || dates.Length != 2 ||  !dates[0].ToInteger().IsInRange(1, 12) || !dates[1].ToInteger().IsInRange(1900, 2200))
            {
                return DateTime.MinValue;
            }
            return new DateTime(Convert.ToInt32(dates[1]), Convert.ToInt32(dates[0]), 1);
        }

        public static DateTime toDateTimeFromYYYYString(this string s)
        {
            string[] dates = s.Split('.');
            if (dates.IsNullOrEmpty() || dates.Length != 2 ||  !dates[1].ToInteger().IsInRange(1900, 2200))
            {
                return DateTime.MinValue;
            }
            return new DateTime(Convert.ToInt32(dates[1]), 1, 1);
        }


        public static string ProperCase(this string s)
        {
            s = s.ToLower();
            string sProper = String.Empty;

            char[] seps = new char[] { ' ' };
            foreach (string ss in s.Split(seps))
            {
                try
                {
                    sProper += char.ToUpper(ss[0]);
                    sProper +=
                    (ss.Substring(1, ss.Length - 1) + ' ');
                }
                catch { }
            }
            return sProper.ToSafeTrim();
        }

        public static string ProperCase(this string s, string culture, bool onlyFirstLetterIsUpper = true)
        {
            s = s.ToLower(CultureInfo.GetCultureInfo(culture));
            string sProper = String.Empty;

            if (onlyFirstLetterIsUpper)
            {
                try
                {
                    sProper += char.ToUpper(s[0], CultureInfo.GetCultureInfo(culture));
                    sProper += s.Substring(1, s.Length - 1) + ' ';
                }
                catch { }
            }
            else
            {
                char[] seps = new char[] { ' ' };
                foreach (string ss in s.Split(seps))
                {
                    try
                    {
                        sProper += char.ToUpper(ss[0]);
                        sProper +=
                        (ss.Substring(1, ss.Length - 1) + ' ');
                    }
                    catch { }
                }
            }
            return sProper.ToSafeTrim();
        }

        public static string ToCamelCase(this string target)
        {
            string[] splittedTarget = target.Split(' ', '-', '.');

            StringBuilder camelCaseSb = new StringBuilder();

            for (int i = 0; i < splittedTarget.Length; i++)
            {
                if (i == 0)
                {
                    if (splittedTarget.Length == 1)
                    {
                        camelCaseSb.Append(Char.ToLowerInvariant(splittedTarget[i][0]) + splittedTarget[i].Substring(1));
                    }
                    else
                    {
                        camelCaseSb.Append(splittedTarget[i].ToLower());
                    }
                }
                else
                {
                    camelCaseSb.Append(splittedTarget[i]);
                }
            }

            return camelCaseSb.ToString();
        }

        public static bool IsNullOrEmpty<T>(this T[] value)
        {
            if (value == null || value.Length == 0)
                return true;

            return false;
        }

        public static bool IsNullOrEmpty<T>(this List<T> value)
        {
            if (value == null || value.Count == 0)
                return true;

            return false;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
        {
            return value == null || !value.Any();
        }

        public static bool HasElements<T>(this IEnumerable<T> value)
        {
            if (value != null && value.Count() > 0)
                return true;
            return false;
        }

        public static bool IsInRange(this int value, int minValue, int maxValue)
        {
            if (value >= minValue && value <= maxValue)
                return true;
            return false;
        }

        public static short ToShort(this bool value)
        {
            return Convert.ToInt16(value);
        }

       

        public static bool ToBoolean(this short? value)
        {
            if (!value.HasValue)
                return false;
            else
                return value.Value == 0 ? false : true;
        }

        public static bool ToBoolean(this short value)
        {
            return value == 0 ? false : true;
        }

        public static bool ToBoolean(this string value)
        {
            if (String.IsNullOrEmpty(value))
                return false;
            if (value.ToLower() == "true" || value == "1" || value.ToLower().Trim() == "yes" || value.ToLower().Trim() == "y")
                return true;
            return false;
        }

        public static bool ToBoolean(this object value)
        {
            return value == null ? false : (bool)value;
        }

        public static bool ToBoolean(this object value, bool defaultValue)
        {
            if (value == null)
                return defaultValue;
            string strValue = value.ToString(string.Empty).ToLower();
            if (String.IsNullOrEmpty(strValue))
                return defaultValue;
            if (strValue.ToLower().Equals("true") || strValue.ToLower().Equals("1"))
                return true;
            else
                return false;
        }

        public static object ToEnum<EnumType>(this string value)
        {
            return Enum.Parse(typeof(EnumType), value);
        }

       

       

        public static string ToLower(this Enum value)
        {
            return value.ToString().ToLowerInvariant();
        }

        public const char DefaultArraySeperator = ',';

        public static string[] ToStringArray(this string value)
        {
            return ToStringArray(value, DefaultArraySeperator);
        }

        public static string[] ToStringArray(this string value, char seperator)
        {
            if (String.IsNullOrEmpty(value))
                return null;

            if (value.StartsWith(seperator.ToString()))
                value = value.Substring(1);
            if (value.EndsWith(seperator.ToString()))
                value = value.Substring(0, value.Length - 1);
            if (String.IsNullOrEmpty(value))
                return null;

            string[] array = new string[] { value };
            if (value.Contains(seperator))
                array = value.Split(new char[] { seperator });
            return array;
        }

        public static string ConcatStringArrayToString(this string[] stringList)
        {
            return ConcatStringArrayToString(stringList, DefaultArraySeperator);
        }

        public static string ConcatStringArrayToString(this string[] stringList, char seperator)
        {
            string concattedStringList = String.Empty;
            for (int mailIndex = 0; mailIndex < stringList.Length; mailIndex++)
                concattedStringList += stringList[mailIndex] + seperator;

            //Remove last seperator
            if (concattedStringList.Length > 0)
                concattedStringList = concattedStringList.Substring(0, concattedStringList.Length - 1);

            return concattedStringList;
        }

        public static DateTime ToDateTime(this int dateTime)
        {
            return new DateTime(dateTime.ToString().Substring(0, 4).ToInteger(), dateTime.ToString().Substring(4, 2).ToInteger(), dateTime.ToString().Substring(6, 2).ToInteger());
        }

        public static DateTime? ToDateTimeNullable(this int dateTime)
        {
            if (dateTime == -1)
                return null;
            return new DateTime(dateTime.ToString().Substring(0, 4).ToInteger(), dateTime.ToString().Substring(4, 2).ToInteger(), dateTime.ToString().Substring(6, 2).ToInteger());
        }

        public static DateTime ToDateTime(this object dateTime)
        {
            return ToDateTime(dateTime, DateTime.Now);
        }

        public static DateTime ToDateTime(this object dateTime, DateTime defaultValue)
        {
            if (dateTime == null)
            {
                return defaultValue;
            }
            else
            {
                return (DateTime)dateTime;
            }
        }

        public static DateTime ToDateTime(this object dateTime, string dateFormat)
        {
            return DateTime.ParseExact(dateTime.ToString(), dateFormat, CultureInfo.InvariantCulture);
        }


        public static string ToJsDate(this DateTime date)
        {
            if (date != DateTime.MinValue)
            {
                int year = date.Year;
                int month = date.Month;
                int day = date.Day;
                int second = date.Second;
                int minunte = date.Minute;
                int hour = date.Hour;
                int milisecond = date.Millisecond;
                string parameters = String.Join(",", year, month - 1, day, hour, minunte, second, milisecond);
                return String.Format(CultureInfo.InvariantCulture, "new Date({0})", parameters);
            }
            else
            {
                return String.Empty;
            }
        }

       

      

        public static int ToInteger(this string value)
        {
            int result = 0;
            Int32.TryParse(value, out result);
            return result;
        }

        public static long ToLong(this string value)
        {
            long result = 0;
            Int64.TryParse(value, out result);
            return result;
        }

        public static string[] ToStringArray(this object value)
        {
            if (value == null)
                return null;

            return (string[])value;
        }

        public static string ToString(this object value)
        {
            if (value == null)
                return null;

            return value.ToString();
        }

        public static string ToString(this bool value, string trueValue, string falseValue)
        {

            if (value)
                return trueValue;
            else
                return falseValue;
        }

        public static string ToString(this object value, string defaultValue)
        {
            if (value == null)
                return defaultValue;

            return value.ToString();
        }

        public static byte[] ToByteArray(this string value)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return encoding.GetBytes(value);
        }

       

       

       

        public static string ToTitleFormat(this string title)
        {
            return char.ToUpper(title[0]) + title.Substring(1);
        }

        public static string Substring(this string value, ref int startIndex, int length)
        {
            string subStringValue = value.Substring(startIndex, length);
            startIndex += length;
            return subStringValue.Trim();
        }

        public static string SubstringAndTrim(this string value, int startIndex, int length)
        {
            return value.Substring(startIndex, length).Trim();
        }

        /// <summary>
        /// Convert 1,25 string to 1.25 decimal
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Decimal ToDecimal(this string value)
        {
            return Decimal.Parse(value, NumberStyles.Float, new CultureInfo("tr-TR"));
        }

        public static Int32 ToInteger(this object value, int defaultValue)
        {
            int result = 0;
            bool conversionResult = Int32.TryParse(value.ToString(String.Empty), out result);
            if (!conversionResult)
                return defaultValue;
            else
                return result;
        }

        public static Int32 ToIntegerDateTime(this DateTime value)
        {
            return Convert.ToInt32(value.Year.ToString() + value.Month.ToString().PadLeft(2, '0') + value.Day.ToString().PadLeft(2, '0'));
        }

        public static Int64 ToLong(this object value, long defaultValue)
        {
            long result = 0;
            bool conversionResult = Int64.TryParse(value.ToString(String.Empty), out result);
            if (!conversionResult)
                return defaultValue;
            else
                return result;
        }

        public static decimal ToDecimal(this object value, decimal defaultValue)
        {
            decimal result = 0;
            bool conversionResult = decimal.TryParse(value.ToString(String.Empty), out result);
            if (!conversionResult)
                return defaultValue;
            else
                return result;
        }

      

        public static string ToShortString(this object value, int maxLength)
        {
            return value.ToShortString(maxLength, true);
        }

        public static string ToShortString(this object value, int maxLength, bool addThreePoint)
        {
            string returnString = String.Empty;
            if (value != null && !String.IsNullOrEmpty(value.ToString()))
            {
                if (value.ToString().TrimEnd().Length > maxLength)
                {
                    returnString = value.ToString().TrimEnd().Substring(0, maxLength) + ((addThreePoint) ? "..." : "");
                }
                else
                    returnString = value.ToString().TrimEnd();
            }

            return returnString;
        }

        public static void MarkAsChanged(this WebControl control)
        {
            control.ForeColor = Color.FromArgb(255, 102, 0);
            control.Attributes.Add("bochanged", "true");
        }

        public static void MarkAsChanged(this WebControl control, Color forecolor)
        {
            control.ForeColor = forecolor;
            control.Attributes.Add("bochanged", "true");
        }

        public static string ToSafeTrim(this string value, string defaultValue = "")
        {
            return String.IsNullOrEmpty(value) ? defaultValue : value.Trim();
        }

       

       

        public static string ToMultipleLineString(this string value, bool enableMultipleLine, int everyLineMaxLength)
        {
            if (enableMultipleLine)
            {
                while (value.Contains("  "))
                {
                    value = value.Replace("  ", " ");
                }
                value = value.TrimEnd().Replace(" ", "<br>").Replace("<br/>", "<br*>").Replace("/", "<br>").Replace("<br*>", "<br/>");
                string[] parts = value.TrimEnd().Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                for (int index = 0; index < parts.Length; index++)
                {
                    if (parts[index].Length > everyLineMaxLength + 1)
                    {
                        parts[index] = parts[index].Insert(everyLineMaxLength, "<br>");
                        parts[index] = parts[index].ToMultipleLineString(enableMultipleLine, everyLineMaxLength);
                    }
                }
                value = String.Join("<br>", parts);
            }
            return value;
        }

        public static string FormatWith(this string target, params object[] args)
        {
            if (String.IsNullOrEmpty(target))
            {
                return String.Empty;
            }

            return String.Format(CultureInfo.CurrentUICulture, target, args);
        }

        public static string[] GetParts(this string target, string seperator = " ")
        {
            if (String.IsNullOrEmpty(target))
            {
                return null;
            }

            return target.Split(new string[] { seperator }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> Split(this string target, int maxLength)
        {
            for (int index = 0; index < target.Length; index += maxLength)
            {
                yield return target.Substring(index, Math.Min(maxLength, target.Length - index));
            }
        }

      

        public static string ToImageTag(this string target)
        {
            if (String.IsNullOrEmpty(target))
            {
                return String.Empty;
            }

            return "<img src=\"{0}\" alt=\"\" />".FormatWith(VirtualPathUtility.ToAbsolute(target));
        }

        public static bool IsListItem(this RepeaterItemEventArgs e)
        {
            return e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem;
        }

        public static bool IsHeaderItem(this RepeaterItemEventArgs e)
        {
            return e.Item.ItemType == ListItemType.Header;
        }

        public static bool IsFooterItem(this RepeaterItemEventArgs e)
        {
            return e.Item.ItemType == ListItemType.Footer;
        }

        public static T FindControl<T>(this RepeaterItemEventArgs e, string id) where T : Control
        {
            return (T)e.Item.FindControl(id);
        }

        public static bool IsDataItem(this ListViewItemEventArgs e)
        {
            return e.Item.ItemType == ListViewItemType.DataItem;
        }

        public static bool IsEmptyItem(this ListViewItemEventArgs e)
        {
            return e.Item.ItemType == ListViewItemType.EmptyItem;
        }

        public static bool IsInsertItem(this ListViewItemEventArgs e)
        {
            return e.Item.ItemType == ListViewItemType.InsertItem;
        }

       

        public static void AddCssClass(this WebControl webControl, params string[] cssClass)
        {
            if (cssClass.IsNullOrEmpty())
            {
                return;
            }

            string currentCssClass = webControl.CssClass;

            foreach (string css in cssClass)
            {
                currentCssClass += " " + css;
            }

            webControl.CssClass = currentCssClass.ToSafeTrim();
        }

        public static void RemoveCssClass(this WebControl webControl, params string[] cssClass)
        {
            if (cssClass.IsNullOrEmpty())
            {
                return;
            }

            string currentCssClass = webControl.CssClass;

            foreach (string css in cssClass)
            {
                currentCssClass = currentCssClass.Replace(css, " ");
            }

            webControl.CssClass = currentCssClass.ToSafeTrim();
        }

        public static void Show(this HtmlControl htmlControl, string additionalStyle = null)
        {
            ShowHide(ref htmlControl, true, additionalStyle);
        }

        public static void Hide(this HtmlControl htmlControl, string additionalStyle = null)
        {
            ShowHide(ref htmlControl, false, additionalStyle);
        }

        public static void Show(this WebControl webControl, string additionalStyle = null)
        {
            ShowHide(ref webControl, true, additionalStyle);
        }

        public static void Hide(this WebControl webControl, string additionalStyle = null)
        {
            ShowHide(ref webControl, false, additionalStyle);
        }

        private static void ShowHide(ref WebControl webControl, bool show, string additionalStyle = null)
        {
            if (webControl == null)
            {
                return;
            }

            string currentStyle = webControl.Attributes["style"] ?? String.Empty;
            webControl.Attributes["style"] = ArrangeStyle(currentStyle, "display", show ? "block" : "none");
        }

        private static void ShowHide(ref HtmlControl htmlControl, bool show, string additionalStyle = null)
        {
            if (htmlControl == null)
            {
                return;
            }

            string currentStyle = htmlControl.Attributes["style"] ?? String.Empty;
            htmlControl.Attributes["style"] = ArrangeStyle(currentStyle, "display", show ? "block" : "none");
        }

        private static string ArrangeStyle(string currentStyle, string additionalStyle, string value)
        {
            List<string> styleList = new List<string>();

            if (!String.IsNullOrEmpty(currentStyle))
            {
                styleList = currentStyle
                                .Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries)
                                .Where(style => !style.Contains("display"))
                                .ToList();
            }

            styleList.Add(String.Concat(additionalStyle, ":", value));
            return String.Join(";", styleList.ToArray());
        }

        public static bool IsRelativeUrl(this string target)
        {
            if (Uri.IsWellFormedUriString(target, UriKind.RelativeOrAbsolute))
            {
                return Uri.IsWellFormedUriString(target, UriKind.Relative);
            }

            return false;
        }

     

        public static string FormatIban(this string iban)
        {
            if (!string.IsNullOrEmpty(iban) && iban.Length > 4)
            {
                iban = iban.Replace(" ", "");
                int division = iban.Length / 4;

                int remaining = iban.Length % 4;
                int index = 0;
                for (int i = 1; i <= division; i++)
                {
                    iban = iban.Insert((i * 4) + index, " ");
                    index = index + 1;
                }

                if (remaining == 0)
                {
                    iban = iban.TrimEnd();
                }
            }
            return iban;
        }

        public static string ToEnglishChars(this string title)
        {
            title = Regex.Replace(title, "ş", "s");
            title = Regex.Replace(title, "ı", "i");
            title = Regex.Replace(title, "ö", "o");
            title = Regex.Replace(title, "ü", "u");
            title = Regex.Replace(title, "ç", "c");
            title = Regex.Replace(title, "ğ", "g");
            title = Regex.Replace(title, "Ş", "S");
            title = Regex.Replace(title, "İ", "I");
            title = Regex.Replace(title, "Ö", "O");
            title = Regex.Replace(title, "Ü", "U");
            title = Regex.Replace(title, "Ç", "C");
            title = Regex.Replace(title, "Ğ", "G");
            return title;
        }

        public static void ClearCache(this HttpResponse httpResponse)
        {
            httpResponse.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            httpResponse.Cache.SetValidUntilExpires(false);
            httpResponse.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            httpResponse.Cache.SetCacheability(HttpCacheability.NoCache);
            httpResponse.Cache.SetNoStore();
        }

        public static void Width(this HtmlControl htmlControl, int width)
        {
            if (htmlControl == null)
            {
                return;
            }

            if (width == -1)
            {
                htmlControl.Style.Remove(HtmlTextWriterStyle.Width);
            }
            else
            {
                htmlControl.Style[HtmlTextWriterStyle.Width] = String.Concat(width, "px");
            }
        }

        public static void Width(this WebControl webControl, int width)
        {
            if (webControl == null)
            {
                return;
            }

            if (width == -1)
            {
                webControl.Style.Remove(HtmlTextWriterStyle.Width);
            }
            else
            {
                webControl.Width = new Unit(width);
            }
        }

        public static int Width(this HtmlControl htmlControl)
        {
            if (htmlControl == null)
            {
                return 0;
            }

            string width = htmlControl.Style[HtmlTextWriterStyle.Width].ToSafeTrim();

            if (width == "auto")
            {
                return -1;
            }

            return int.Parse(width.Replace("px", String.Empty));
        }

        public static int Width(this WebControl webControl)
        {
            if (webControl == null)
            {
                return 0;
            }

            return (int)webControl.Width.Value;
        }
    }
}