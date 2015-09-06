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

//import android.annotation.TargetApi;
//import android.app.Activity;
//import android.os.Build;
//import android.os.Bundle;
//import android.view.Window;
//import android.view.WindowManager;

using Android.Annotation;
//import com.readystatesoftware.systembartint.SystemBarTintManager;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Com.ReadystateSoftware.SystembarTint;


namespace SystemBarTint.Sample
{
    [Activity(Label = "MatchActionBarActivity.Net", Theme = "@style/ActionBarTheme")]
    [IntentFilter(new[] { Intent.ActionMain }, Categories = new string[] { "com.readystatesoftware.systembartint.SAMPLE" })]
    public class MatchActionBarActivity : Activity
    {

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_match_actionbar);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                setTranslucentStatus(true);
            }

            SystemBarTintManager tintManager = new SystemBarTintManager(this);
            tintManager.setStatusBarTintEnabled(true);
            tintManager.setStatusBarTintResource(Resource.Color.statusbar_bg);

        }

        //@TargetApi(19) 
        [TargetApi(Value=19)]
        private void setTranslucentStatus(bool on)
        {
            Android.Views.Window win = Window;
            WindowManagerLayoutParams winParams = win.Attributes;
            WindowManagerFlags bits = WindowManagerFlags.TranslucentStatus;
            if (on)
            {
                winParams.Flags |= bits;
            }
            else
            {
                winParams.Flags &= ~bits;
            }
            win.Attributes = winParams;
        }

    }
}