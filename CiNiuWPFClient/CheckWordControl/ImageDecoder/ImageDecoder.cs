using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace CheckWordControl
{
    public static class ImageDecoder
    {
        public static readonly DependencyProperty SourceProperty;
        public static string GetSource(Image image)
        {
            if (image != null)
            {
                return (string)image.GetValue(ImageDecoder.SourceProperty);
            }
            else
            {
                return "";
            }
        }
        public static void SetSource(Image image, string value)
        {
            if (image != null)
            {
                image.SetValue(ImageDecoder.SourceProperty, value);
            }
        }
        static ImageDecoder()
        {
            SourceProperty = DependencyProperty.RegisterAttached("Source", typeof(string), typeof(ImageDecoder), new PropertyMetadata(new PropertyChangedCallback(ImageDecoder.OnSourceWithSourceChanged)));
            ImageQueue.OnComplate += ImageQueue_OnComplate;
        }
        private static void ImageQueue_OnComplate(Image i, string u, ImageSource b)
        {
            string source = GetSource(i);
            if (source == u.ToString())
            {
                i.Source = b;
            }
        }
        private static void OnSourceWithSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ImageQueue.Queue((Image)o, (string)e.NewValue);
        }
    }
}
