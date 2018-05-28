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
    
    }
}
