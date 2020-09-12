﻿using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using Txiribimakula.ExpertWatch.Drawing;
using Txiribimakula.ExpertWatch.Geometries;

namespace Txiribimakula.ExpertWatch.Loading
{
    public class WatchItem : INotifyPropertyChanged
    {
        public WatchItem() {
            Drawables = new DrawableCollection<IDrawable>(new Box(0,0,0,0));
            TokenSource = new CancellationTokenSource();
        }

        public CancellationTokenSource TokenSource { get; set; }

        public void OnCancelTokenClick(object sender, EventArgs e) {
            TokenSource.Cancel();
            TokenSource = new CancellationTokenSource();
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
