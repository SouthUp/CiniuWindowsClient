using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CheckWordControl
{
    [StyleTypedProperty(Property = "WatermarkStyle", StyleTargetType = typeof(TextBlock))]
    public class WatermarkTextBox : TextBox
    {

        static WatermarkTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WatermarkTextBox), new FrameworkPropertyMetadata(typeof(WatermarkTextBox)));
        }


        public string Watermark
        {

            get { return (string)GetValue(WatermarkProperty); }
            set { SetValue(WatermarkProperty, value); }

        }

        public Style WatermarkStyle
        {

            get { return (Style)GetValue(WatermarkStyleProperty); }
            set { SetValue(WatermarkStyleProperty, value); }

        }


        public static Style GetWatermarkStyle(DependencyObject obj)
        {
            return (Style)obj.GetValue(WatermarkStyleProperty);
        }


        public static void SetWatermarkStyle(DependencyObject obj, Style value)
        {
            obj.SetValue(WatermarkStyleProperty, value);
        }

        public static readonly DependencyProperty WatermarkStyleProperty =
                DependencyProperty.RegisterAttached("WatermarkStyle", typeof(Style), typeof(WatermarkTextBox));



        public static string GetWatermark(DependencyObject obj)
        {
            return (string)obj.GetValue(WatermarkProperty);
        }

        public static void SetWatermark(DependencyObject obj, string value)
        {
            obj.SetValue(WatermarkProperty, value);
        }

        public static readonly DependencyProperty WatermarkProperty =
       DependencyProperty.RegisterAttached("Watermark", typeof(string), typeof(WatermarkTextBox),
       new FrameworkPropertyMetadata(OnWatermarkChanged));

        private static void OnWatermarkChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
        }
        #region 输入数据类型

        /// <summary>
        /// 整数
        /// </summary>
        public bool IsOnlyNumber
        {
            get { return (bool)GetValue(IsOnlyNumberProperty); }
            set { SetValue(IsOnlyNumberProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOnlyNumber.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOnlyNumberProperty =
            DependencyProperty.Register("IsOnlyNumber", typeof(bool), typeof(WatermarkTextBox), new FrameworkPropertyMetadata(OnIsOnlyNumberChanged));
        private static void OnIsOnlyNumberChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null)
            {
                WatermarkTextBox txtIsOnlyNumber = sender as WatermarkTextBox;
                txtIsOnlyNumber.PreviewKeyDown -= TxtIsOnlyNumber_PreviewKeyDown;
                txtIsOnlyNumber.PreviewKeyDown += TxtIsOnlyNumber_PreviewKeyDown;
            }
        }

        private static void TxtIsOnlyNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                WatermarkTextBox tb = sender as WatermarkTextBox;

                if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                      e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right)
                {
                    if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    if (e.Key != Key.Tab)
                        e.Handled = true;
                }
            }
            catch (Exception ex)
            { }
        }
        #endregion
    }
}
