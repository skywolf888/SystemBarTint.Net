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

//package com.readystatesoftware.systembartint;

//import android.annotation.SuppressLint;
//import android.annotation.TargetApi;
//import android.app.Activity;
//import android.content.Context;
//import android.content.res.Configuration;
//import android.content.res.Resources;
//import android.content.res.TypedArray;
//import android.graphics.drawable.Drawable;
//import android.os.Build;
//import android.util.DisplayMetrics;
//import android.util.TypedValue;
//import android.view.Gravity;
//import android.view.View;
//import android.view.ViewConfiguration;
//import android.view.ViewGroup;
//import android.view.Window;
//import android.view.WindowManager;
//import android.widget.FrameLayout.LayoutParams;

//import java.lang.reflect.Method;


using Android.Annotation;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Lang.Reflect;
using System;


namespace Com.ReadystateSoftware.SystembarTint
{
    /**
     * Class to manage status and navigation bar tint effects when using KitKat 
     * translucent system UI modes.
     *
     */
    public class SystemBarTintManager
    {

        //static SystemBarTintManager {
        //    // Android allows a system property to override the presence of the navigation bar.
        //    // Used by the emulator.
        //    // See https://github.com/android/platform_frameworks_base/blob/master/policy/src/com/android/internal/policy/impl/PhoneWindowManager.java#L1076
        //    if (Build.VERSION.SDKINT >= BuildVersionCodes.KITKAT) {
        //        try {
        //            Class c = Class.forName("android.os.SystemProperties");
        //            Method m = c.getDeclaredMethod("get", String.class);
        //            m.setAccessible(true);
        //            sNavBarOverride = (String) m.invoke(null, "qemu.hw.mainkeys");
        //        } catch (Throwable e) {
        //            sNavBarOverride = null;
        //        }
        //    }
        //}


