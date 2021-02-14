using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

namespace EngBox
{
    /// <summary>
    /// Engineering input box where user can easily change engineering unit.
    /// </summary>
    /// 
    public enum MeasurementTypes  {Length, Area, Volume }
    public partial class EngBoxControl : UserControl, INotifyPropertyChanged
    {
        public const string __DEFAULTFORMAT = "G4";
        public const double __DEFAULTVALUE = 1d;
        public const double __LL = 0.1;
        public const double __HH = 1E5;

        private double _oldCurrentValue;

        public bool IsValueValid { get; private set; }

        /// <summary>
        /// Unit as Inline ObservableCollection
        /// </summary>
        public ObservableCollection<Inline> SuperUnit
        {
            get
            {
                return Utils.UnitsToSuperScripts(cmbUnit.Text);
            }
        }
        public ObservableCollection<Inline> ValueWithUnit
        {
            get
            {
                ObservableCollection<Inline> oc = SuperUnit;
                Inline num = new Run(this.CurrentValue.ToString(this.OutputFormat) + " ");
                oc.Insert(0, num);

                return oc;
            }
        }
        static EngBoxControl()
        {
            /*
            CurrentValueProperty = DependencyProperty.Register("CurrentValue", typeof(double), typeof(EngBoxControl),
                new FrameworkPropertyMetadata(__DEFAULTVALUE, new PropertyChangedCallback(OnCurrentValueChanged)));
            */

            UserInputChangedEvent = EventManager.RegisterRoutedEvent("UserInputChanged", RoutingStrategy.Direct,
                typeof(RoutedPropertyChangedEventHandler<double>), typeof(EngBoxControl));



        }
        public EngBoxControl()
        {
            InitializeComponent();
            cmbUnit.ItemsSource = Units;
            checkLimits();
            txtValue.IsEnabled = !IsReadOnly;
        }

        #region Routed Events
     
