using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Devices.Gpio;

namespace _7Segments4DigitsDisplay
{
    class LedDigitDisplay
    {
        public enum Segments { A = 0, B = 1, C = 2, D = 3, E = 4, F = 5, G = 6};
        public LedDigitDisplay()
        {

        }

        public int digits { get; private set; }

        public void setPins(GpioPin []pins, GpioPin sel)
        {
            if (pins.Length >= SEGMENTS)
            {
                uint i = 0;
                while(i < SEGMENTS)
                {
                    segments_[i] = pins[i++];
                }
                if (pins.Length >= SEGMENTS + 1)
                {
                    dp_ = pins[i++];
                    selectionPin_ = sel;
                }
            }
        }

        public void setDigitValue(uint val)
        {
            selectionPin_.Write(GpioPinValue.Low);

            if (val > DIGITS)
            {
                val = DIGITS + 1;
            }
            for (int i = 0; i < SEGMENTS; i++)
            {
                if (segmentsConfig_[val, i] == 1)
                {
                    segments_[i].Write(GpioPinValue.Low);
                }
                else
                {
                    segments_[i].Write(GpioPinValue.High);
                }
            }
        }

        public void clearDigit()
        {
            selectionPin_.Write(GpioPinValue.High);
            foreach(var pin in segments_)
            {
                pin.Write(GpioPinValue.High);
            }
        }


        private static readonly int[,] segmentsConfig_ = { {1, 1, 1, 1, 1, 1, 0},//digit 0
                                                           {0, 1, 1, 0, 0, 0, 0},//digit 1
                                                           {1, 1, 0, 1, 1, 0, 1},
                                                           {1, 1, 1, 1, 0, 0, 1},
                                                           {0, 1, 1, 0, 0, 1, 1},
                                                           {1, 0, 1, 1, 0, 1, 1},//5
                                                           {1, 0, 1, 1, 1, 1, 1},
                                                           {1, 1, 1, 0, 0, 0, 0},
                                                           {1, 1, 1, 1, 1, 1, 1},
                                                           {1, 1, 1, 0, 0, 1, 1},//digit 9
                                                           {1, 0, 0, 1, 1, 1, 1}//special case E for out error or out of range
                                                          };

        private const uint SEGMENTS = 7;
        private const uint DISPLAY_DIGITS = 4;
        private const uint DIGITS = 9;
        private GpioPin[] segments_ = new GpioPin[SEGMENTS];
        private GpioPin dp_;
        private GpioPin selectionPin_;

    }
}
