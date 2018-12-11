﻿using System;
using CoreGraphics;
using Foundation;
using SkiaSharp;
using SkiaSharp.Views.iOS;
using UIKit;
using Xamarin.Forms;

namespace PhotoTaker.iOS.Controls
{
    public class UIControlsOverlayView : SKCanvasView 
    {
        private SvgButton flashButton = new SvgButton("flash_button.svg", "flash_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f));
        private SvgButton closeButton = new SvgButton("close_button.svg", "close_button_touched.svg", SKMatrix.MakeScale(0.8f, 0.8f));
        private SvgButton cameraButton = new SvgButton("camera_button.svg", "camera_button.svg", SKMatrix.MakeScale(2.5f, 2.5f));
        private SvgButton galleryButton = new SvgButton("gallery_button.svg", "gallery_button.svg", SKMatrix.MakeScale(2.5f, 2.5f));
        private SvgButton takeButton = new SvgButton("take_button.svg", "take_button_touched.svg", SKMatrix.MakeScale(1.5f, 1.5f));
        private SvgButton sendButton = new SvgButton("camera_button.svg", "camera_button.svg", SKMatrix.MakeScale(2.5f, 2.5f));

        public EventHandler TakeButtonTouched { get; set; }
        public EventHandler FlashButtonTouched { get; set; }
        public EventHandler CloseButtonTouched { get; set; }
        public EventHandler CameraButtonTouched { get; set; }

        public UIControlsOverlayView(CGRect frame)
        {
            PaintSurface += Handle_PaintSurface;
            BackgroundColor = UIColor.Clear;
            flashButton.IsToggleButton = true;

            Device.StartTimer(TimeSpan.FromMilliseconds(1000 / 60), () =>
            {
                SetNeedsLayout();
                return true;
            });
        }

        public void Handle_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (e.Surface != null)
            {
                var surface = e.Surface;
                var surfaceWidth = e.Info.Width;
                var surfaceHeight = e.Info.Height;
                var canvas = surface.Canvas;

                canvas.Clear(SKColors.Transparent);

                SKPaint paint = new SKPaint();
                paint.IsAntialias = true;

                var svgButtonTouched = new SkiaSharp.Extended.Svg.SKSvg(190f);
                svgButtonTouched.Load("take_button_touched.svg");

                var svgTakeButton = new SkiaSharp.Extended.Svg.SKSvg(190f);
                svgTakeButton.Load("take_button.svg");

                // get the size of the canvas
                float canvasMin = Math.Min(e.Info.Width, e.Info.Height);

                // get the size of the picture
                float svgMax = Math.Max(svgTakeButton.Picture.CullRect.Width, svgTakeButton.Picture.CullRect.Height);

                // get the scale to fill the screen
                float scale = 1.5f;//  canvasMin / svgMax;

                // create a scale matrix
                var matrix = SKMatrix.MakeScale(scale, scale);
                // 667 iphone 8, // 813 iphone x // 736 iphone 8 plus
                var max = (float)UIScreen.MainScreen.Bounds.Height;

                if (UIScreen.MainScreen.Scale > 2) 
                {
                    scale *= 1.5f;
                }

                var x = e.Info.Width / 2 - (svgTakeButton.Picture.CullRect.Width * scale) / 2;
                var y = e.Info.Height - 2 * svgTakeButton.Picture.CullRect.Height;

                float xOffset = 0f;
                if (max > 800) 
                {
                    y -= 100f;
                    xOffset = 30f;
                }

                surface.Canvas.Translate(x, y);

                takeButton.Draw(surface.Canvas, x, y, paint);

                x = 0 + 30f + xOffset;

                float galleryPositionX = x;
                float galleryPositionY = y + (galleryButton.SvgTouched.Picture.CullRect.Height * scale);
                // galleryButton.Draw(surface.Canvas, galleryPositionX, galleryPositionY, paint);

                float cameraPositionX = e.Info.Width - xOffset - 65f - cameraButton.SvgTouched.Picture.CullRect.Width * scale;
                float cameraPoisitonY = y + (cameraButton.SvgTouched.Picture.CullRect.Height * scale);
                cameraButton.Draw(surface.Canvas, cameraPositionX, cameraPoisitonY, paint);

                float flashPositionX = e.Info.Width - xOffset - flashButton.SvgTouched.Picture.CullRect.Width;
                float flashPositionY = xOffset + flashButton.SvgTouched.Picture.CullRect.Height;
                flashButton.Draw(surface.Canvas, flashPositionX, flashPositionY, paint);

                float closePositionX = x;
                float closePositionY = xOffset + closeButton.SvgTouched.Picture.CullRect.Height;
                closeButton.Draw(surface.Canvas, closePositionX, closePositionY, paint);

                // draw on the canvas
                canvas.Flush();
            }
        }

        public override void TouchesBegan(NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;

            var cgPoint = touch.LocationInView(this);
            var point = new SKPoint((float)this.ContentScaleFactor * (float)cgPoint.X, (float)this.ContentScaleFactor * (float)cgPoint.Y);
            var rect = new SKRect(point.X - 25f, point.Y - 25f, point.X + 50f, point.Y + 50f);

            cameraButton.CheckIntersection(rect);
            flashButton.CheckIntersection(rect);
            takeButton.CheckIntersection(rect);
            closeButton.CheckIntersection(rect);
        }

        public override void TouchesMoved(NSSet touches, UIEvent evt)
        {
            base.TouchesMoved(touches, evt);
        }

        public override void TouchesEnded(NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            UITouch touch = touches.AnyObject as UITouch;

            var cgPoint = touch.LocationInView(this);
            var point = new SKPoint((float)this.ContentScaleFactor * (float)cgPoint.X, (float)this.ContentScaleFactor * (float)cgPoint.Y);
            var rect = new SKRect(point.X - 25f, point.Y - 25f, point.X + 50f, point.Y + 50f);

            // if touch ended within current viewbox!
            if (takeButton.TouchUpInside(rect)) 
            {
                // fire Event
                TakeButtonTouched?.Invoke(this, new EventArgs());

                System.Diagnostics.Debug.WriteLine("touched Take button!");
            }

            if (flashButton.TouchUpInside(rect)) 
            {
                FlashButtonTouched?.Invoke(this, new EventArgs());

                System.Diagnostics.Debug.WriteLine("Flash button touched!");
            }

            if (cameraButton.TouchUpInside(rect))
            {
                CameraButtonTouched?.Invoke(this, new EventArgs());
            }

            if (closeButton.TouchUpInside(rect)) 
            {
                System.Diagnostics.Debug.WriteLine("Close button touched!");
            }

            takeButton.Touched = false;
            cameraButton.Touched = false;
            flashButton.Touched = false;
            galleryButton.Touched = false;
            closeButton.Touched = false;
        }

        public override void TouchesCancelled(NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);

            takeButton.Touched = false;
            cameraButton.Touched = false;
            flashButton.Touched = false;
            galleryButton.Touched = false;
            closeButton.Touched = false;
        }

        public override void Draw(CGRect rect)
        {
            this.Frame = rect;
            base.Draw(rect);
        }
    }
}
