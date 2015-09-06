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

//import static android.content.Intent.ACTION_MAIN;

//import java.util.ArrayList;
//import java.util.HashMap;
//import java.util.List;
//import java.util.Map;

//import android.app.ListActivity;
//import android.content.Intent;
//import android.content.pm.PackageManager;
//import android.content.pm.ResolveInfo;
//import android.os.Bundle;
//import android.view.LayoutInflater;
//import android.view.View;
//import android.view.ViewGroup;
//import android.widget.BaseAdapter;
//import android.widget.ListView;
//import android.widget.TextView;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace SystemBarTint.Sample
{
    [Activity(Label = "SystemBarTint.Net.Sample", MainLauncher = true, Icon = "@drawable/ic_launcher")]
    public class SamplesListActivity : ListActivity
    {

        private IntentAdapter mAdapter;

        //@Override
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            mAdapter = new IntentAdapter(this);
            ListAdapter = mAdapter;
            //setListAdapter(mAdapter);
            mAdapter.refresh();
        }

        //@Override

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            StartActivity((Intent)mAdapter.GetItem(position));
        }

        private class IntentAdapter : BaseAdapter
        {

            private List<string> mNames;
            private Dictionary<string, Intent> mIntents;
            private Context mContext;

            public IntentAdapter(Context context)
            {
                mNames = new List<string>();
                mIntents = new Dictionary<string, Intent>();
                mContext = context;
            }

            public void refresh()
            {
                mNames.Clear();
                mIntents.Clear();

                Intent mainIntent = new Intent(Intent.ActionMain, null);
                //mainIntent.AddCategory("com.readystatesoftware.systembartint.SAMPLE");
                //mainIntent.AddCategory("systembartint.sample");
                
                //PackageManager pm = mContext.PackageManager;
                //IList<ResolveInfo> matches = pm.QueryIntentActivities(mainIntent, 0);
                //foreach (ResolveInfo match in matches)
                //{
                //    Intent intent = new Intent();
                //    intent.SetClassName(match.ActivityInfo.PackageName,
                //            match.ActivityInfo.Name);
                //    string name = match.LoadLabel(pm);
                //    mNames.Add(name);
                //    mIntents.Add(name, intent);
                //}

                mNames.Add("DefaultActivity");
                mIntents.Add("DefaultActivity", new Intent(mContext, typeof(DefaultActivity)));
                mNames.Add("ColorActivity");
                mIntents.Add("ColorActivity", new Intent(mContext, typeof(ColorActivity)));
                mNames.Add("MatchActionBarActivity");
                mIntents.Add("MatchActionBarActivity", new Intent(mContext, typeof(MatchActionBarActivity)));

                NotifyDataSetChanged();
            }

            //@Override

            public override int Count
            {
                get { return mNames.Count; }
            }


            //@Override
            public override Java.Lang.Object GetItem(int position)
            {
                return mIntents[mNames[position]];
            }

            //@Override		 
            public override long GetItemId(int position)
            {
                return position;
            }

            //@Override
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                TextView tv = (TextView)convertView;
                if (convertView == null)
                {
                    tv = (TextView)LayoutInflater.From(mContext)
                            .Inflate(Android.Resource.Layout.SimpleListItem1, parent,
                                    false);
                }
                tv.Text = mNames[position];
                return tv;
            }


        }

    }
}