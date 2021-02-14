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
using EngBox;

namespace Deploy
{

   
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _calculation = 1;
        private VolumeUnitsManager _volumeUnits = new VolumeUnitsManager();
        private LengthUnitManager _lengthUnits = new LengthUnitManager();
        public MainWindow()
        {
               InitializeComponent();

        }
        /// <summary>
        /// Sets units for all eng controls. Associate event also
        /// </summary>
        private void setUnits()
        {
        

            foreach (EngBoxControl item in Collaborator.FindVisualChildren<EngBoxControl>(this))
            {
                //assign units
                switch (item.UnitType)
                {

                    case MeasurementTypes.Length:
                        item.Units = _lengthUnits.LengthUnits;
                        break;
                    case MeasurementTypes.Area:
                        break;
                    case MeasurementTypes.Volume:
                        item.Units = _volumeUnits.VolumeUnits;
                        break;
                    default:
                        break;
                }

                //asign eventhandler to non read only 
                if (!item.IsReadOnly)
                {
                    item.UserInputChanged += EngInputUserChanged;
                }
            }
            
            
        }

        private void EngInputUserChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            generateResults();
        }

        /// <summary>
        /// Calculates the result
        /// </summary>
        private void generateResults()
        {
            //Calculate cylinder volume
            engVolume.SIValue = Math.PI * engDiameter.SIValue * engDiameter.SIValue / 4 * engHeight.SIValue;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            setUnits();
        }


        /// <summary>
        /// Demonstrates possibility how to create final protocol using EngBox control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnProtocol_Click(object sender, RoutedEventArgs e)
        {
            TextBlock tb = new TextBlock();

            tb.Inlines.Add(_calculation.ToString() + ". Cylinder with ");
            tb.Inlines.AddRange(engDiameter.ValueWithUnit);
            tb.Inlines.Add(" diameter and height of ");
            tb.Inlines.AddRange(engHeight.ValueWithUnit);
            tb.Inlines.Add(" has volume of ");
            tb.Inlines.AddRange(engVolume.ValueWithUnit);
            tb.Inlines.Add(".");


            //Reduce the superscript font size
           foreach (Inline item in tb.Inlines)
            {
                if (item.BaselineAlignment == BaselineAlignment.Superscript)
                {
                    item.FontSize = tb.FontSize * 0.8;
                }
            }
          
            stckProtocols.Children.Add(tb);
            _calculation++;

        }

        private void btnClearProtocol_Click(object sender, RoutedEventArgs e)
        {
            _calculation = 1;
            stckProtocols.Children.Clear();
        }
    }
}
