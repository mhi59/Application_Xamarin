using ApplicationLED;
using GalaSoft.MvvmLight.Messaging;
using Plugin.SpeechRecognition;
using Plugin.TextToSpeech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;


[assembly: Xamarin.Forms.Dependency(typeof(MainPage))]
namespace ApplicationLED
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{    
            InitializeComponent();          
        }

        async Task PutTaskDelay()//Méthode permettant d'inclure un temps d'attente
        {
            await Task.Delay(2000);
        }

        private  void SpeakBtn(object sender, EventArgs e)
        {                      
             string retourCouleurChoisie = DependencyService.Get<IServices>().SpeechToText();//Appelle de la dépendance pour la reconnaissance vocale qui va nous renvoyer en string la couleur choisie
             TextToSpeech(retourCouleurChoisie);        
        }        

        private async void TextToSpeech(string couleur)
        {
            await PutTaskDelay();//Permet d'attendre de récupérer la variable couleur lors du traitement
            CrossTextToSpeech.Current.Speak("Vous avez choisi d'afficher la couleur " + couleur).Wait(2000);//TextToSpeech avec la couleur choisie
            
        }

        private void ConnectBtn(object sender, EventArgs i)
        {            
            string retour = DependencyService.Get<IServices>().onBluetooth(); // En attente de la tentative de connexion
            if(retour == "connexionOK")//Si la connexion s'est bien passée, on rend visible le Label et on le commente
            {
                ConfirmConnexion.IsVisible = true;
                ConfirmConnexion.Text = "Connecté!";
            }
            else if( retour == "connexionKO")//Inverse d'au dessus
            {
                ConfirmConnexion.IsVisible = true;
                ConfirmConnexion.Text = "Non connecté";
            }
        }
    }
}
