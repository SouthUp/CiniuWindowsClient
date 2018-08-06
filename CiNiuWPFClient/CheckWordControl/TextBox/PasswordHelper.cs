using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CheckWordControl
{
    public static class PasswordHelper
    {
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached("Password",
            typeof(string), typeof(PasswordHelper),
            new FrameworkPropertyMetadata(string.Empty, OnPasswordPropertyChanged));

        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached("Attach",
            typeof(bool), typeof(PasswordHelper), new PropertyMetadata(false, Attach));

        private static readonly DependencyProperty IsUpdatingProperty =
           DependencyProperty.RegisterAttached("IsUpdating", typeof(bool),
           typeof(PasswordHelper));


        public static void SetAttach(DependencyObject dp, bool value)
        {
            dp.SetValue(AttachProperty, value);
        }

        public static bool GetAttach(DependencyObject dp)
        {
            return (bool)dp.GetValue(AttachProperty);
        }

        public static string GetPassword(DependencyObject dp)
        {
            return (string)dp.GetValue(PasswordProperty);
        }

        public static void SetPassword(DependencyObject dp, string value)
        {
            dp.SetValue(PasswordProperty, value);
        }

        private static bool GetIsUpdating(DependencyObject dp)
        {
            return (bool)dp.GetValue(IsUpdatingProperty);
        }

        private static void SetIsUpdating(DependencyObject dp, bool value)
        {
            dp.SetValue(IsUpdatingProperty, value);
        }

        private static void OnPasswordPropertyChanged(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            try
            {
                PasswordBox passwordBox = sender as PasswordBox;
                passwordBox.PasswordChanged -= PasswordChanged;

                if (!(bool)GetIsUpdating(passwordBox))
                {
                    passwordBox.Password = (string)e.NewValue;
                }
                passwordBox.PasswordChanged += PasswordChanged;
            }
            catch (Exception ex)
            { }
        }

        private static void Attach(DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            try
            {
                PasswordBox passwordBox = sender as PasswordBox;

                if (passwordBox == null)
                    return;

                if ((bool)e.OldValue)
                {
                    passwordBox.PasswordChanged -= PasswordChanged;
                }

                if ((bool)e.NewValue)
                {
                    passwordBox.PasswordChanged += PasswordChanged;
                }
            }
            catch (Exception ex)
            { }
        }

        private static void PasswordChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                PasswordBox passwordBox = sender as PasswordBox;
                SetIsUpdating(passwordBox, true);
                SetPassword(passwordBox, passwordBox.Password);
                SetIsUpdating(passwordBox, false);
            }
            catch (Exception ex)
            { }
        }

        #region ShowTapTip
        private static void OnIsShowTabTipChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != null && (bool)e.NewValue)
            {

            }
        }
        public static readonly DependencyProperty ShowTabTipProperty =
            DependencyProperty.RegisterAttached("ShowTabTip",
            typeof(bool), typeof(PasswordHelper),
            new FrameworkPropertyMetadata(false, OnIsShowTabTipChanged));

        public static bool GetShowTabTip(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowTabTipProperty);
        }

        public static void SetShowTabTip(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowTabTipProperty, value);
        }
        #endregion
    }
}
