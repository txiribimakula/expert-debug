﻿using System.ComponentModel;
using Txiribimakula.ExpertWatch.Drawing;
using Txiribimakula.ExpertWatch.Geometries;

namespace Txiribimakula.ExpertWatch.Loading
{
    public class WatchItem : INotifyPropertyChanged
    {
        public WatchItem() {
            Drawables = new DrawableCollection<IDrawable>(new Box(0,0,0,0));
            isLoading = true;
        }

        private bool isLoading;
        public bool IsLoading {
            get { return isLoading; }
            set { 
                isLoading = value;
                OnLoadingChanged();
            }
        }
        public event WatchItemEventHandler IsLoadingChanged;
        private void OnLoadingChanged() {
            IsLoadingChanged?.Invoke(this);
        }

        private string name;
        public string Name {
            get { return name; }
            set { name = value; OnNameChanged(); }
        }

        private string description;
        public string Description {
            get { return description; }
            set { description = value; OnPropertyChanged(nameof(Description)); }
        }

        private DrawableCollection<IDrawable> drawables;
        public DrawableCollection<IDrawable> Drawables {
            get { return drawables; }
            set { drawables = value; OnPropertyChanged(nameof(Drawables)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        public event WatchItemEventHandler NameChanged;
        private void OnNameChanged() {
            NameChanged?.Invoke(this);
        }
        public delegate void WatchItemEventHandler(WatchItem sender);
    }
}
