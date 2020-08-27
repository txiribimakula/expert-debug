﻿using EnvDTE;
using System.Collections.ObjectModel;
using System.Windows;
using Txiribimakula.ExpertWatch.Drawing;
using Txiribimakula.ExpertWatch.Geometries;
using Txiribimakula.ExpertWatch.Geometries.Contracts;
using Txiribimakula.ExpertWatch.Graphics;
using Txiribimakula.ExpertWatch.Graphics.Contracts;
using Txiribimakula.ExpertWatch.Loading;
using Txiribimakula.ExpertWatch.Models;

namespace Txiribimakula.ExpertWatch.ViewModels
{
    public class ViewModel {
        public ViewModel(Debugger debugger) {
            WatchItems = new ObservableCollection<WatchItem>();
            WatchItems.CollectionChanged += OnWatchItemsCollectionChanged;

            IInterpreter interpreter = new Interpreter();
            loader = new Loader(debugger, interpreter);
        }

        private Loader loader;
        private GeometryDrawer geoDrawer;
        public ObservableCollection<WatchItem> WatchItems { get; set; }

        private bool isMiddleMouseDown;
        private IPoint lastClickPoint;

        public void OnSizeChanged(object sender, SizeChangedEventArgs args) {
            ICoordinateSystem coordinateSystem = new CoordinateSystem((float)args.NewSize.Width, (float)args.NewSize.Height, new Box(-10, 10, -10, 10));
            if (geoDrawer == null) {
                geoDrawer = new GeometryDrawer(coordinateSystem);
            } else {
                IPoint currentOffset = geoDrawer.CoordinateSystem.Offset;
                geoDrawer.CoordinateSystem = coordinateSystem;
                geoDrawer.CoordinateSystem.Offset = currentOffset;
                foreach (var watchItem in WatchItems) {
                    geoDrawer.TransformGeometries(watchItem.Drawables);
                }
            }
        }

        public void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            IInputElement senderElement = (IInputElement)sender;
            if (e.MiddleButton == System.Windows.Input.MouseButtonState.Pressed) {
                isMiddleMouseDown = true;
                System.Windows.Point point = e.GetPosition(senderElement);
                lastClickPoint = new Geometries.Point((float)point.X, (float)point.Y);
            }
            senderElement.CaptureMouse();
        }

        public void OnMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            IInputElement senderElement = (IInputElement)sender;
            isMiddleMouseDown = false;
            senderElement.ReleaseMouseCapture();
        }

        public void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
            IInputElement senderElement = (IInputElement)sender;
            System.Windows.Point canvasClickPoint = e.GetPosition(senderElement);
            IPoint clickPoint = new Geometries.Point((float)canvasClickPoint.X, (float)canvasClickPoint.Y);
            if (isMiddleMouseDown) {
                float incrementalX = clickPoint.X - lastClickPoint.X;
                float incrementalY = clickPoint.Y - lastClickPoint.Y;
                geoDrawer.DrawableVisitor.CoordinateSystem.Offset = new Geometries.Point(incrementalX, incrementalY);
                foreach (var watchItem in WatchItems) {
                    geoDrawer.TransformGeometries(watchItem.Drawables);
                }
                lastClickPoint = clickPoint;
            }
        }

        private void OnWatchItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
            if (e.NewItems != null) {
                foreach (WatchItem item in e.NewItems) {
                    if(item != null) {
                        item.NameChanged += OnWatchItemNameChangedAsync;
                    }
                }
            }
            if (e.OldItems != null) {
                foreach (WatchItem item in e.OldItems) {
                    if (item != null) {
                        item.NameChanged -= OnWatchItemNameChangedAsync;
                    }
                }
            }
        }

        public void OnMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e) {
            IInputElement senderElement = (IInputElement)sender;
            System.Windows.Point cursorWorldPoint = e.GetPosition(senderElement);
            IPoint localPoint = new Geometries.Point(geoDrawer.DrawableVisitor.CoordinateSystem.ConvertXToLocal((float)cursorWorldPoint.X), geoDrawer.DrawableVisitor.CoordinateSystem.ConvertYToLocal((float)cursorWorldPoint.Y));
            if (e.Delta < 0) {
                geoDrawer.DrawableVisitor.CoordinateSystem.Scale *= 1.1f;
            } else {
                geoDrawer.DrawableVisitor.CoordinateSystem.Scale /= 1.1f;
            }
            float newWorldPointX = geoDrawer.DrawableVisitor.CoordinateSystem.ConvertXToWorld(localPoint.X);
            float newWorldPointY = geoDrawer.DrawableVisitor.CoordinateSystem.ConvertYToWorld(localPoint.Y);

            geoDrawer.DrawableVisitor.CoordinateSystem.Offset = new Geometries.Point((float)cursorWorldPoint.X - newWorldPointX, (float)cursorWorldPoint.Y - newWorldPointY);

            foreach (var watchItem in WatchItems) {
                geoDrawer.TransformGeometries(watchItem.Drawables);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Evite métodos async void", Justification = "Used for an event")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "VSTHRD200:Use el sufijo \"Async\" para métodos asincrónicos", Justification = "Used for an event")]
        private async void OnWatchItemNameChangedAsync(WatchItem sender) {
            WatchItem item = await loader.LoadAsync(sender.Name);
            geoDrawer.TransformGeometries(item.Drawables);
            sender.Description = item.Description;
            sender.Drawables = item.Drawables;
        }
    }
}