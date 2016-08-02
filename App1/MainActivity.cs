using System;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Android.Preferences;




namespace App1
{
    [Activity(Label = "App1", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private const string TAG = "App1";
        private const bool Debug = true;

        int count = 0;
        private ImageView ad;
        private Spinner spinner_mode;
        private BluetoothAdapter mBluetoothAdapter = null;
       

        Context mContext = Android.App.Application.Context;
        

        // Intent request codes
        // TODO: Make into Enums
        private const int REQUEST_CONNECT_DEVICE = 1;
        private const int REQUEST_ENABLE_BT = 2;


        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            var label = FindViewById<TextView>(Resource.Id.tv1);
            button.Click += delegate {
                count++;
                label.Text = $"你點擊了 {count} 次";
                //button.Text = string.Format("{0} clicks!", count++);
            };
            
            ad = FindViewById<ImageView>(Resource.Id.AD);
            ad.SetImageResource(Resource.Drawable.pikachu);

            // Get local Bluetooth adapter
            mBluetoothAdapter = BluetoothAdapter.DefaultAdapter;

            // If the adapter is null, then Bluetooth is not supported
            if (mBluetoothAdapter == null)
            {
                Toast.MakeText(this, "Bluetooth is not available", ToastLength.Long).Show();
                Finish();
                return;
            }

            //---------------------SPINNER-------------------------------  
            spinner_mode = FindViewById<Spinner>(Resource.Id.spinner1);

            AppPreferences ap = new AppPreferences(mContext);
            int spinnerPos = ap.getAccessKey();
            
            spinner_mode.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter.CreateFromResource(
                    this, Resource.Array.Connect_Mode_Array, Android.Resource.Layout.SimpleSpinnerItem);

            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner_mode.Adapter = adapter;

            if (spinner_mode != null)
            {
                spinner_mode.SetSelection(ap.getAccessKey());
            }

        }
        protected override void OnStart()
        {
            base.OnStart();

            if (Debug)
                Log.Error(TAG, "++ ON START ++");
        }

        protected override void OnPause()
        {

            //spinnerPos = spinner_mode.SelectedItemPosition;
            AppPreferences ap = new AppPreferences(mContext);
            ap.saveAccessKey(spinner_mode.SelectedItemPosition);

            base.OnPause();
        }

        protected override void OnStop()
        {
            AppPreferences ap = new AppPreferences(mContext);
            ap.saveAccessKey(spinner_mode.SelectedItemPosition);

            base.OnStop();
        }
        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            Spinner spinner = (Spinner)sender;

            string toast = string.Format("The mode is {0}", spinner.GetItemAtPosition(e.Position));
            
            Toast.MakeText(this, toast, ToastLength.Long).Show();

            btConnect(e.Position);
        }

        public void btConnect(int mode)
        {
            if (mode.Equals(1))
            {
                if (!mBluetoothAdapter.IsEnabled)
                {
                    Intent enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);
                    StartActivityForResult(enableBtIntent, REQUEST_ENABLE_BT);
                }
            }
            else
            {
                if (!mBluetoothAdapter.IsEnabled)
                    mBluetoothAdapter.Enable();
            }
        }
    }
    public class AppPreferences
    {
        private ISharedPreferences mSharedPrefs;
        private ISharedPreferencesEditor mPrefsEditor;
        private Context mContext;

        private static String PREFERENCE_ACCESS_KEY = "PREFERENCE_ACCESS_KEY";
        private static String COUNT = "COUNT";

        public AppPreferences(Context context)
        {
            this.mContext = context;
            mSharedPrefs = PreferenceManager.GetDefaultSharedPreferences(mContext);
            mPrefsEditor = mSharedPrefs.Edit();
        }

        public void saveAccessKey(int key)
        {
            mPrefsEditor.PutInt(PREFERENCE_ACCESS_KEY, key);
            //mPrefsEditor.Commit();
            mPrefsEditor.Apply();
        }

        public void saveAccessKey(string key)
        {
            mPrefsEditor.PutString(COUNT, key);
            //mPrefsEditor.Commit();
            mPrefsEditor.Apply();
        }

        public int getAccessKey()
        {
            return mSharedPrefs.GetInt(PREFERENCE_ACCESS_KEY, 0);
        }

        public string getAccessKey1()
        {
            return mSharedPrefs.GetString(COUNT, "");
        }
    }
}

