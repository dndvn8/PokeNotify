using System;
using System.Windows.Forms;
using GMap.NET;
using MetroFramework.Forms;
using PokeNotify.Core;

namespace PokeNotify
{
    public partial class Maps : MetroForm
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
        //public double Alt { get; set; }
        public Maps(double _lat, double _lng)
        {
            Lat = _lat;
            Lng = _lng;
            InitializeComponent();
        }
        private void Maps_Load(object sender, EventArgs e)
        {
            txtLat.Text = $"{Lat}";
            txtLng.Text = $"{Lng}";
            gMap.MapProvider = GMap.NET.MapProviders.BingMapProvider.Instance;
            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gMap.DragButton = MouseButtons.Left;
            gMap.Position = new PointLatLng(Lat, Lng);
            gMap.Zoom = 15;
        }

        
        private void gMap_OnMapDrag()
        {
            var location = gMap.Position;

            txtLat.Text = $"{location.Lat}";
            txtLng.Text = $"{location.Lng}";
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            Lat = Utils.ConvertToDouble(txtLat.Text);
            Lng = Utils.ConvertToDouble(txtLng.Text);
            this.Dispose();
            this.Close();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            gMap.SetPositionByKeywords(txtAddress.Text);
            var location = gMap.Position;

            txtLat.Text = $"{location.Lat}";
            txtLng.Text = $"{location.Lng}";
        }
    }
}
