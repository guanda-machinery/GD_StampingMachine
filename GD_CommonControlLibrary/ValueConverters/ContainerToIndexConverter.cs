﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GD_CommonControlLibrary
{
    public class ContainerToIndexConverter : BaseValueConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            DependencyObject container = value as ContentPresenter;
            if (container == null)
            {
                container = value as ContentControl;
            }

            //Finds the parent ItemsControl by looking up the visual tree
            var itemControls = (ItemsControl)FindParentOfType<ItemsControl>(container);
            //Gets the index of the container from the parent ItemsControl
            var index = itemControls.ItemContainerGenerator.IndexFromContainer(container);

            if (itemControls is ListBox ItemListBox)
            {
                if (container is ContentPresenter containerControl)
                {
                    var ListboxIndex = ItemListBox.Items.IndexOf(containerControl.DataContext);
                    if (ListboxIndex != -1)
                    {
                        return ListboxIndex;
                    }
                }
            }

            return index;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public DependencyObject FindParentOfType<T>(DependencyObject child) where T : DependencyObject
        {
            //We get the immediate parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null)
            {
                return null;
            }

            //check if the parent matches the type we're looking for
            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return FindParentOfType<T>(parentObject);
            }
        }

    }
}
