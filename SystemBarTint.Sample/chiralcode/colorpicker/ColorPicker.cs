/*
 * Copyright 2013 Piotr Adamus
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

//package com.chiralcode.colorpicker;

//import android.annotation.SuppressLint;
//import android.content.Context;
//import android.graphics.Bitmap;
//import android.graphics.Bitmap.Config;
//import android.graphics.Canvas;
//import android.graphics.Color;
//import android.graphics.ComposeShader;
//import android.graphics.Matrix;
//import android.graphics.Paint;
//import android.graphics.Paint.Join;
//import android.graphics.Paint.Style;
//import android.graphics.Path;
//import android.graphics.PorterDuff;
//import android.graphics.RadialGradient;
//import android.graphics.RectF;
//import android.graphics.Shader.TileMode;
//import android.graphics.SweepGradient;
//import android.os.Bundle;
//import android.os.Parcelable;
//import android.util.AttributeSet;
//import android.view.MotionEvent;
//import android.view.View;


using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Util;
using Android.Views;
using Java.Lang;


namespace Com.ChiralCode.ColorPicker
{
    public class ColorPicker : View
    {

        /**
         * Customizable display parameters (in percents)
         */
        private readonly int paramOuterPadding = 2; // outer padding of the whole color picker view
        private readonly int paramInnerPadding = 5; // distance between value slider wheel and inner color wheel
        private readonly int paramValueSliderWidth = 10; // width of the value slider
        private readonly int paramArrowPointerSize = 4; // size of the arrow pointer; set to 0 to hide the pointer

        private Paint colorWheelPaint;
        private Paint valueSliderPaint;

        private Paint colorViewPaint;

        private Paint colorPointerPaint;
        private RectF colorPointerCoords;

        private Paint valuePointerPaint;
        private Paint valuePointerArrowPaint;

        private RectF outerWheelRect;
        private RectF innerWheelRect;

        private Path colorViewPath;
        private Path valueSliderPath;
        private Path arrowPointerPath;

        private Bitmap colorWheelBitmap;

        private int valueSliderWidth;
        private int innerPadding;
        private int outerPadding;

        private int arrowPointerSize;
        private int outerWheelRadius;
        private int innerWheelRadius;
        private int colorWheelRadius;

        private Matrix gradientRotationMatrix;

        /** Currently selected color */
        private float[] colorHSV = new float[] { 0f, 0f, 1f };

        public ColorPicker(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            init();
        }

        public ColorPicker(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
            init();
        }

        public ColorPicker(Context context)
            : base(context)
        {
            init();
        }

        private void init()
        {

            colorPointerPaint = new Paint();

            colorPointerPaint.SetStyle(Paint.Style.Stroke);
            colorPointerPaint.StrokeWidth = 2f;
            colorPointerPaint.SetARGB(128, 0, 0, 0);

            valuePointerPaint = new Paint();
            valuePointerPaint.SetStyle(Paint.Style.Stroke);
            valuePointerPaint.StrokeWidth = 2f;

            valuePointerArrowPaint = new Paint();

            colorWheelPaint = new Paint();
            colorWheelPaint.AntiAlias = true;
            colorWheelPaint.Dither = true;

            valueSliderPaint = new Paint();
            valueSliderPaint.AntiAlias = true;
            valueSliderPaint.Dither = true;

            colorViewPaint = new Paint();
            colorViewPaint.AntiAlias = true;

            colorViewPath = new Path();
            valueSliderPath = new Path();
            arrowPointerPath = new Path();

            outerWheelRect = new RectF();
            innerWheelRect = new RectF();

            colorPointerCoords = new RectF();

        }

        //@Override
        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            int widthSize = MeasureSpec.GetSize(widthMeasureSpec);
            int heightSize = MeasureSpec.GetSize(heightMeasureSpec);
            int size = Math.Min(widthSize, heightSize);
            SetMeasuredDimension(size, size);
        }

        //@SuppressLint("DrawAllocation")
        //@Override
        protected override void OnDraw(Canvas canvas)
        {

            int centerX = Width / 2;
            int centerY = Height / 2;

            // drawing color wheel

            canvas.DrawBitmap(colorWheelBitmap, centerX - colorWheelRadius, centerY - colorWheelRadius, null);

            // drawing color view

            colorViewPaint.Color = Color.HSVToColor(colorHSV);
            canvas.DrawPath(colorViewPath, colorViewPaint);

            // drawing value slider

            float[] hsv = new float[] { colorHSV[0], colorHSV[1], 1f };

            SweepGradient sweepGradient = new SweepGradient(centerX, centerY, new int[] { Color.Black, Color.HSVToColor(hsv), Color.White }, null);
            sweepGradient.SetLocalMatrix(gradientRotationMatrix);
            valueSliderPaint.SetShader(sweepGradient);

            canvas.DrawPath(valueSliderPath, valueSliderPaint);

            // drawing color wheel pointer

            float hueAngle = (float)Math.ToRadians(colorHSV[0]);
            int colorPointX = (int)(-Math.Cos(hueAngle) * colorHSV[1] * colorWheelRadius) + centerX;
            int colorPointY = (int)(-Math.Sin(hueAngle) * colorHSV[1] * colorWheelRadius) + centerY;

            float pointerRadius = 0.075f * colorWheelRadius;
            int pointerX = (int)(colorPointX - pointerRadius / 2);
            int pointerY = (int)(colorPointY - pointerRadius / 2);

            colorPointerCoords.Set(pointerX, pointerY, pointerX + pointerRadius, pointerY + pointerRadius);
            canvas.DrawOval(colorPointerCoords, colorPointerPaint);

            // drawing value pointer

            valuePointerPaint.Color = Color.HSVToColor(new float[] { 0f, 0f, 1f - colorHSV[2] });

            double valueAngle = (colorHSV[2] - 0.5f) * Math.Pi;
            float valueAngleX = (float)Math.Cos(valueAngle);
            float valueAngleY = (float)Math.Sin(valueAngle);

            canvas.DrawLine(valueAngleX * innerWheelRadius + centerX, valueAngleY * innerWheelRadius + centerY, valueAngleX * outerWheelRadius + centerX,
                    valueAngleY * outerWheelRadius + centerY, valuePointerPaint);

            // drawing pointer arrow

            if (arrowPointerSize > 0)
            {
                drawPointerArrow(canvas);
            }

        }

        private void drawPointerArrow(Canvas canvas)
        {

            int centerX = Width / 2;
            int centerY = Height / 2;

            double tipAngle = (colorHSV[2] - 0.5f) * Math.Pi;
            double leftAngle = tipAngle + Math.Pi / 96;
            double rightAngle = tipAngle - Math.Pi / 96;

            double tipAngleX = Math.Cos(tipAngle) * outerWheelRadius;
            double tipAngleY = Math.Sin(tipAngle) * outerWheelRadius;
            double leftAngleX = Math.Cos(leftAngle) * (outerWheelRadius + arrowPointerSize);
            double leftAngleY = Math.Sin(leftAngle) * (outerWheelRadius + arrowPointerSize);
            double rightAngleX = Math.Cos(rightAngle) * (outerWheelRadius + arrowPointerSize);
            double rightAngleY = Math.Sin(rightAngle) * (outerWheelRadius + arrowPointerSize);

            arrowPointerPath.Reset();
            arrowPointerPath.MoveTo((float)tipAngleX + centerX, (float)tipAngleY + centerY);
            arrowPointerPath.LineTo((float)leftAngleX + centerX, (float)leftAngleY + centerY);
            arrowPointerPath.LineTo((float)rightAngleX + centerX, (float)rightAngleY + centerY);
            arrowPointerPath.LineTo((float)tipAngleX + centerX, (float)tipAngleY + centerY);

            valuePointerArrowPaint.Color = Color.HSVToColor(colorHSV);
            valuePointerArrowPaint.SetStyle(Paint.Style.Fill);
            canvas.DrawPath(arrowPointerPath, valuePointerArrowPaint);

            valuePointerArrowPaint.SetStyle(Paint.Style.Stroke);
            valuePointerArrowPaint.StrokeJoin = Paint.Join.Round;
            valuePointerArrowPaint.Color = Color.Black;
            canvas.DrawPath(arrowPointerPath, valuePointerArrowPaint);

        }

        //@Override
        protected override void OnSizeChanged(int width, int height, int oldw, int oldh)
        {

            int centerX = width / 2;
            int centerY = height / 2;

            innerPadding = (int)(paramInnerPadding * width / 100);
            outerPadding = (int)(paramOuterPadding * width / 100);
            arrowPointerSize = (int)(paramArrowPointerSize * width / 100);
            valueSliderWidth = (int)(paramValueSliderWidth * width / 100);

            outerWheelRadius = width / 2 - outerPadding - arrowPointerSize;
            innerWheelRadius = outerWheelRadius - valueSliderWidth;
            colorWheelRadius = innerWheelRadius - innerPadding;

            outerWheelRect.Set(centerX - outerWheelRadius, centerY - outerWheelRadius, centerX + outerWheelRadius, centerY + outerWheelRadius);
            innerWheelRect.Set(centerX - innerWheelRadius, centerY - innerWheelRadius, centerX + innerWheelRadius, centerY + innerWheelRadius);

            colorWheelBitmap = createColorWheelBitmap(colorWheelRadius * 2, colorWheelRadius * 2);

            gradientRotationMatrix = new Matrix();
            gradientRotationMatrix.PreRotate(270, width / 2, height / 2);

            colorViewPath.ArcTo(outerWheelRect, 270, -180);
            colorViewPath.ArcTo(innerWheelRect, 90, 180);

            valueSliderPath.ArcTo(outerWheelRect, 270, 180);
            valueSliderPath.ArcTo(innerWheelRect, 90, -180);

        }

        private Bitmap createColorWheelBitmap(int width, int height)
        {

            Bitmap bitmap = Bitmap.CreateBitmap(width, height, Bitmap.Config.Argb8888);

            int colorCount = 12;
            int colorAngleStep = 360 / 12;
            int[] colors = new int[colorCount + 1];
            float[] hsv = new float[] { 0f, 1f, 1f };
            for (int i = 0; i < colors.Length; i++)
            {
                hsv[0] = (i * colorAngleStep + 180) % 360;
                colors[i] = Color.HSVToColor(hsv);
            }
            colors[colorCount] = colors[0];

            SweepGradient sweepGradient = new SweepGradient(width / 2, height / 2, colors, null);

            RadialGradient radialGradient = new RadialGradient(width / 2, height / 2, colorWheelRadius, new Color(0xFFFFFF), new Color(0x00FFFFFF), Shader.TileMode.Clamp);
            ComposeShader composeShader = new ComposeShader(sweepGradient, radialGradient, PorterDuff.Mode.SrcOver);

            colorWheelPaint.SetShader(composeShader);

            Canvas canvas = new Canvas(bitmap);
            canvas.DrawCircle(width / 2, height / 2, colorWheelRadius, colorWheelPaint);

            return bitmap;

        }

        //@Override
        public override bool OnTouchEvent(MotionEvent mevent)
        {
            MotionEventActions action = mevent.Action;
            switch (action)
            {
                case MotionEventActions.Down:
                case MotionEventActions.Move:

                    int x = (int)mevent.GetX();
                    int y = (int)mevent.GetY();
                    int cx = x - Width / 2;
                    int cy = y - Height / 2;
                    double d = Math.Sqrt(cx * cx + cy * cy);

                    if (d <= colorWheelRadius)
                    {

                        colorHSV[0] = (float)(Math.ToDegrees(Math.Atan2(cy, cx)) + 180f);
                        colorHSV[1] = Math.Max(0f, Math.Min(1f, (float)(d / colorWheelRadius)));

                        Invalidate();

                    }
                    else if (x >= Width / 2 && d >= innerWheelRadius)
                    {

                        colorHSV[2] = (float)Math.Max(0, Math.Min(1, Math.Atan2(cy, cx) / Math.Pi + 0.5f));

                        Invalidate();
                    }

                    return true;
            }
            return base.OnTouchEvent(mevent);
        }

        public void setColor(Color color)
        {
            Color.ColorToHSV(color, colorHSV);
        }

        public int getColor()
        {
            return Color.HSVToColor(colorHSV);
        }

        //@Override
        protected override IParcelable OnSaveInstanceState()
        {
            Bundle state = new Bundle();
            state.PutFloatArray("color", colorHSV);
            state.PutParcelable("super", base.OnSaveInstanceState());
            return state;
        }

        //@Override
        protected override void OnRestoreInstanceState(IParcelable state)
        {
            if (state is Bundle)
            {
                Bundle bundle = (Bundle)state;
                colorHSV = bundle.GetFloatArray("color");
                base.OnRestoreInstanceState((IParcelable)bundle.GetParcelable("super"));
            }
            else
            {
                base.OnRestoreInstanceState(state);
            }
        }

    }
}