using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using Windows.Storage;
using Windows.UI.Xaml;

namespace CMScoutIntrinsic {

    class FavoritesViewModel : ViewModelBase {

        public event EventHandler FavoritesFilterApplied;
        public event EventHandler FavoritesLoaded;
        public event EventHandler FavoritesChanged;

        public FavoritesViewModel(MainViewModel mainViewModel) {
            App app = (App)Application.Current;

            _mainViewModel = mainViewModel;

            app.DataService.LoadFinished += OnLoadFinished;

            _mainViewModel.PropertyChanged += OnMainViewModelPropertyChanged;
        }

        public Boolean IsOpened {
            get {
                return _isOpened;
            }

            set {
                _isOpened = value;

                RaisePropertyChanged();
            }
        }

        public Int32 FavoritesCount {
            get {
                return _favoritesCount;
            }

            private set {
                _favoritesCount = value;

                RaisePropertyChanged();
            }
        }

        public ICommand ListAllFavorites {
            get {
                return _listAllFavorites ?? (
                    _listAllFavorites = new RelayCommand(
                        param => {
                            FavoritesFilterApplied?.Invoke(this, EventArgs.Empty);
                        },
                        param => FavoritesCount != 0
                    )
                );
            }
        }

        public ICommand AddToFavorites {
            get {
                return _addToFavorites ?? (
                    _addToFavorites = new RelayCommand(
                        param => {
                            CMStaffVM staff = _mainViewModel.SelectedPlayer;

                            staff.IsFavorite = true;

                            ++FavoritesCount;

                            UpdateCanExecuteCommands();

                            FavoritesChanged?.Invoke(this, EventArgs.Empty);
                        },
                        param => {
                            CMStaffVM staff = _mainViewModel.SelectedPlayer;

                            return staff != null && !staff.IsFavorite;
                        }
                    )
                );
            }
        }

        public ICommand RemoveFromFavorites {
            get {
                return _removeFromFavorites ?? (
                    _removeFromFavorites = new RelayCommand(
                        param => {
                            CMStaffVM staff = _mainViewModel.SelectedPlayer;

                            staff.IsFavorite = false;

                            --FavoritesCount;

                            UpdateCanExecuteCommands();

                            FavoritesChanged?.Invoke(this, EventArgs.Empty);
                        },
                        param => {
                            CMStaffVM staff = _mainViewModel.SelectedPlayer;

                            return staff != null && staff.IsFavorite;
                        }
                    )
                );
            }
        }

        public ICommand SaveFavorites {
            get {
                return _saveFavorites ?? (
                    _saveFavorites = new RelayCommand(
                        async param => {
                            App app = (App) Application.Current;

                            StorageFile file = await app.NavigationService.PickSaveFileAsync(
                                new [] {
                                    new Pair< String, List<String> >("Short List", new List<String> { ".pls", }),
                                },
                                String.Format("Favorites_{0}.pls", Path.GetFileNameWithoutExtension(_mainViewModel.OpenViewModel.OpenedFile.Name))
                            );

                            if(file != null) {
                                try {
                                    using(Stream stream = await file.OpenStreamForWriteAsync()) {
                                        using(BinaryWriter binaryWriter = new BinaryWriter(stream)) {
                                            String searchName = "CM Scount Intristic Search";
                                            String playerName = "Vladimir Filatov";

                                            binaryWriter.Write(new Byte[] { 0x9E, 0x4A, 0x02, 0x00, 0x01, 0x01 });
                                            
                                            binaryWriter.Write(searchName.ToCharArray());
                                            for(Int32 i = 0; i < 101 - searchName.Length; ++i) { binaryWriter.Write((Byte)0x00); }
                                            
                                            binaryWriter.Write(playerName.ToCharArray());
                                            for(Int32 i = 0; i < 252 - playerName.Length; ++i) { binaryWriter.Write((Byte)0x00); }

                                            binaryWriter.Write(new Byte[] { 0xFC, 0x13, 0x00, 0x00, 0xFA, 0x00, 0x00, 0x00 });

                                            binaryWriter.Write(_favoritesCount);

                                            foreach(CMStaff staff in app.DataService.Staffs) {
                                                if(DataService.IsValidPlayer(staff)) {
                                                    if(staff.IsFavorite) {
                                                        binaryWriter.Write(staff.FirstNameId);
                                                        binaryWriter.Write(staff.SecondNameId);
                                                        binaryWriter.Write(staff.CommonNameId);
                                                        binaryWriter.Write(staff.Id);
                                                        binaryWriter.Write((Int16)(staff.DateOfBirth.Value.DayOfYear - 1));
                                                        binaryWriter.Write((Int16)staff.DateOfBirth.Value.Year);
                                                        binaryWriter.Write((Int32)(DateTime.IsLeapYear(staff.DateOfBirth.Value.Year) ? 1 : 0));

                                                        for(Int32 i = 0; i <= 19; ++i) { binaryWriter.Write((Byte)0x00); }

                                                        binaryWriter.Write((Byte)0xFF);
                                                    }
                                                }
                                            }

                                            binaryWriter.Write(
                                                new Byte[] {
                                                   0x81, 0x00, 0x14, 0x14, 0x84, 0xA8, 0xAA, 0x44, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x01, 0xFF, 0xFF,
                                                   0xFF, 0xFF, 0xFF, 0x00, 0x05, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
                                                   0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
                                                   0x01, 0x01, 0x01, 0x01, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14,
                                                   0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14,
                                                   0x14, 0x14, 0x14, 0x00, 0x00, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01,
                                                   0x01, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x14, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                                                   0x00, 0x81, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00, 0x01,
                                                   0x00, 0x01, 0xFF, 0xFF, 0xFF, 0xFF, 0xF2, 0xFF, 0xFF, 0xF2
                                                }
                                            );
                                        }
                                    }
                                }
                                catch(Exception) {
                                }
                            }

                        },
                        param => FavoritesCount != 0
                    )
                );
            }
        }

