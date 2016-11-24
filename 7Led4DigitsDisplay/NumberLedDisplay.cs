using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7Segments4DigitsDisplay {
  /// <summary>
  /// 
  /// </summary>
  class NumberLedDisplay {

    public NumberLedDisplay(LedDigitDisplay[] displayDigits) {
      digits_ = (uint)displayDigits.Length;
      displayDigits_ = new LedDigitDisplay[digits_];
      numberDigits_ = new uint?[digits_];
      for (int i = 0; i < digits_; i++) {
        displayDigits_[i] = displayDigits[i];
        numberDigits_[i] = null;
      }

    }

    public void start() {
      if (task_ == null) {
        task_ = Task.Factory.StartNew(run, TaskCreationOptions.LongRunning);
      }
    }

    public void stop() {
      lock (lock_) {
        stopSignal_ = true;
      }
      task_.Wait();
    }

    private async void run() {
      bool shouldStop = false;
      uint?[] n = new uint?[digits_];
      do {
        lock (lock_) {
          shouldStop = stopSignal_;
          for (int i = 0; i < digits_; i++) {
            n[i] = numberDigits_[i];
          }
        }

        for (int i = 0; i < digits_; i++) {
          display(i, n[i]);
          await Task.Delay(10);
        }
        
      }
      while (shouldStop == false);
    }


    private void display(int i, uint? n) {
      displayDigits_[i].clearDigit();
      displayDigits_[i].setDigitValue(n);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    public void setDisplayNumber(uint? n) {
      uint i = 0;
      while (i < digits_) {
        if (n == null) {
          numberDigits_[i] = null;
        }
        else {
          numberDigits_[i] = n % 10;
          n = n / 10;
          if (n == 0) {
            n = null;
          }
        }
      }
    }



    private readonly uint digits_ = 0;
    private LedDigitDisplay[] displayDigits_;
    private uint?[] numberDigits_;
    private object lock_ = new object();
    private Task task_;
    private bool stopSignal_ = true;
  }
}
