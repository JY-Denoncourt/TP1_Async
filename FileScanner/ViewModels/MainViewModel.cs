using FileScanner.Commands;
using FileScanner.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;

namespace FileScanner.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private string selectedFolder;
        private ObservableCollection<string> folderItems = new ObservableCollection<string>();
        private ObservableCollection<itemType> items = new ObservableCollection<itemType>();

        public DelegateCommand<string> OpenFolderCommand { get; private set; }
        public DelegateCommand<string> ScanFolderCommand { get; private set; }

        public ObservableCollection<string> FolderItems
        {
            get => folderItems;
            set
            {
                folderItems = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<itemType> Items
        {
            get => items;
            set
            {
                items = value;
                OnPropertyChanged();
            }
        }

        public string SelectedFolder
        {
            get => selectedFolder;
            set
            {
                selectedFolder = value;
                OnPropertyChanged();
                ScanFolderCommand.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel()
        {
            OpenFolderCommand = new DelegateCommand<string>(OpenFolder);
            ScanFolderCommand = new DelegateCommand<string>(ScanFolderAsync, CanExecuteScanFolder);
        }

        private bool CanExecuteScanFolder(string obj)
        {
            return !string.IsNullOrEmpty(SelectedFolder);
        }

        private void OpenFolder(string obj)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    SelectedFolder = fbd.SelectedPath;
                }
            }
        }

        private async void ScanFolderAsync(string dir)
        {
            await Task.Run( () =>
           {
               System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate ()
               {
                    Items.Clear();
               });

               try
               {
                   FolderItems = new ObservableCollection<string>(GetDirs(dir));  //Recoit tout le dossier choisi (fic & dos)
                }
               catch
               {
                   MessageBox.Show("Acces refuse, choisir un dossier avec acces autorise svp.");
               }

               foreach (var item in Directory.EnumerateFiles(dir, "*"))  //si c'est de file
                {
                   var IM = new itemType { Name = item, Image = "/Image/File.png" };
                    System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate ()
                    {
                        Items.Add(IM);
                    });
               }

               foreach (var item in FolderItems)
               {
                   var IM = new itemType { Name = item, Image = "/Image/Folder.png" };
                   System.Windows.Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate ()
                   {
                       Items.Add(IM);
                   });
               }
           });
            
        }



        IEnumerable<string> GetDirs(string dir)
        {
            foreach (var d in Directory.EnumerateDirectories(dir, "*", SearchOption.AllDirectories))
                {
                    yield return d;
                }
        }


        




        //Fait
        //TODO : Rendre l'application asynchrone
        //TODO : Ajouter icon si dossier ou fichier
        //TODO : Ajouter un try/catch pour les dossiers sans permission
        //TODO : Tester avec un dossier avec beaucoup de fichier

        //Source 
        //https://stackoverflow.com/questions/1395205/better-way-to-check-if-a-path-is-a-file-or-a-directory
        //https://github.com/nbourre/0SS_semaine_04
        //https://youtu.be/2moh18sh5p4
        //https://docs.microsoft.com/fr-fr/dotnet/csharp/programming-guide/concepts/async/
    }
}