        public ICommand LoadFavorites {
            get {
                return _loadFavorites ?? (
                    _loadFavorites = new RelayCommand(
                        async param => {
                            App app = (App) Application.Current;

                            StorageFile file = await app.NavigationService.PickSingleFileAsync(new [] { "*", ".pls" });

                            if(file != null) {
                                try {
                                    List<Int32> staffIds = new List<Int32>();

                                    using(Stream stream = await file.OpenStreamForReadAsync()) {
                                        using(BinaryReader binaryReader = new BinaryReader(stream)) {
                                            binaryReader.ReadBytes(367);

                                            Int32 favoritesCount = binaryReader.ReadInt32();

                                            for(Int32 i = 0; i < favoritesCount; ++i) {
                                                binaryReader.ReadInt32();
                                                binaryReader.ReadInt32();
                                                binaryReader.ReadInt32();
                                                Int32 staffId = binaryReader.ReadInt32();
                                                binaryReader.ReadInt16();
                                                binaryReader.ReadInt16();
                                                binaryReader.ReadInt32();
                                                binaryReader.ReadBytes(20);
                                                binaryReader.ReadByte();

                                                staffIds.Add(staffId);
                                            }
                                        }
                                    }

                                    foreach(CMStaff staff in app.DataService.Staffs) {
                                        if(DataService.IsValidPlayer(staff)) {
                                            staff.IsFavorite = staffIds.Contains(staff.Id);
                                        }
                                    }

                                    FavoritesCount = staffIds.Count;

                                    UpdateCanExecuteCommands();

                                    FavoritesLoaded?.Invoke(this, EventArgs.Empty);
                                }
                                catch(Exception) {
                                }
                            }
                        },
                        param => true
                    )
                );
            }
        }



        private void OnLoadFinished(Object sender, DataService.LoadFinishedEventArgs args) {
            if(args.ErrorMessage == null) {
                App app = (App) Application.Current;

                Int32 favoritesCount = 0;

                foreach(CMStaff staff in app.DataService.Staffs) {
                    if(DataService.IsValidPlayer(staff)) {
                        if(staff.IsFavorite) {
                            ++favoritesCount;
                        }
                    }
                }

                FavoritesCount = favoritesCount;

                UpdateCanExecuteCommands();
            }
        }

        private void OnMainViewModelPropertyChanged(Object sender, PropertyChangedEventArgs args) {
            if(args.PropertyName == nameof(MainViewModel.SelectedPlayer)) {
                UpdateCanExecuteCommands();
            }
        }

        private void UpdateCanExecuteCommands() {
            _listAllFavorites?.RaiseCanExecuteChanged();
            _addToFavorites?.RaiseCanExecuteChanged();
            _removeFromFavorites?.RaiseCanExecuteChanged();
        }



        private MainViewModel _mainViewModel;
        private Boolean       _isOpened;
        private Int32         _favoritesCount;
        private RelayCommand  _listAllFavorites;
        private RelayCommand  _addToFavorites;
        private RelayCommand  _removeFromFavorites;
        private RelayCommand  _saveFavorites;
        private RelayCommand  _loadFavorites;
    }

}
