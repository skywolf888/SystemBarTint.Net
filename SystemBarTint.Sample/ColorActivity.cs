/*
 * Copyright (C) 2013 readyState Software Ltd
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

//package com.readystatesoftware.systembartint.sample;

//import android.app.Activity;
//import android.graphics.Color;
//import android.os.Bundle;
//import android.view.View;
//import android.view.View.OnClickListener;
//import android.widget.Button;

//import com.chiralcode.colorpicker.ColorPicker;
//import com.readystatesoftware.systembartint.SystemBarTintManager;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Widget;
using Com.ChiralCode.ColorPicker;
using Com.ReadystateSoftware.SystembarTint;
  
namespace SystemBarTint.Sample
{
    [Activity(Label = "SystemBarTintColorActivity.Net", Theme = "@style/FullBleedTheme")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.readystatesoftware.systembartint.SAMPLE" })]
    public class ColorActivity : Activity
    {

        private SystemBarTintManager mTintManager;
        private ColorPicker mColorPicker;
        private Button mButton;

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_color);

            mTintManager = new SystemBarTintManager(this);
            mTintManager.setStatusBarTintEnabled(true);
            mTintManager.setNavigationBarTintEnabled(true);

            mColorPicker = (ColorPicker)FindViewById(Resource.Id.color_picker);
            applySelectedColor();

            mButton = (Button)FindViewById(Resource.Id.button);
            mButton.Click  +=delegate
            {
                applySelectedColor();
            };
            //mButton.SetOnClickListener(new OnClickListener() {
            //    @Override
            //    public void onClick(View v) {
            //        applySelectedColor();
            //    }
            //});
        }

        //@Override
        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);
            applySelectedColor();
        }

        private void applySelectedColor()
        {
            int selected = mColorPicker.getColor();

            Color color = Color.Argb(153, Color.GetRedComponent(selected), Color.GetGreenComponent(selected), Color.GetBlueComponent(selected));
            mTintManager.setTintColor(color);
        }

    }
}