using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace GadgeteerApp5
{
    public partial class Program
    {

        
        private const int SLEEPTIME = 10;

        #region Private properties
        int sleepCounter = SLEEPTIME;
        GT.Timer sleepTimeTimer = new GT.Timer(1000);


        private double MinTemperature = 20;
        private double MaxTemperature = 26;

        private double MinHumidity = 15;
        private double MaxHumidity = 19;     
   
        private Enums.MinMax minOrMax = Enums.MinMax.min;
        private Enums.Unit actualUnit = Enums.Unit.temperature;

        private UnitBound activeUnitBound;

        private UnitBound[] unitBounds;
        private TempHumidSI70[] sensorsArray = new TempHumidSI70[2];
        #endregion



        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {

            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            Initialize();
            RegisterEventsAndModules();

            // every second 
            GT.Timer timer = new GT.Timer(1000);
            timer.Tick += timer_Tick;
            timer.Start();

            display.BacklightEnabled = true;
            led1.BlinkRepeatedly(GT.Color.Red);
            //Thread.Sleep(3000);
        }

        private void Initialize()
        {
            MinTemperature = 20;
            MaxTemperature = 26;

            MinHumidity = 15;
            MaxHumidity = 19;

            minOrMax = Enums.MinMax.min;
            actualUnit = Enums.Unit.temperature;

            #region Sensors
            sensorsArray[0] = sensor1;
            sensorsArray[1] = sensor2;
            #endregion

            unitBounds = new UnitBound[4];

            InitializeBounds();

        }

        private void InitializeBounds()
        {
            unitBounds[0] = new UnitBound(Enums.MinMax.min, Enums.Unit.temperature, MinTemperature);
            unitBounds[1] = new UnitBound(Enums.MinMax.max, Enums.Unit.temperature, MaxTemperature);
            unitBounds[2] = new UnitBound(Enums.MinMax.min, Enums.Unit.humidity, MinHumidity);
            unitBounds[3] = new UnitBound(Enums.MinMax.max, Enums.Unit.humidity, MaxHumidity);
        }

        private void RegisterEventsAndModules()
        {       
            sleepTimeTimer.Tick += sleepTimeTimer_Tick;

            #region Button
            button.ButtonPressed += button_ButtonPressed;
            #endregion

            //"klavesnica"
            #region Touch
            touch.WheelPositionChanged += touch_WheelPositionChanged;
            touch.ButtonPressed += touch_ButtonPressed;
            #endregion

            #region Gsm
            gsm.SmsReceived += gsm_SmsReceived;
            #endregion
        }

        void touch_ButtonPressed(TouchC8 sender, TouchC8.ButtonTouchedEventArgs e)
        {
            int pressedButton = 1;

            switch (pressedButton)
            {
                case 1:
                    actualUnit = Enums.Unit.temperature;
                    break;

                case 2:
                    actualUnit = Enums.Unit.humidity;
                    break;

                case 3:
                    minOrMax = minOrMax == Enums.MinMax.max ? Enums.MinMax.min : Enums.MinMax.max;
                    break;
            }

            activeUnitBound = GetUnitBound(actualUnit,minOrMax);
        }

        private UnitBound GetUnitBound(Enums.Unit actualUnit, Enums.MinMax minOrMax)
        {

            foreach (var bound in unitBounds)
            {
                if (bound.MinMax == minOrMax && bound.Unit == actualUnit)
                {
                    return bound;
                }
            }
            throw new Exception("Bound not defined " + minOrMax + " " + actualUnit.ToString());
        }

        /// <summary>
        /// Turn off the display after dellay.
        /// </summary>
        /// <param name="timer"></param>
        void sleepTimeTimer_Tick(GT.Timer timer)
        {
            sleepCounter--;
            if (sleepCounter == 0)
            {
                display.BacklightEnabled = false;
            }
        }

        void timer_Tick(GT.Timer timer)
        {
            TakeMeasurement();
            DoBeep(400,100);
        }

        private void TakeMeasurement()
        {
            TakeMeasurement(Enums.Sensors.Sensor1);
            TakeMeasurement(Enums.Sensors.Sensor2);
            TakeMeasurement(Enums.Sensors.All);
        }

        private void TakeMeasurement(Enums.Sensors sensors, bool showOnDisplay = true)
        {
            switch (sensors)
            {
                case Enums.Sensors.Sensor1: 
                    ProcessMeasuremnent(sensor1.TakeMeasurement(), led1);
                    break;

                case Enums.Sensors.Sensor2: 
                    ProcessMeasuremnent(sensor2.TakeMeasurement(), led2);
                    break;

                case Enums.Sensors.All:
                    ProcessMeasuremnent(sensor1.TakeMeasurement(), sensor2.TakeMeasurement(), showOnDisplay);
                    break;

            }
        }

        private void ProcessMeasuremnent(TempHumidSI70.Measurement measurement1, TempHumidSI70.Measurement measurement2, bool showOnDisplay)
        {
            double averageTemperature = (measurement1.Temperature + measurement2.Temperature) / 2;
            double averageHumidity = (measurement1.RelativeHumidity + measurement2.RelativeHumidity) / 2;

            if (showOnDisplay)
            {
                DisplayText(true, "Teplota: " + averageTemperature.ToString(), "Vlhkost: " + averageHumidity.ToString());
            }
            else
            {
                if (averageHumidity < MinHumidity || averageTemperature < MinTemperature)
                {
                    DoBeep(200, 1000);
                    string unit = string.Empty;

                    if (averageHumidity < MinHumidity)
                        unit = "vlhkost";
                    else
                        unit = "teplota";

                    SendSMS("Nizka priemerna " + unit);
                }
                else if (averageHumidity > MaxHumidity || averageTemperature > MaxTemperature)
                {
                    DoBeep(600, 1000);
                    string unit = string.Empty;

                    if (averageHumidity > MaxHumidity)
                        unit = "vlhkost";
                    else
                        unit = "teplota";

                    SendSMS("Vysoka priemerna " + unit);               
                }
            }
        }

        private void ProcessMeasuremnent(TempHumidSI70.Measurement measurement, MulticolorLED led)
        {
            GT.Color temperatureColor;
            GT.Color humidityColor;
            TimeSpan timespan = new TimeSpan(1000);

            if (measurement.Temperature > MaxTemperature)
                temperatureColor = GT.Color.Red;
            else if (measurement.Temperature < MaxTemperature)
                temperatureColor = GT.Color.Yellow;
            else
                temperatureColor = GT.Color.Orange;


            if (measurement.RelativeHumidity > MaxHumidity)
                humidityColor = GT.Color.FromRGB(0, 0, 139);
            else if (measurement.Temperature < MaxTemperature)
                humidityColor = GT.Color.FromRGB(173, 216, 230);
            else
                humidityColor = GT.Color.FromRGB(111, 109, 185);
            
            led.BlinkRepeatedly(temperatureColor,timespan,humidityColor, timespan);
        }

        void touch_WheelPositionChanged(TouchC8 sender, TouchC8.WheelPositionChangedEventArgs e)
        {
            double coef = touch.GetWheelDirection() == TouchC8.Direction.Clockwise ? 0.2 : -0.2;
                            
        }

        private void button_ButtonPressed(Button sender, Button.ButtonState state)
        {
            TakeMeasurement(Enums.Sensors.All, true);
        }

        void gsm_SmsReceived(CellularRadio sender, CellularRadio.Sms message)
        {
            message.Message = message.Message.Trim();
            double value;
            if (Double.TryParse(message.Message.Substring(message.Message.LastIndexOf(" ")), out value))
            {

                Enums.Unit unit = message.Message.Substring(0, "temperature".Length).ToLower() == "temperature" ? Enums.Unit.temperature : Enums.Unit.humidity;
                Enums.MinMax minmax = message.Message.Substring(message.Message.IndexOf(" "), 3).ToLower() == "min" ? Enums.MinMax.min : Enums.MinMax.max;

                GetUnitBound(unit, minmax).Value = value;
            }
        }


        private void DisplayText(bool clearAll, string line1Text, string line2Text = "")
        {
            if (clearAll)
            {
                display.Clear();
                display.SetCursorPosition(0, 0);
                display.Print(line1Text);

                if (line2Text != "")
                {
                    display.SetCursorPosition(1, 0);
                    display.Print(line2Text);
                }
            }
        }

        private void DisplayTextAtPosition(string text, int position = 0, int line = 0) {
            display.SetCursorPosition(line, position);
            display.Print(HelpFunctions.GetEmptyText(position,true));

            display.SetCursorPosition(line, position);
            display.Print(text);

        }

        private void DoBeep(int frequency, int duration )
        {
            tunes.Play(new Tunes.MusicNote(new Tunes.Tone(frequency),duration));
        }

        private void SendSMS(string SMSText)
        {
            gsm.SendSms("+421918922375", SMSText);
        }
    }
}
