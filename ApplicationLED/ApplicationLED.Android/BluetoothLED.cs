using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight.Views;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Runtime;
using Android.Speech;
using Android.Speech.Tts;
using Android.Views;
using Android.Widget;
using ApplicationLED.Droid;
using Java.Util;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Messaging;
using Android.OS;

[assembly: Xamarin.Forms.Dependency(typeof(BluetoothLED))]

namespace ApplicationLED.Droid
{    
    public class BluetoothLED : Activity, IServices
    {
        public BluetoothLED() { }

        BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
        static BluetoothSocket bluetoothSocket;        

        public string onBluetooth()
        {   
            
            if (adapter == null)
                throw new Exception("No Bluetooth adapter found.");

            ICollection<BluetoothDevice> pairedDevices = new List<BluetoothDevice>();//List de stockage des devices appairés
            pairedDevices = adapter.BondedDevices;//Appareils déjà appairés

            if (pairedDevices.Count > 0)//Si on a des appareils appairés
            {
                foreach (BluetoothDevice device in pairedDevices)//Pour chaque pareil appairé
                {
                    string deviceName = device.Name;//Nom de l'appareil
                    string deviceHardwareAddress = device.Address; // MAC address
                    ParcelUuid deviceUUID = device.GetUuids().ElementAt(0);// On récupère le UUID de l'appareil

                    if (device.Name == "HC-06")//Si l'appareil correspond, ici le Arduino
                    {
                        try
                        {
                            bluetoothSocket = device.CreateRfcommSocketToServiceRecord(deviceUUID.Uuid);  //On crée un socket avec en paramètre l'UUID du téléphone
                            bluetoothSocket.Connect(); // On se connecte                             
                        }
                        catch (Java.IO.IOException e)
                        {
                            return "connexionKO";
                        }

                        if (bluetoothSocket.IsConnected) return "connexionOK";//Retourne ok si la connexion s'est établie
                    }
                }
            }

            return "connexionKO";
        }

        private readonly int VOICE = 42; // Sert à identifier plus bas quand on appelle StartActivityResult  
        private static string couleurChoisie;//Récupération de la couleur choisie

        public void SendDataToArduino(string couleur)//Méthode pour envoyer les DATA à l'Arduino
        {
            byte[] msgBuffer = Encoding.ASCII.GetBytes(couleur);//Permet d'encoder en bytes le string reçu(ici la couleur dictée lors de la saisie vocale)
            bluetoothSocket.OutputStream.Write(msgBuffer, 0, msgBuffer.Length);//Envoi de la DATA à l'Arduino
            couleurChoisie = couleur;
        }

        public string SpeechToText()//Méthode pour initialiser la reconnaissance vocale et lancer l'activité qui va se répercuter dans la méthode OnActivityResult dans le MainActivity.cs
        {
            Intent voiceIntent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);//Ce qui est en dessous sert à paramétrer la reconnaissance vocale
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.French);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
            voiceIntent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
            voiceIntent.PutExtra(RecognizerIntent.ExtraMaxResults, 1);
            voiceIntent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            ((Activity)Forms.Context).StartActivityForResult(voiceIntent, VOICE);//On lance l'activité(Méthode un peu spécial car fonctionne pas avec directement StartActivityForResult)
            return couleurChoisie; //Renvoi de la couleur vers le code partagé pour l'utilisé avec el TextToSpeech
        }
    }
        
    
}