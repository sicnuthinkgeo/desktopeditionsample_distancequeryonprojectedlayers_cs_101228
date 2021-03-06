﻿using System;
using System.Collections.ObjectModel;
using System.Windows.Forms;
using ThinkGeo.MapSuite.Core;
using ThinkGeo.MapSuite.DesktopEdition;

namespace  DistanceQueryOnProjectedLayers
{
    public partial class TestForm : Form
    {
        public TestForm()
        {
            InitializeComponent();
        }

        private void TestForm_Load(object sender, EventArgs e)
        {
            winformsMap1.MapUnit = GeographyUnit.DecimalDegree;
            winformsMap1.CurrentExtent = new RectangleShape(-88.0309, 42.2772, -88.0036, 42.2609);
            winformsMap1.BackgroundOverlay.BackgroundBrush = new GeoSolidBrush(GeoColor.FromArgb(255, 198, 255, 255));

            //Displays the World Map Kit as a background.
            WorldMapKitWmsDesktopOverlay worldMapKitDesktopOverlay = new WorldMapKitWmsDesktopOverlay();
            winformsMap1.Overlays.Add(worldMapKitDesktopOverlay);

            ManagedProj4Projection proj4 = new ManagedProj4Projection();
            proj4.InternalProjectionParametersString = ManagedProj4Projection.GetEpsgParametersString(26916);
            proj4.ExternalProjectionParametersString = ManagedProj4Projection.GetEpsgParametersString(4326);

            ShapeFileFeatureLayer layer1 = new ShapeFileFeatureLayer(@"..\..\data\lake_streets_utm16.shp");
            layer1.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.DarkGreen, 4, true);
            layer1.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            layer1.FeatureSource.Projection = proj4;

            ShapeFileFeatureLayer layer2 = new ShapeFileFeatureLayer(@"..\..\data\lake_poly.shp");
            layer2.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.FromArgb(150, GeoColor.StandardColors.Green), GeoColor.StandardColors.Black);
            layer2.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            LayerOverlay layerOverlay = new LayerOverlay();
            layerOverlay.Layers.Add(layer1);
            layerOverlay.Layers.Add(layer2);
            winformsMap1.Overlays.Add(layerOverlay);

            InMemoryFeatureLayer inMemoryFeatureLayer = new InMemoryFeatureLayer();
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultPointStyle = PointStyles.CreateSimpleCircleStyle(GeoColor.StandardColors.Red, 10);
            inMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;
            PointShape pointShape = new PointShape(-88.0168,42.2688);
            inMemoryFeatureLayer.InternalFeatures.Add(new Feature(pointShape));

            InMemoryFeatureLayer selectInMemoryFeatureLayer = new InMemoryFeatureLayer();
            selectInMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultAreaStyle = AreaStyles.CreateSimpleAreaStyle(GeoColor.StandardColors.Yellow);
            selectInMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.DefaultLineStyle = LineStyles.CreateSimpleLineStyle(GeoColor.StandardColors.Yellow, 3, true);
            selectInMemoryFeatureLayer.ZoomLevelSet.ZoomLevel01.ApplyUntilZoomLevel = ApplyUntilZoomLevel.Level20;

            double meterDistance = 300;

            layer1.Open();
            // This method works fine with Map Suite Assemblies 4.5.54.0 or later.
            Collection<Feature> withinFeatures1 = layer1.QueryTools.GetFeaturesWithinDistanceOf(pointShape, winformsMap1.MapUnit, DistanceUnit.Meter, meterDistance, ReturningColumnsType.AllColumns);
            layer1.Close();
            foreach (Feature withinFeature1 in withinFeatures1)
            {
                selectInMemoryFeatureLayer.InternalFeatures.Add(withinFeature1);
            }

            layer2.Open();
            Collection<Feature> withinFeatures2 = layer2.QueryTools.GetFeaturesWithinDistanceOf(pointShape, winformsMap1.MapUnit, DistanceUnit.Meter, meterDistance, ReturningColumnsType.AllColumns);
            layer2.Close();
            foreach (Feature withinFeature2 in withinFeatures2)
            {
                selectInMemoryFeatureLayer.InternalFeatures.Add(withinFeature2);
            }

            LayerOverlay dynamicOverlay = new LayerOverlay();
            dynamicOverlay.Layers.Add(inMemoryFeatureLayer);
            dynamicOverlay.Layers.Add(selectInMemoryFeatureLayer);
            winformsMap1.Overlays.Add(dynamicOverlay);

            ScaleBarAdornmentLayer scaleBarAdornmentLayer = new ScaleBarAdornmentLayer();
            scaleBarAdornmentLayer.UnitFamily = UnitSystem.Metric;
            scaleBarAdornmentLayer.Location = AdornmentLocation.LowerLeft;
            winformsMap1.AdornmentOverlay.Layers.Add(scaleBarAdornmentLayer);

            winformsMap1.Refresh();
        }

      
        private void winformsMap1_MouseMove(object sender, MouseEventArgs e)
        {
            //Displays the X and Y in screen coordinates.
            statusStrip1.Items["toolStripStatusLabelScreen"].Text = "X:" + e.X + " Y:" + e.Y;

            //Gets the PointShape in world coordinates from screen coordinates.
            PointShape pointShape = ExtentHelper.ToWorldCoordinate(winformsMap1.CurrentExtent, new ScreenPointF(e.X, e.Y), winformsMap1.Width, winformsMap1.Height);

            //Displays world coordinates.
            statusStrip1.Items["toolStripStatusLabelWorld"].Text = "(world) X:" + Math.Round(pointShape.X, 4) + " Y:" + Math.Round(pointShape.Y, 4);
        }
        
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
