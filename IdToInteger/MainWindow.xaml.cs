using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace IdToInteger
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        enum ChangeStep
        {
            Wait,
            ToID,
            ToInteger,
        }
        private ChangeStep ChangeStatus = ChangeStep.ToInteger;
        Regex RegID = new Regex("[0-9a-zA-Z]{4}");
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void TextBox_ID_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ChangeStatus == ChangeStep.Wait) return;
            if (TextBox_ID.Text.Count() != 4)
            {
                ChangeStatus = ChangeStep.Wait;
                TextBox_Integer.Text = "";
                ChangeStatus = ChangeStep.ToInteger;
            }
            string valueID = TextBox_ID.Text;
            if (RegID.IsMatch(valueID))
            {
                ChangeStatus = ChangeStep.Wait;
                UInt32 valueInt = (UInt32)(valueID[0] * 0x1000000 + valueID[1] * 0x10000 + valueID[2] * 0x100 + valueID[3]);
                if (ComboBox_Type.SelectedItem == ComboBoxItem_Hex)
                {
                    TextBox_Integer.Text = String.Format("{0:X}", valueInt);
                }
                else if (ComboBox_Type.SelectedItem == ComboBoxItem_Dec)
                {
                    TextBox_Integer.Text = valueInt.ToString();
                }
                else
                {
                    TextBox_Integer.Text = "";
                }                
                ChangeStatus = ChangeStep.ToInteger;
            }
        }

        private void TextBox_Integer_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ChangeStatus == ChangeStep.Wait) return;
            try
            {
                UInt32 valueInt = 0;
                if (ComboBox_Type.SelectedItem == ComboBoxItem_Hex)
                {
                    valueInt = UInt32.Parse(TextBox_Integer.Text, System.Globalization.NumberStyles.HexNumber);
                }
                else if (ComboBox_Type.SelectedItem == ComboBoxItem_Dec)
                {
                    valueInt = UInt32.Parse(TextBox_Integer.Text);
                }
                else
                {
                    return;
                }
                
                string valueID = ((char)((valueInt & 0xff000000) >> 24)).ToString() + ((char)((valueInt & 0xff0000) >> 16)).ToString() + ((char)((valueInt & 0xff00) >> 8)).ToString() + ((char)((valueInt & 0xff))).ToString();
                if (RegID.IsMatch(valueID))
                {
                    ChangeStatus = ChangeStep.Wait;
                    TextBox_ID.Text = valueID;
                    ChangeStatus = ChangeStep.ToID;
                }
                else
                {
                    ChangeStatus = ChangeStep.Wait;
                    TextBox_ID.Text = "";
                    ChangeStatus = ChangeStep.ToID;
                }
            }
            catch
            {
                ChangeStatus = ChangeStep.Wait;
                TextBox_ID.Text = "";
                ChangeStatus = ChangeStep.ToID;
            }
        }

        private void ComboBox_Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (ChangeStatus)
            {
                case ChangeStep.Wait:
                    break;
                case ChangeStep.ToID:
                    TextBox_Integer_TextChanged(null, null);
                    break;
                case ChangeStep.ToInteger:
                    TextBox_ID_TextChanged(null, null);
                    break;
                default:
                    break;
            }
        }
    }
}
