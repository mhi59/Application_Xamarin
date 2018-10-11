using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Speech.Tts;
using Android.Bluetooth;

namespace ApplicationLED.Droid
{
    [Activity(Label = "ApplicationLED", Icon = "@drawable/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            global::Xamarin.Forms.Forms.Init(this, bundle);
            LoadApplication(new App());
        }    

        protected override void OnActivityResult(int requestCode, Result resultVal, Intent data)//Méthode qui prend de relai après la saisie vocale 
        {
            if (requestCode == 42) // si le code correspond bien avec le int VOICE dans la class BluetoothLED
            {
                if (resultVal == Result.Ok)
                {
                    var matches = data.GetStringArrayListExtra(Android.Speech.RecognizerIntent.ExtraResults);//Array qui a récupéré ce qui a été dicté
                    if (matches.Count != 0)//Si on a au moins un élément dans le Array
                    {
                        string textInput = matches[0];//Récupération du mot trouvé
                        base.OnActivityResult(requestCode, resultVal, data);
                        BluetoothLED InstanceBluetooth = new BluetoothLED();
                        InstanceBluetooth.SendDataToArduino(textInput);//On appelle la méthode SendDataToArduino pour lui envoyer la couleur dictée
                    }
                }

            }
        }

  

        
    }




}