        public static readonly RoutedEvent UserInputChangedEvent;
        [Description("Raised when user succesfully change the input"), Category("EngBoxEvents")]
        public event RoutedPropertyChangedEventHandler<double> UserInputChanged
        {
            add { AddHandler(UserInputChangedEvent, value); }
            remove { RemoveHandler(UserInputChangedEvent, value); }
        }
        #endregion
        #region Units Dependency Property
        [Description("Observable collection of units"), Category("EngBoxMisc")]
        public ObservableCollection<UnitProperties> Units
        {
            get { return (ObservableCollection<UnitProperties>)GetValue(UnitsProperty); }
            set { SetValue(UnitsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Units.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitsProperty =
            DependencyProperty.Register("Units", typeof(ObservableCollection<UnitProperties>), typeof(EngBoxControl),
                new FrameworkPropertyMetadata(CreateEmptyUnits(), new PropertyChangedCallback(OnUnitsChanged)));

        private static void OnUnitsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = (EngBoxControl)sender;
            ec.Units = (ObservableCollection<UnitProperties>)e.NewValue;
            ec.cmbUnit.ItemsSource = ec.Units;
            ec.checkLL();
        }


        private static object CreateEmptyUnits()
        {
            ObservableCollection<UnitProperties> emptyUnits = new ObservableCollection<UnitProperties>();
            return emptyUnits;
        }
        /// <summary>
        /// TODO Later implement validation if necessary
        /// </summary>
        /// <param name="value">Units ObservableCollection value</param>
        /// <returns>True if valid</returns>
        private static bool IsUnitValid(object value)
        {
            return true;
        }
        #endregion
        #region Current Value Dependency property



        /// <summary>
        /// Value represented in current selected unit
        /// </summary>
        [Description("Value represented in currently selected unit"), Category("EngBoxCommon")]
        public double CurrentValue
        {
            get { return (double)GetValue(CurrentValueProperty); }
            set { SetValue(CurrentValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentValueProperty =
            DependencyProperty.Register("CurrentValue", typeof(double), typeof(EngBoxControl), new PropertyMetadata(0d, OnCurrentValueChanged),
                new ValidateValueCallback(ValidateCurrentValue));

        private static void OnCurrentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = d as EngBoxControl;
            ec.calculateSI();
        }

        private static bool ValidateCurrentValue(object value)
        {
            return true;
        }

        private static double DefaultCurrent()
        {
            return 0;
        }


        #endregion
        #region IsZeroAllowed Dependency Property

        [Description("Determines if zero SI value is allowed"), Category("EngBoxMisc")]
        public bool IsZeroAllowed
        {
            get { return (bool)GetValue(IsZeroAllowedProperty); }
            set { SetValue(IsZeroAllowedProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsZeroAllowed.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsZeroAllowedProperty =
            DependencyProperty.Register("IsZeroAllowed", typeof(bool), typeof(EngBoxControl), new PropertyMetadata(true));


        #endregion
        #region IsReadOnly Dependency Property
        [Description("Is Read Only. If enabled keyboard input is disabled and limits does not affect the value."), Category("EngBoxCommon")]

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsReadOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(EngBoxControl), new PropertyMetadata(false,
                new PropertyChangedCallback(OnReadOnlyChanged)));

        private static void OnReadOnlyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = (EngBoxControl)sender;
            ec.txtValue.IsEnabled = !(bool)e.NewValue;
        }
        #endregion
        #region SIValue

        [Description("Value in SI units"), Category("EngBoxCommon")]
        public double SIValue
        {
            get { return (double)GetValue(SIValueProperty); }
            set { SetValue(SIValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SIValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SIValueProperty =
            DependencyProperty.Register("SIValue", typeof(double), typeof(EngBoxControl), new PropertyMetadata(1d, OnSIValueChanged));

        private static void OnSIValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = d as EngBoxControl;
          

            double newValue = (double)e.NewValue;

            //Check if the new value != 0 for case ZeroIsNotAllowed
            if (newValue == 0 && ec.IsZeroAllowed == false)
                ec.SIValue = 1;
            ec.calculateValueFromSI();
            ec.checkLimits();

        }
        #endregion
        #region Delta Dependendcy Property
        [Description("Mouse wheel delta value. Increases or decreases current value by this coefficient"), Category("EngBoxCommon")]


        public double Delta
        {
            get { return (double)GetValue(DeltaProperty); }
            set { SetValue(DeltaProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Delta.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DeltaProperty =
            DependencyProperty.Register("Delta", typeof(double), typeof(EngBoxControl), new PropertyMetadata(0.05));


        #endregion
        #region Selected Index property

        [Description("Unit index"), Category("EngBoxCommon")]
        public int UnitIndex
        {
            get { return (int)GetValue(UnitIndexProperty); }
            set { SetValue(UnitIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitIndexProperty =
            DependencyProperty.Register("UnitIndex", typeof(int), typeof(EngBoxControl),
                new FrameworkPropertyMetadata(0, new PropertyChangedCallback(OnUnitIndexChanged)));

        private static void OnUnitIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = (EngBoxControl)sender;
            if (ec.Units.Count > (int)e.NewValue)
            {
                ec.cmbUnit.SelectedIndex = (int)e.NewValue;
            }
        }

        #endregion
        #region Output format Dependency Property
        [Description("Output format as string "), Category("EngBoxCommon")]



        public string OutputFormat
        {
            get { return (string)GetValue(OutputFormatProperty); }
            set { SetValue(OutputFormatProperty, value); }
        }



        // Using a DependencyProperty as the backing store for OutputFormat.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OutputFormatProperty =
            DependencyProperty.Register("OutputFormat", typeof(string), typeof(EngBoxControl), new PropertyMetadata(__DEFAULTFORMAT,
                new PropertyChangedCallback(OnOutputFormatChanged)));



        private static void OnOutputFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = (EngBoxControl)sender;
            ec.OutputFormat = (string)e.NewValue;
            Collaborator.NumericFormat = ec.OutputFormat;
        }
        #endregion
        #region Unit Type property
        [Description("Physical quantity of the value "), Category("EngBoxCommon")]
        public MeasurementTypes UnitType
        {
            get { return (MeasurementTypes)GetValue(UnitTypeProperty); }
            set { SetValue(UnitTypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UnitType.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UnitTypeProperty =
            DependencyProperty.Register("UnitType", typeof(MeasurementTypes), typeof(EngBoxControl), new PropertyMetadata(MeasurementTypes.Length));

        #endregion

        #region Limits




        public bool IsLLExceeded { get; private set; } = false;
        public bool IsHHExceeded { get; private set; } = false;

        [Description("Use LL limit"), Category("Limits")]
        public bool UseLL
        {
            get { return (bool)GetValue(UseLLProperty); }
            set { SetValue(UseLLProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseLL.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseLLProperty =
            DependencyProperty.Register("UseLL", typeof(bool), typeof(EngBoxControl), new PropertyMetadata(true, new PropertyChangedCallback(OnUseLLChanged)));

        private static void OnUseLLChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = (EngBoxControl)sender;
            ec.checkLL();

        }

        [Description("LL limit; value should be always > LL"), Category("Limits")]


        public double LL_Limit
        {
            get { return (double)GetValue(LL_LimitProperty); }
            set { SetValue(LL_LimitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LL_Limit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LL_LimitProperty =
            DependencyProperty.Register("LL_Limit", typeof(double), typeof(EngBoxControl), new PropertyMetadata(__LL,
                new PropertyChangedCallback(OnLL_LimitChanged)));

        private static void OnLL_LimitChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = (EngBoxControl)sender;
            ec.checkLL();
        }

        [Description("Use LL limit"), Category("Limits")]



        public bool UseHH
        {
            get { return (bool)GetValue(UseHHProperty); }
            set { SetValue(UseHHProperty, value); }
        }

        // Using a DependencyProperty as the backing store for UseHH.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty UseHHProperty =
            DependencyProperty.Register("UseHH", typeof(bool), typeof(EngBoxControl), new PropertyMetadata(true, new PropertyChangedCallback(OnUseHHChanged)));

        private static void OnUseHHChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = (EngBoxControl)sender;
            ec.checkHH();
        }

        [Description("Occurs when current SI value is above HH and use HH is set to true"), Category("Limits")]

        public double HH_Limit
        {
            get { return (double)GetValue(HH_LimitProperty); }
            set { SetValue(HH_LimitProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HH_Limit.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HH_LimitProperty =
            DependencyProperty.Register("HH_Limit", typeof(double), typeof(EngBoxControl), new PropertyMetadata(__HH, new PropertyChangedCallback(OnHH_LimitChanged)));

        private static void OnHH_LimitChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            EngBoxControl ec = (EngBoxControl)sender;
            ec.checkHH();
        }

        private void checkLimits()
        {
            checkLL();
            checkHH();
        }
        private void checkLL()
        {
            bool result;
            if (!this.UseLL || SIValue >= LL_Limit) result = false;
            else result = true;

            IsLLExceeded = result;
            OnPropertyChanged("IsLLExceeded");
        }
        private void checkHH()
        {
            bool result;
            if (!this.UseHH || SIValue <= HH_Limit) result = false;
            else result = true;

            IsHHExceeded = result;
            OnPropertyChanged("IsHHExceeded");
        }

        #endregion
        #region Notify Property changed interface
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string s)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(s));
        }
        #endregion
        /// <summary>
        /// Calculates new Current value from SI value
        /// </summary>
        private void calculateValueFromSI()
        {
            if (cmbUnit.SelectedIndex != -1)
            {
                int index = cmbUnit.SelectedIndex;

                double a = Units[index].A;
                double b = Units[index].B;

                if (a != 0)
                {
                    CurrentValue = (SIValue - b) / a;
                }
                else
                    CurrentValue = 0;

            }
            OnPropertyChanged("CurrentValue");
        }
        private void calculateSI()
        {
            if (cmbUnit != null && cmbUnit.SelectedIndex != -1)
            {
                int index = cmbUnit.SelectedIndex;

                double a = Units[index].A;
                double b = Units[index].B;

                SIValue = CurrentValue * a + b;

            }
        }

        private void cmbUnit_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            calculateValueFromSI();
        }

        private void txtValue_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            performValueCheck();
        }
        private bool testValidity()
        {
            double d;

            if (double.TryParse(txtValue.Text, out d))
            {
                if (d == 0 && this.IsZeroAllowed == false)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;

            }
        }

        private void performValueCheck()
        {
            if (!IsReadOnly)
                if (!IsValueValid || IsLLExceeded || IsHHExceeded)
                {

                    this.SIValue = Collaborator.OldValue;
                    calculateValueFromSI();
                }

        }
        private void raiseUserInputEvent()
        {
           
            
            RoutedPropertyChangedEventArgs<double> args =
                        new RoutedPropertyChangedEventArgs<double>(_oldCurrentValue, CurrentValue);
                    args.RoutedEvent = EngBoxControl.UserInputChangedEvent;
            RaiseEvent(args);
        }
        private void txtValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool result = testValidity();
            IsValueValid = result;
            OnPropertyChanged("IsValueValid");
            if (result == false) return;

            _oldCurrentValue = CurrentValue;

            double cv;

            double.TryParse(txtValue.Text, out cv);
            CurrentValue = cv;
            
            checkLimits();
            calculateSI();
            raiseUserInputEvent();
        }

        private void txtValue_GotFocus(object sender, RoutedEventArgs e)
        {
            Collaborator.OldValue = this.SIValue;
        }

        private void txtValue_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (this.IsReadOnly) return;
            // double oldValue;
            // if (!double.TryParse(txtValue.Text, out oldValue)) return;
            Collaborator.OldValue = this.SIValue;
            double newValue;
            if (e.Delta > 0)
            {
                newValue = (1 + Delta) * CurrentValue;
            }
            else
            {
                newValue = (1 - Delta) * CurrentValue;
            }
            CurrentValue = newValue;
            OnPropertyChanged("CurrentValue");
            performValueCheck();
        }

        
    }
}