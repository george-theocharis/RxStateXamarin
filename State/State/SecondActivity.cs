using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace State
{
    [Activity(Label = "State2")]
    class SecondActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
        }

        public override void OnBackPressed()
        {
            base.OnBackPressed();

            StartActivity(new Intent(this, typeof(MainActivity)));
            Finish();
        }
    }
}