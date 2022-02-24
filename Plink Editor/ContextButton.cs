using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Plink_Editor
{
    internal static class ContextButton
    {
        public static readonly DependencyProperty MenuProperty = DependencyProperty.RegisterAttached("Menu", typeof(ContextMenu), typeof(ContextButton), new PropertyMetadata(null, MenuChanged));

        public static ContextMenu? GetMenu(DependencyObject obj) => obj.GetValue(MenuProperty) as ContextMenu;

        public static void SetMenu(DependencyObject obj, object? value) => obj.SetValue(MenuProperty, value);

        private static void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button is null)
                return;
            var menu = GetMenu(button);
            if (menu is null)
                return;
            menu.Placement = PlacementMode.Bottom;
            menu.PlacementTarget = button;
            menu.DataContext = button.DataContext;
            menu.IsOpen = true;
        }

        private static void MenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                if (e.NewValue is not null && e.OldValue is null)
                    button.Click += Button_Click;
                else if (e.NewValue is null && e.OldValue is not null)
                    button.Click -= Button_Click;
            }
        }
    }
}