using Android.Widget;
using Android.OS;
using ServiceStack.ServiceClient.Web;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using ProteinTrackerAPI.Api;
using System;


namespace ProteinTrackerDroid
{
    [Activity(Label = "Protein Tracker App", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private JsonServiceClient client;
        private List<User> users;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            client = new JsonServiceClient("http://vmredisserver.cloudapp.net/api");

            PopulateSelectUsers();

            Spinner usersSpinner = FindViewById<Spinner>(Resource.Id.usersSpinner);
            usersSpinner.ItemSelected += (object sender, AdapterView.ItemSelectedEventArgs e) => {
                TextView goalTextView = FindViewById<TextView>(Resource.Id.usersGoalTextView);
                TextView totalTextView = FindViewById<TextView>(Resource.Id.usersTotalTextView);

                var selectedUser = users[e.Position];
                goalTextView.Text = selectedUser.Goal.ToString();
                totalTextView.Text = selectedUser.Total.ToString();
            };

            //Adicionando novo usuario com a sua destinada meta de proteinas ao banco de dados.
            var addUserButton = FindViewById<Button>(Resource.Id.addNewUerButton);
            addUserButton.Click += (object sender, EventArgs e) =>
            {
                TextView nameTextView = FindViewById<TextView>(Resource.Id.nameTextView2);
                TextView goalTextView = FindViewById<TextView>(Resource.Id.goalTextView2);

                var goal = int.Parse(goalTextView.Text);

                var response = client.Send(new AddUser { Goal = goal, Name = nameTextView.Text });
                PopulateSelectUsers();                         

                nameTextView.Text = string.Empty;
                goalTextView.Text = string.Empty;

                Toast.MakeText(this, "Add New User", ToastLength.Short).Show();

            };


            //Addicionando proteinas (amount) ao usuario selecionado no Spinner
            var addProteinButton = FindViewById<Button>(Resource.Id.addProteinButton);
            addProteinButton.Click += (object sender, EventArgs e) =>
            {
                TextView amountTextView = FindViewById<TextView>(Resource.Id.amountTextView);
                var amount = int.Parse(amountTextView.Text);
                var selectedUser = users[usersSpinner.SelectedItemPosition];

                var response = client.Send(new AddProtein { Amount = amount, UserId = selectedUser.Id });
                selectedUser.Total = response.NewTotal;

                TextView totalTextView = FindViewById<TextView>(Resource.Id.usersTotalTextView);
                totalTextView.Text = selectedUser.Total.ToString();
                amountTextView.Text = string.Empty;

            };

        }

        //Populando a Spinner com os usuarios cadastrados no Banco

        void PopulateSelectUsers()
        {
            var response = client.Get (new Users());
            users = response.Users.ToList();

            var names = users.Select(u => u.Name);

            var usersSpinner = FindViewById<Spinner>(Resource.Id.usersSpinner);
            usersSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, names.ToArray() );
        }
    }
}

