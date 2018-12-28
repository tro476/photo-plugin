﻿using System;
using Java.IO;
using Java.Lang;
using Java.Nio;
using Android.Media;
using PhotoTaker.Droid.Controls;

namespace PhotoTaker.Droid.Listeners
{
    public class ImageAvailableListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        readonly File file;
        readonly CameraWidget owner;

        public ImageAvailableListener(CameraWidget cameraWidget, File file)
        {
            owner = cameraWidget ?? throw new ArgumentNullException("cameraWidget");
            this.file = file ?? throw new ArgumentNullException("file");
        }

        //public File File { get; private set; }
        //public Camera2BasicFragment Owner { get; private set; }

        public void OnImageAvailable(ImageReader reader)
        {
            owner.mBackgroundHandler.Post(new ImageSaver(reader.AcquireNextImage(), file));
        }

        // Saves a JPEG {@link Image} into the specified {@link File}.
        private class ImageSaver : Java.Lang.Object, IRunnable
        {

            //#TODO must add image file to an ObservableCollection

            // The JPEG image
            private Image mImage;

            // The file we save the image into.
            private File mFile;

            public ImageSaver(Image image, File file)
            {
                mImage = image ?? throw new ArgumentNullException("image");
                mFile = file ?? throw new ArgumentNullException("file");
            }

            public void Run()
            {
                ByteBuffer buffer = mImage.GetPlanes()[0].Buffer;
                byte[] bytes = new byte[buffer.Remaining()];
                buffer.Get(bytes);
                using (var output = new FileOutputStream(mFile))
                {
                    try
                    {
                        output.Write(bytes);
                    }
                    catch (IOException e)
                    {
                        e.PrintStackTrace();
                    }
                    finally
                    {
                        mImage.Close();
                    }
                }
            }
        }
    }
}
