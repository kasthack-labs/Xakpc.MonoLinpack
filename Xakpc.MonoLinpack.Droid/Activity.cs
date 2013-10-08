using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xakpc.MonoLinpack.Core;

namespace Xakpc.MonoLinpack.Droid
{
    [Activity(Label = "Mono Linpack", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity, ILinpackView
    {
        int count = 1;

        TextView mflops;
        TextView time;
        TextView normRes;
        TextView precision;
        Button button;
        LinpackPresenter lp;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            button = FindViewById<Button>(Resource.Id.StartButton);

            mflops = FindViewById<TextView>(Resource.Id.textView1);
            time = FindViewById<TextView>(Resource.Id.textView2);
            normRes = FindViewById<TextView>(Resource.Id.textView3);
            precision = FindViewById<TextView>(Resource.Id.textView4);

            button.Click += button_Click;

            lp = new LinpackPresenter(this);
        }

        async void button_Click(object sender, EventArgs e)
        {
            try
            {
                button.Text = "Calculating...";
                button.Enabled = false;
                await lp.CalculateAsync();
                button.Enabled = true;
                button.Text = "Calculate finished. Press for repeat";
            }
            catch (Exception ex)
            {
                Android.Util.Log.Error(Title, ex.ToString());
                throw ex;
            }
           
        }

        public double Mflops
        {
            get
            {
                return Convert.ToDouble(mflops.Text);
            }
            set
            {
                mflops.Text = String.Format("MFPLOP/s: {0:F4}", value);
            }
        }

        public string Time
        {
            set { time.Text = String.Format("Time: {0}", value); }
        }

        public double NormRes
        {
            set { normRes.Text = String.Format("Norm Res: {0:F2}", value); }
        }

        public double Precision
        {
            set { precision.Text = String.Format("Precision: {0}", value); }
        }
    }
}

