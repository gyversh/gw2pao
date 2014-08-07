﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GW2PAO.API.Data.Enums;
using GW2PAO.Controllers.Interfaces;
using GW2PAO.ViewModels.DungeonTracker;
using GW2PAO.ViewModels.Interfaces;
using GW2PAO.ViewModels.WvWTracker;
using NLog;

namespace GW2PAO.Views.WvWTracker
{
    /// <summary>
    /// Interaction logic for WvWTrackerView.xaml
    /// </summary>
    public partial class WvWTrackerView : OverlayWindow
    {
        /// <summary>
        /// Default logger
        /// </summary>
        private static Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// True if the user is resizing the window, else false
        /// </summary>
        private bool resizeInProcess = false;

        /// <summary>
        /// WvW controller
        /// </summary>
        private IWvWController controller;

        /// <summary>
        /// WvW tracker view model
        /// </summary>
        private WvWTrackerViewModel viewModel;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dungeonsController">The dungeons controller</param>
        public WvWTrackerView(IWvWController wvwController, IHasWvWMap mapVm)
        {
            logger.Debug("New WvWTrackerView created");
            this.controller = wvwController;
            this.viewModel = new WvWTrackerViewModel(this.controller, mapVm);
            this.DataContext = this.viewModel;
            InitializeComponent();

            // Set the window size and location
            this.Closing += WvWTrackerView_Closing;
            if (Properties.Settings.Default.WvWTrackerHeight > 0)
                this.Height = Properties.Settings.Default.WvWTrackerHeight;
            if (Properties.Settings.Default.WvWTrackerWidth > 0)
                this.Width = Properties.Settings.Default.WvWTrackerWidth;
            this.Left = Properties.Settings.Default.WvWTrackerX;
            this.Top = Properties.Settings.Default.WvWTrackerY;
        }

        private void WvWTrackerView_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.WindowState == System.Windows.WindowState.Normal)
            {
                Properties.Settings.Default.WvWTrackerHeight = this.Height;
                Properties.Settings.Default.WvWTrackerWidth = this.Width;
                Properties.Settings.Default.WvWTrackerX = this.Left;
                Properties.Settings.Default.WvWTrackerY = this.Top;
                Properties.Settings.Default.Save();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            logger.Debug("WvWTrackerView closed");
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MinimizeWindowButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Resize_Init(object sender, MouseButtonEventArgs e)
        {
            Grid senderRect = sender as Grid;

            if (senderRect != null)
            {
                resizeInProcess = true;
                senderRect.CaptureMouse();
            }
        }

        private void Resize_End(object sender, MouseButtonEventArgs e)
        {
            Grid senderRect = sender as Grid;
            if (senderRect != null)
            {
                resizeInProcess = false;
                senderRect.ReleaseMouseCapture();
            }
        }

        private void Resizeing_Form(object sender, MouseEventArgs e)
        {
            if (resizeInProcess)
            {
                Grid senderRect = sender as Grid;
                if (senderRect != null)
                {
                    double width = e.GetPosition(this).X;
                    double height = e.GetPosition(this).Y;
                    senderRect.CaptureMouse();
                    if (senderRect.Name == "ResizeWidth")
                    {
                        width += 1;
                        if (width > 0 && width < this.MaxWidth && width > this.MinWidth)
                        {
                            this.Width = width;
                        }
                    }
                    else if (senderRect.Name == "ResizeHeight")
                    {
                        height += 1;
                        if (height > 0 && height < this.MaxHeight && height > this.MinHeight)
                        {
                            this.Height = height;
                        }
                    }
                }
            }
        }

        private void TitleImage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Image image = sender as Image;
                ContextMenu contextMenu = image.ContextMenu;
                contextMenu.PlacementTarget = image;
                contextMenu.IsOpen = true;
                e.Handled = true;
            }
        }
    }
}