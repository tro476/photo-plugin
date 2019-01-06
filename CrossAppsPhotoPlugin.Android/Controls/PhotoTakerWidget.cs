﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CrossAppsPhotoPlugin.Android.Controls
{
    public class PhotoTakerWidget : FrameLayout
    {
        /// <summary>
        /// Getting camera preview and capture images.
        /// </summary>
        CameraWidget cameraWidget;

        /// <summary>
        /// preview of all captured images in one view,
        /// with delete option.
        /// </summary>
        MultiPhotoSelectorView multiPhotoSelectorView;

        /// <summary>
        /// Controls for default camera, take, switch camera,
        /// focus 
        /// </summary>
        PhotoTakerControlsOverlayView controlsOverlayView;

        // SeekBar seekBar;

        public int MaxImageCount { get; set; }

        public bool TakenImagesThumbnailVisible { get; set; } = false;

        public EventHandler SendButtonTapped { get; set; }

        public ObservableCollection<Java.IO.File> Photos { get; set; }

        public PhotoTakerWidget(Context context) : base(context)
        {
            Photos = new ObservableCollection<Java.IO.File>();
            Photos.CollectionChanged += Photos_CollectionChanged;

            controlsOverlayView = new PhotoTakerControlsOverlayView(context);
            controlsOverlayView.TakeButtonTouched += ControlsOverlayView_TakeButtonTouched;
            controlsOverlayView.FlashButtonTouched += ControlsOverlayView_FlashButtonTouched;
            controlsOverlayView.CameraButtonTouched += ControlsOverlayView_CameraButtonTouched;
            controlsOverlayView.CounterButtonTouched += ControlsOverlayView_CounterButtonTouched;

            cameraWidget = new CameraWidget(context, Photos);

            multiPhotoSelectorView = new MultiPhotoSelectorView(context, Photos);
            multiPhotoSelectorView.Visibility = ViewStates.Invisible;
            multiPhotoSelectorView.CloseButtonTouched += MultiPhotoSelectorView_CloseButtonTouched;

            /*
            seekBar = new SeekBar(context);
            seekBar.Thumb.SetTint(Color.White);
            seekBar.ProgressDrawable.SetColorFilter(Color.White, PorterDuff.Mode.SrcIn);
            seekBar.ProgressChanged += SeekBar_ProgressChanged;
            */
            AddView(cameraWidget.mTextureView);
            // AddView(seekBar);
            AddView(controlsOverlayView);
            AddView(multiPhotoSelectorView);
        }

        void SeekBar_ProgressChanged(object sender, SeekBar.ProgressChangedEventArgs e)
        {
            cameraWidget.ChangeZoom(e.Progress);
        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

            var layoutParameters = new FrameLayout.LayoutParams(Width, 50, GravityFlags.Bottom);
            layoutParameters.SetMargins(0, 0, 0, 300);
            // seekBar.LayoutParameters = layoutParameters;
        }

        /*
        public override bool OnInterceptTouchEvent(MotionEvent ev)
        {
            return base.OnInterceptTouchEvent(ev);
        }
        */

        public override bool OnTouchEvent(MotionEvent e)
        {
            controlsOverlayView.OnTouchEvent(e);
            cameraWidget.OnTouchEvent(e);

            return true;
        }

        void MultiPhotoSelectorView_CloseButtonTouched(object sender, EventArgs e)
        {
            multiPhotoSelectorView.Visibility = ViewStates.Invisible;
        }

        void ControlsOverlayView_CounterButtonTouched(object sender, EventArgs e)
        {
            multiPhotoSelectorView.Visibility = ViewStates.Visible;
            multiPhotoSelectorView.SetLayoutParameters();
        }

        void Photos_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            controlsOverlayView.Counter = Photos.Count;
        }

        void ControlsOverlayView_CameraButtonTouched(object sender, EventArgs e)
        {
            cameraWidget.SwitchCamera();
        }

        void ControlsOverlayView_FlashButtonTouched(object sender, EventArgs e)
        {
            cameraWidget.IsFlashEnabled = !cameraWidget.IsFlashEnabled;
        }

        void ControlsOverlayView_TakeButtonTouched(object sender, EventArgs e)
        {
            cameraWidget.TakePicture();
        }

        public List<string> SaveFiles()
        {
            List<string> fileNames = new List<string>();

            var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            var tmp = System.IO.Path.Combine(documents, "..", "tmp");

            /*
            foreach (var image in takenPhotosOverlayView.Photos)
            {
                var fileName = Path.Combine(tmp, Guid.NewGuid().ToString() + ".jpg");
                image.AsJPEG().Save(fileName, true);
                fileNames.Add(fileName);
            }
            */

            return fileNames;
        }
    }
}