        static void main()
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                try
                {
                    Class c = Class.ForName("android.os.SystemProperties");
                    Java.Lang.String ss = new Java.Lang.String();

                    Method m = c.GetDeclaredMethod("get", new Class[] { ss.Class });
                    m.Accessible = true;
                    sNavBarOverride = (string)m.Invoke(null, "qemu.hw.mainkeys");
                }
                catch (Throwable e)
                {
                    sNavBarOverride = null;
                }
            }
        }

        /**
         * The default system bar tint color value.
         */
        public static readonly Color DEFAULT_TINT_COLOR = new Color(0x990000);

        private static string sNavBarOverride;

        private SystemBarConfig mConfig;
        private bool mStatusBarAvailable;
        private bool mNavBarAvailable;
        private bool mStatusBarTintEnabled;
        private bool mNavBarTintEnabled;
        private View mStatusBarTintView;
        private View mNavBarTintView;

        /**
         * Constructor. Call this in the host activity onCreate method after its
         * content view has been set. You should always create new instances when
         * the host activity is recreated.
         *
         * @param activity The host activity.
         */
        ////@TargetApi(19)
        [Android.Runtime.Register("android/annotation/TargetApi", DoNotGenerateAcw = true)]
        [TargetApi(Value=19)]
        public SystemBarTintManager(Activity activity)
        {

            Window win = activity.Window;
            ViewGroup decorViewGroup = (ViewGroup)win.DecorView;

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat)
            {
                // check theme attrs
                int[] attrs = {Android.Resource.Attribute.WindowTranslucentStatus,
                    Android.Resource.Attribute.WindowTranslucentNavigation};
                TypedArray a = activity.ObtainStyledAttributes(attrs);
                try
                {
                    mStatusBarAvailable = a.GetBoolean(0, false);
                    mNavBarAvailable = a.GetBoolean(1, false);
                }
                finally
                {
                    a.Recycle();
                }


                // check window flags
                //IWindowManager.LayoutParams 
                WindowManagerLayoutParams winParams = win.Attributes;
                WindowManagerFlags bits = WindowManagerFlags.TranslucentStatus;
                if ((winParams.Flags & bits) != 0)
                {
                    mStatusBarAvailable = true;
                }
                bits = WindowManagerFlags.TranslucentNavigation;
                if ((winParams.Flags & bits) != 0)
                {
                    mNavBarAvailable = true;
                }
            }

            mConfig = new SystemBarConfig(activity, mStatusBarAvailable, mNavBarAvailable);
            // device might not have virtual navigation keys
            if (!mConfig.hasNavigtionBar())
            {
                mNavBarAvailable = false;
            }

            if (mStatusBarAvailable)
            {
                setupStatusBarView(activity, decorViewGroup);
            }
            if (mNavBarAvailable)
            {
                setupNavBarView(activity, decorViewGroup);
            }

        }

        /**
         * Enable tinting of the system status bar.
         *
         * If the platform is running Jelly Bean or earlier, or translucent system
         * UI modes have not been enabled in either the theme or via window flags,
         * then this method does nothing.
         *
         * @param enabled True to enable tinting, false to disable it (default).
         */
        public void setStatusBarTintEnabled(bool enabled)
        {
            mStatusBarTintEnabled = enabled;
            if (mStatusBarAvailable)
            {
                mStatusBarTintView.Visibility = enabled ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        /**
         * Enable tinting of the system navigation bar.
         *
         * If the platform does not have soft navigation keys, is running Jelly Bean
         * or earlier, or translucent system UI modes have not been enabled in either
         * the theme or via window flags, then this method does nothing.
         *
         * @param enabled True to enable tinting, false to disable it (default).
         */
        public void setNavigationBarTintEnabled(bool enabled)
        {
            mNavBarTintEnabled = enabled;
            if (mNavBarAvailable)
            {
                mNavBarTintView.Visibility = enabled ? ViewStates.Visible : ViewStates.Gone;
            }
        }

        /**
         * Apply the specified color tint to all system UI bars.
         *
         * @param color The color of the background tint.
         */
        public void setTintColor(Color color)
        {
            setStatusBarTintColor(color);
            setNavigationBarTintColor(color);
        }

        /**
         * Apply the specified drawable or color resource to all system UI bars.
         *
         * @param res The identifier of the resource.
         */
        public void setTintResource(int res)
        {
            setStatusBarTintResource(res);
            setNavigationBarTintResource(res);
        }

        /**
         * Apply the specified drawable to all system UI bars.
         *
         * @param drawable The drawable to use as the background, or null to remove it.
         */
        public void setTintDrawable(Drawable drawable)
        {
            setStatusBarTintDrawable(drawable);
            setNavigationBarTintDrawable(drawable);
        }

        /**
         * Apply the specified alpha to all system UI bars.
         *
         * @param alpha The alpha to use
         */
        public void setTintAlpha(float alpha)
        {
            setStatusBarAlpha(alpha);
            setNavigationBarAlpha(alpha);
        }

        /**
         * Apply the specified color tint to the system status bar.
         *
         * @param color The color of the background tint.
         */
        public void setStatusBarTintColor(Color color)
        {
            if (mStatusBarAvailable)
            {
                mStatusBarTintView.SetBackgroundColor(color);
            }
        }

        /**
         * Apply the specified drawable or color resource to the system status bar.
         *
         * @param res The identifier of the resource.
         */
        public void setStatusBarTintResource(int res)
        {
            if (mStatusBarAvailable)
            {
                mStatusBarTintView.SetBackgroundResource(res);
            }
        }

        /**
         * Apply the specified drawable to the system status bar.
         *
         * @param drawable The drawable to use as the background, or null to remove it.
         */
        //@SuppressWarnings("deprecation")
        public void setStatusBarTintDrawable(Drawable drawable)
        {
            if (mStatusBarAvailable)
            {
                mStatusBarTintView.SetBackgroundDrawable(drawable);
            }
        }

        /**
         * Apply the specified alpha to the system status bar.
         *
         * @param alpha The alpha to use
         */
        //@TargetApi(11)
        [TargetApi(Value = 11)]
        public void setStatusBarAlpha(float alpha)
        {
            if (mStatusBarAvailable && Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                mStatusBarTintView.Alpha = alpha;
            }
        }

        /**
         * Apply the specified color tint to the system navigation bar.
         *
         * @param color The color of the background tint.
         */
        public void setNavigationBarTintColor(Color color)
        {
            if (mNavBarAvailable)
            {
                mNavBarTintView.SetBackgroundColor(color);
            }
        }

        /**
         * Apply the specified drawable or color resource to the system navigation bar.
         *
         * @param res The identifier of the resource.
         */
        public void setNavigationBarTintResource(int res)
        {
            if (mNavBarAvailable)
            {
                mNavBarTintView.SetBackgroundResource(res);
            }
        }

        /**
         * Apply the specified drawable to the system navigation bar.
         *
         * @param drawable The drawable to use as the background, or null to remove it.
         */
        //@SuppressWarnings("deprecation")
        [SuppressWarnings(Value = new string[] { "deprecation" })]
        public void setNavigationBarTintDrawable(Drawable drawable)
        {
            if (mNavBarAvailable)
            {
                mNavBarTintView.SetBackgroundDrawable(drawable);
            }
        }

        /**
         * Apply the specified alpha to the system navigation bar.
         *
         * @param alpha The alpha to use
         */
        //@TargetApi(11)
        [TargetApi(Value = 11)]
        public void setNavigationBarAlpha(float alpha)
        {
            if (mNavBarAvailable && Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb)
            {
                mNavBarTintView.Alpha = alpha;
            }
        }

        /**
         * Get the system bar configuration.
         *
         * @return The system bar configuration for the current device configuration.
         */
        public SystemBarConfig getConfig()
        {
            return mConfig;
        }

        /**
         * Is tinting enabled for the system status bar?
         *
         * @return True if enabled, False otherwise.
         */
        public bool isStatusBarTintEnabled()
        {
            return mStatusBarTintEnabled;
        }

        /**
         * Is tinting enabled for the system navigation bar?
         *
         * @return True if enabled, False otherwise.
         */
        public bool isNavBarTintEnabled()
        {
            return mNavBarTintEnabled;
        }

        private void setupStatusBarView(Context context, ViewGroup decorViewGroup)
        {
            mStatusBarTintView = new View(context);

            FrameLayout.LayoutParams lparams = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MatchParent, mConfig.getStatusBarHeight());
            lparams.Gravity = GravityFlags.Top;
            if (mNavBarAvailable && !mConfig.isNavigationAtBottom())
            {
                lparams.RightMargin = mConfig.getNavigationBarWidth();
            }
            mStatusBarTintView.LayoutParameters = lparams;
            mStatusBarTintView.SetBackgroundColor(DEFAULT_TINT_COLOR);
            mStatusBarTintView.Visibility = ViewStates.Gone;
            decorViewGroup.AddView(mStatusBarTintView);
        }

        private void setupNavBarView(Context context, ViewGroup decorViewGroup)
        {
            mNavBarTintView = new View(context);
            FrameLayout.LayoutParams lparams;
            if (mConfig.isNavigationAtBottom())
            {
                lparams = new FrameLayout.LayoutParams(FrameLayout.LayoutParams.MatchParent, mConfig.getNavigationBarHeight());
                lparams.Gravity = GravityFlags.Bottom;
            }
            else
            {
                lparams = new FrameLayout.LayoutParams(mConfig.getNavigationBarWidth(), FrameLayout.LayoutParams.MatchParent);
                lparams.Gravity = GravityFlags.Right;
            }
            mNavBarTintView.LayoutParameters = lparams;
            mNavBarTintView.SetBackgroundColor(DEFAULT_TINT_COLOR);
            mNavBarTintView.Visibility = ViewStates.Gone;
            decorViewGroup.AddView(mNavBarTintView);
        }

        /**
         * Class which describes system bar sizing and other characteristics for the current
         * device configuration.
         *
         */
        public class SystemBarConfig
        {

            private static readonly string STATUS_BAR_HEIGHT_RES_NAME = "status_bar_height";
            private static readonly string NAV_BAR_HEIGHT_RES_NAME = "navigation_bar_height";
            private static readonly string NAV_BAR_HEIGHT_LANDSCAPE_RES_NAME = "navigation_bar_height_landscape";
            private static readonly string NAV_BAR_WIDTH_RES_NAME = "navigation_bar_width";
            private static readonly string SHOW_NAV_BAR_RES_NAME = "config_showNavigationBar";

            private bool mTranslucentStatusBar;
            private bool mTranslucentNavBar;
            private int mStatusBarHeight;
            private int mActionBarHeight;
            private bool mHasNavigationBar;
            private int mNavigationBarHeight;
            private int mNavigationBarWidth;
            private bool mInPortrait;
            private float mSmallestWidthDp;

            public SystemBarConfig(Activity activity, bool translucentStatusBar, bool traslucentNavBar)
            {
                Resources res = activity.Resources;
                mInPortrait = (res.Configuration.Orientation == Android.Content.Res.Orientation.Portrait);
                mSmallestWidthDp = getSmallestWidthDp(activity);
                mStatusBarHeight = getInternalDimensionSize(res, STATUS_BAR_HEIGHT_RES_NAME);
                mActionBarHeight = getActionBarHeight(activity);
                mNavigationBarHeight = getNavigationBarHeight(activity);
                mNavigationBarWidth = getNavigationBarWidth(activity);
                mHasNavigationBar = (mNavigationBarHeight > 0);
                mTranslucentStatusBar = translucentStatusBar;
                mTranslucentNavBar = traslucentNavBar;
            }

            ////@TargetApi(14)
            [TargetApi(Value=14)]
            private int getActionBarHeight(Context context)
            {
                int result = 0;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
                {
                    TypedValue tv = new TypedValue();
                    context.Theme.ResolveAttribute(Android.Resource.Attribute.ActionBarSize, tv, true);
                    result = TypedValue.ComplexToDimensionPixelSize(tv.Data, context.Resources.DisplayMetrics);
                }
                return result;
            }

            ////@TargetApi(14)
            [TargetApi(Value = 14)]
            private int getNavigationBarHeight(Context context)
            {
                Resources res = context.Resources;
                int result = 0;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
                {
                    if (hasNavBar(context))
                    {
                        string key;
                        if (mInPortrait)
                        {
                            key = NAV_BAR_HEIGHT_RES_NAME;
                        }
                        else
                        {
                            key = NAV_BAR_HEIGHT_LANDSCAPE_RES_NAME;
                        }
                        return getInternalDimensionSize(res, key);
                    }
                }
                return result;
            }

            //@TargetApi(14)
            [TargetApi(Value = 14)]
            private int getNavigationBarWidth(Context context)
            {
                Resources res = context.Resources;
                int result = 0;
                if (Build.VERSION.SdkInt >= BuildVersionCodes.IceCreamSandwich)
                {
                    if (hasNavBar(context))
                    {
                        return getInternalDimensionSize(res, NAV_BAR_WIDTH_RES_NAME);
                    }
                }
                return result;
            }

            //@TargetApi(14)
            [TargetApi(Value = 14)]
            private bool hasNavBar(Context context)
            {
                Resources res = context.Resources;
                int resourceId = res.GetIdentifier(SHOW_NAV_BAR_RES_NAME, "bool", "android");
                if (resourceId != 0)
                {
                    bool hasNav = res.GetBoolean(resourceId);
                    // check override flag (see static block)
                    if ("1".Equals(sNavBarOverride))
                    {
                        hasNav = false;
                    }
                    else if ("0".Equals(sNavBarOverride))
                    {
                        hasNav = true;
                    }
                    return hasNav;
                }
                else
                { // fallback
                    return !ViewConfiguration.Get(context).HasPermanentMenuKey;
                }
            }

            private int getInternalDimensionSize(Resources res, string key)
            {
                int result = 0;
                int resourceId = res.GetIdentifier(key, "dimen", "android");
                if (resourceId > 0)
                {
                    result = res.GetDimensionPixelSize(resourceId);
                }
                return result;
            }

            //@SuppressLint("NewApi")
            [SuppressLint(Value=new string[]{"NewApi"})]
            private float getSmallestWidthDp(Activity activity)
            {
                DisplayMetrics metrics = new DisplayMetrics();
                if (Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean)
                {
                    activity.WindowManager.DefaultDisplay.GetRealMetrics(metrics);
                }
                else
                {
                    // TODO this is not correct, but we don't really care pre-kitkat
                    activity.WindowManager.DefaultDisplay.GetMetrics(metrics);
                }
                float widthDp = metrics.WidthPixels / metrics.Density;
                float heightDp = metrics.HeightPixels / metrics.Density;
                return System.Math.Min(widthDp, heightDp);
            }

            /**
             * Should a navigation bar appear at the bottom of the screen in the current
             * device configuration? A navigation bar may appear on the right side of
             * the screen in certain configurations.
             *
             * @return True if navigation should appear at the bottom of the screen, False otherwise.
             */
            public bool isNavigationAtBottom()
            {
                return (mSmallestWidthDp >= 600 || mInPortrait);
            }

            /**
             * Get the height of the system status bar.
             *
             * @return The height of the status bar (in pixels).
             */
            public int getStatusBarHeight()
            {
                return mStatusBarHeight;
            }

            /**
             * Get the height of the action bar.
             *
             * @return The height of the action bar (in pixels).
             */
            public int getActionBarHeight()
            {
                return mActionBarHeight;
            }

            /**
             * Does this device have a system navigation bar?
             *
             * @return True if this device uses soft key navigation, False otherwise.
             */
            public bool hasNavigtionBar()
            {
                return mHasNavigationBar;
            }

            /**
             * Get the height of the system navigation bar.
             *
             * @return The height of the navigation bar (in pixels). If the device does not have
             * soft navigation keys, this will always return 0.
             */
            public int getNavigationBarHeight()
            {
                return mNavigationBarHeight;
            }

            /**
             * Get the width of the system navigation bar when it is placed vertically on the screen.
             *
             * @return The width of the navigation bar (in pixels). If the device does not have
             * soft navigation keys, this will always return 0.
             */
            public int getNavigationBarWidth()
            {
                return mNavigationBarWidth;
            }

            /**
             * Get the layout inset for any system UI that appears at the top of the screen.
             *
             * @param withActionBar True to include the height of the action bar, False otherwise.
             * @return The layout inset (in pixels).
             */
            public int getPixelInsetTop(bool withActionBar)
            {
                return (mTranslucentStatusBar ? mStatusBarHeight : 0) + (withActionBar ? mActionBarHeight : 0);
            }

            /**
             * Get the layout inset for any system UI that appears at the bottom of the screen.
             *
             * @return The layout inset (in pixels).
             */
            public int getPixelInsetBottom()
            {
                if (mTranslucentNavBar && isNavigationAtBottom())
                {
                    return mNavigationBarHeight;
                }
                else
                {
                    return 0;
                }
            }

            /**
             * Get the layout inset for any system UI that appears at the right of the screen.
             *
             * @return The layout inset (in pixels).
             */
            public int getPixelInsetRight()
            {
                if (mTranslucentNavBar && !isNavigationAtBottom())
                {
                    return mNavigationBarWidth;
                }
                else
                {
                    return 0;
                }
            }

        }

    }
}