using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GD_StampingMachine.UserControls
{
    /// <summary>
    /// StampingFontChangedUserControl.xaml 的互動邏輯
    /// </summary>
    public partial class StampingFontChangedUserControl : UserControl
    {
        public StampingFontChangedUserControl()
        {
            InitializeComponent();
        }




        Point StampingFontListStartMousePos;
        Point UnusedStampingFontStartMousePos;


        private void StampingFontListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StampingFontListStartMousePos = e.GetPosition(null);
        }

        private void UnusedStampingFontListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UnusedStampingFontStartMousePos = e.GetPosition(null);
        }

        private void StampingFontListBox_Drop(object sender, DragEventArgs e)
        {
            // This casts 'e.Data.GetData()' as a ListBoxItem and if it isn't null
            // then the code will "execute" sort of. basically, listItem will always be 
            // a ListBoxItem (atleast i think it will)
            if (e.Data.GetData(DataFormats.FileDrop) is ListBoxItem listItem)
            {
                StampingFontListBox.Items.Add(listItem);
            }
        }

        private void UnusedStampingFontListBox_Drop(object sender, DragEventArgs e)
        {
            // This casts 'e.Data.GetData()' as a ListBoxItem and if it isn't null
            // then the code will "execute" sort of. basically, listItem will always be 
            // a ListBoxItem (atleast i think it will)
            if (e.Data.GetData(DataFormats.FileDrop) is ListBoxItem listItem)
            {
                UnusedStampingFontListBox.Items.Add(listItem);
            }
        }

        private void StampingFontListBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point mPos = e.GetPosition(null);

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(mPos.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(mPos.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                try
                {
                    var selectedItem2 = (GD_StampingMachine.Model.StampingTypeModel)(sender as ListBox).SelectedItem;

                    selectedItem2.StampingTypeNumber = 0;
                    selectedItem2.StampingTypeString = "._.";
                    selectedItem2.StampingTypeUseCount = 0;

                    // This gets the selected item
                    //ListBoxItem selectedItem = (ListBoxItem)StampingFontListBox.SelectedItem;
                    // You need to remove it before adding it to another listbox.
                    // if  you dont, it throws an error (due to referencing between 2 listboxes)
                    //StampingFontListBox.Items.Remove(selectedItem);



                    // The actual dragdrop thingy
                    // DragDropEffects.Copy... i dont think this matters but oh well.
                    //DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, selectedItem), DragDropEffects.Copy);

                    // This code will check if the listboxitem is inside a ListBox or not.
                    // This will stop the ListBoxItem you dragged from vanishing if you dont
                    // Drop it inside a listbox (drop it in the titlebar or something lol)

                    // ListBoxItems are objects obviously, and objects are passed and moved by reference.
                    // Any change to an object affects every reference. 'selectedItem' is a reference
                    // To UnusedStampingFontListBox.SelectedItem, and they both will NEVER be different :)

                    /*  if (selectedItem.Parent == null)
                      {
                          StampingFontListBox.Items.Add(selectedItem);
                      }*/
                

                }
                catch { }
            }
        }

        private void UnusedStampingFontListBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point mPos = e.GetPosition(null);

            if (e.LeftButton == MouseButtonState.Pressed &&
                Math.Abs(mPos.X) > SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(mPos.Y) > SystemParameters.MinimumVerticalDragDistance)
            {
                try
                {
                    // This gets the selected item
                    ListBoxItem selectedItem = (ListBoxItem)UnusedStampingFontListBox.SelectedItem;
                    // You need to remove it before adding it to another listbox.
                    // if  you dont, it throws an error (due to referencing between 2 listboxes)
                    UnusedStampingFontListBox.Items.Remove(selectedItem);

                    // The actual dragdrop thingy
                    // DragDropEffects.Copy... i dont think this matters but oh well.
                    DragDrop.DoDragDrop(this, new DataObject(DataFormats.FileDrop, selectedItem), DragDropEffects.Copy);

                    // This code will check if the listboxitem is inside a ListBox or not.
                    // This will stop the ListBoxItem you dragged from vanishing if you dont
                    // Drop it inside a listbox (drop it in the titlebar or something lol)

                    // ListBoxItems are objects obviously, and objects are passed and moved by reference.
                    // Any change to an object affects every reference. 'selectedItem' is a reference
                    // To UnusedStampingFontListBox.SelectedItem, and they both will NEVER be different :)

                    if (selectedItem.Parent == null)
                    {
                        UnusedStampingFontListBox.Items.Add(selectedItem);
                    }
                }
                catch { }
            }
        }


    }
}
