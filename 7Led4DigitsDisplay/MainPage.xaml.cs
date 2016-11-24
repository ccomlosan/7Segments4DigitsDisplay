using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using Windows.Devices.Gpio;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace _7Segments4DigitsDisplay {
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  public sealed partial class MainPage : Page {
    public MainPage() {
      this.InitializeComponent();

      if (initializeGpio()) {
        initializeTimer();
      }
    }

    private void initializeTimer() {
      timer_ = new DispatcherTimer();
      timer_.Interval = TimeSpan.FromSeconds(1);
      timer_.Tick += timerCall;
      timer_.Start();
    }


    private bool initializeGpio() {
      var gpio = GpioController.GetDefault();

      if (gpio == null) {
        textBlockStatus.Text = "Gpio initialization error";
        return false;
      }

      for (int i = 0; i < PINS_NR; i++) {

        pins_[i] = gpio.OpenPin(PINS[i]);
        pins_[i].SetDriveMode(GpioPinDriveMode.Output);
      }

      for (int i = 0; i < DIGITS; i++) {
        display_[i] = new LedDigitDisplay();
        display_[i].setPins(pins_, pins_[i + 8]);
        display_[i].clearDigit();
      }

      numberDisplay_ = new NumberLedDisplay(display_);
      numberDisplay_.start();

      textBlockStatus.Text = "Gpio initialized!";
      return true;
    }

    private void timerCall(object sender, object args) {
      /*foreach (var x in display_) {
        x.clearDigit();
      }
      //display_[digit].clearDigit();
      display_[digit].setDigitValue(val++);
      if (val > 10) {
        val = 0;
        digit++;
      }
      if (digit > 3) {
        digit = 0;
      }*/
      numberDisplay_.setDisplayNumber(numberToDisplay_++);
    }

    private void buttonExit_Click(object sender, RoutedEventArgs e) {
      if (numberDisplay_ != null) {
        numberDisplay_.stop();
      }
      Application.Current.Exit();
    }


    private const int DIGITS = 4;
    private const int PINS_NR = 12;
    //first 7 for segments, next for dp, last 4 for digit selection
    private readonly int[] PINS = { 2, 3, 4, 17, 27, 22, 10, 9, 11, 5, 6, 13 };
    private GpioPin[] pins_ = new GpioPin[PINS_NR];
    private LedDigitDisplay[] display_ = new LedDigitDisplay[DIGITS];
    private NumberLedDisplay numberDisplay_;
    private uint numberToDisplay_ = 0;
    private uint val = 0;
    private uint digit = 0;
    private DispatcherTimer timer_ = new DispatcherTimer();

  }
}
