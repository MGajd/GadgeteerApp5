//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GadgeteerApp5 {
    using Gadgeteer;
    using GTM = Gadgeteer.Modules;
    
    
    public partial class Program : Gadgeteer.Program {
        
        /// <summary>The Touch C8 module using socket 10 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.TouchC8 touch;
        
        /// <summary>The Tunes module using socket 11 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Tunes tunes;
        
        /// <summary>The Button module using socket 14 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.Button button;
        
        /// <summary>The Character Display module using socket 12 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.CharacterDisplay display;
        
        /// <summary>The USB Client DP module using socket 1 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.USBClientDP usbClientDP;
        
        /// <summary>The Multicolor LED module using socket 8 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.MulticolorLED led1;
        
        /// <summary>The Multicolor LED module using socket * of led1.</summary>
        private Gadgeteer.Modules.GHIElectronics.MulticolorLED led2;
        
        /// <summary>The TempHumid SI70 module using socket 5 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.TempHumidSI70 sensor1;
        
        /// <summary>The TempHumid SI70 module using socket 6 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.TempHumidSI70 sensor2;
        
        /// <summary>The FLASH module using socket 9 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.FLASH flash;
        
        /// <summary>The CellularRadio module using socket 4 of the mainboard.</summary>
        private Gadgeteer.Modules.GHIElectronics.CellularRadio gsm;
        
        /// <summary>This property provides access to the Mainboard API. This is normally not necessary for an end user program.</summary>
        protected new static GHIElectronics.Gadgeteer.FEZSpider Mainboard {
            get {
                return ((GHIElectronics.Gadgeteer.FEZSpider)(Gadgeteer.Program.Mainboard));
            }
            set {
                Gadgeteer.Program.Mainboard = value;
            }
        }
        
        /// <summary>This method runs automatically when the device is powered, and calls ProgramStarted.</summary>
        public static void Main() {
            // Important to initialize the Mainboard first
            Program.Mainboard = new GHIElectronics.Gadgeteer.FEZSpider();
            Program p = new Program();
            p.InitializeModules();
            p.ProgramStarted();
            // Starts Dispatcher
            p.Run();
        }
        
        private void InitializeModules() {
            this.touch = new GTM.GHIElectronics.TouchC8(10);
            this.tunes = new GTM.GHIElectronics.Tunes(11);
            this.button = new GTM.GHIElectronics.Button(14);
            this.display = new GTM.GHIElectronics.CharacterDisplay(12);
            this.usbClientDP = new GTM.GHIElectronics.USBClientDP(1);
            this.led1 = new GTM.GHIElectronics.MulticolorLED(8);
            this.led2 = new GTM.GHIElectronics.MulticolorLED(this.led1.DaisyLinkSocketNumber);
            this.sensor1 = new GTM.GHIElectronics.TempHumidSI70(5);
            this.sensor2 = new GTM.GHIElectronics.TempHumidSI70(6);
            this.flash = new GTM.GHIElectronics.FLASH(9);
            this.gsm = new GTM.GHIElectronics.CellularRadio(4);
        }
    }
}
