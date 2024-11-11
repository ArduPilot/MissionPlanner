using MissionPlanner;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner.Controls.PreFlight;
using MissionPlanner.Controls;
using System.Linq;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.Maps;
using GMap.NET;
using GMap.NET.WindowsForms;
using System.Globalization;
using System.Drawing;
using Microsoft.Win32;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.RightsManagement;
using Org.BouncyCastle.Utilities;
using NetTopologySuite.Operation.Valid;
using Org.BouncyCastle.Asn1.X509;

namespace TerrainMakerPlugin
{

    public class TerrainMakerPlugin : Plugin
    {


        SplitContainer sc;
        MissionPlanner.Controls.MyButton button1;

        Stopwatch stopwatch = new Stopwatch();
        public override string Name
        {
            get { return "TerrainMakerPlugin"; }
        }

        public override string Version
        {
            get { return "2.0"; }
        }

        public override string Author
        {
            get { return "Andras \"EosBandi\" Schaffer"; }
        }

        public override bool Init()
        {
            return true;	 // If it is false then plugin will not load
        }

        public override bool Loaded()
        {

            Host.FPMenuMap.Items.Add(new ToolStripMenuItem("Make Terrain DAT", null, MakeTerrainDAT));

            return true;     //If it is false plugin will not start (loop will not called)
        }


        private int lat_int_togenerate = 0;
        private int lon_int_togenerate = 0;
        private ushort spacing_togenerate = 0;



        private void MakeTerrainDAT(object sender, EventArgs e)
        {
            RectLatLng area = Host.FPGMapControl.SelectedArea;
            if (area.IsEmpty)
            {
                var res = CustomMessageBox.Show("No area defined, use area displayed on screen?", "Terrain DAT",
                    MessageBoxButtons.YesNo);
                if (res == (int)DialogResult.Yes)
                {
                    area = Host.FPGMapControl.ViewArea;
                }
            }


            if (!area.IsEmpty)
            {
                string spacingstring = "30";
                if (InputBox.Show("SPACING", "Enter the grid spacing in meters (5-100).", ref spacingstring) !=
                    DialogResult.OK)
                    return;

                int spacing = 30;
                if (!int.TryParse(spacingstring, out spacing))
                {
                    CustomMessageBox.Show("Invalid Number", "ERROR");
                    return;
                }

                if (spacing < 5 || spacing > 100)
                {
                    CustomMessageBox.Show("Spacing must be between 5 and 100 meters", "ERROR");
                    return;
                }



                //Do it in the selected area with the selected spacing
                int lat_start = (int)Math.Floor(area.Bottom);
                int lat_end = (int)Math.Ceiling(area.Top);

                int lon_start = (int)Math.Floor(area.Left);
                int lon_end = (int)Math.Ceiling(area.Right);



                for (int lat_int = lat_start; lat_int< lat_end; lat_int++)
                {
                    for (int lon_int = lon_start; lon_int < lon_end; lon_int++)
                    {
                        Console.WriteLine("Make Terrain DAT {0} {1}", lat_int, lon_int);

                        lat_int_togenerate = lat_int;
                        lon_int_togenerate = lon_int;
                        spacing_togenerate = (ushort)spacing;



                        IProgressReporterDialogue frmProgressReporter = new ProgressReporterDialogue
                        {
                            StartPosition = FormStartPosition.CenterScreen,
                            Text = String.Format("Generate terrain data for Lat {0} Lon {1}", lat_int, lon_int)
                        };

                        frmProgressReporter.DoWork += createTerrainDataFile;
                        frmProgressReporter.UpdateProgressAndStatus(-1, "Starting...");

                        ThemeManager.ApplyThemeTo(frmProgressReporter);

                        frmProgressReporter.RunBackgroundOperationAsync();

                        frmProgressReporter.Dispose();

                        CustomMessageBox.Show("Terrain DAT created in Documents/Mission Planner/TerrainDat folder", "Terrain DAT");


                    }
                }



            }


        }


        private void createTerrainDataFile(IProgressReporterDialogue sender)
        {

            int lat_int = lat_int_togenerate;
            int lon_int = lon_int_togenerate;
            ushort spacing = spacing_togenerate;


            stopwatch.Start();
            TerrainDataFile.GRID_SPACING = spacing;

            TerrainDataFile.GridBlock grid;
            srtm.altresponce altresponce = new srtm.altresponce();

            string ns, ew;
            if (lat_int < 0) ns = "S"; else ns = "N";
            if (lon_int <0 ) ew = "W"; else ew = "E";

            string filename = string.Format("{0}{1:00}{2}{3:000}.DAT", ns, Math.Abs(lat_int), ew, Math.Abs(lon_int));
            string path = Path.Combine(Settings.GetUserDataDirectory(), "TerrainData");
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch
                {
                    MessageBox.Show("Can't create directory: " + path);
                    return;
                }
            }
            string fullpath = Path.Combine(path, filename);

            FileStream datafile = new FileStream(fullpath, FileMode.Create);
            BinaryWriter datafileWriter = new BinaryWriter(datafile);

            int a = TerrainDataFile.east_blocks(new Location(lat_int * 10 * 1000 * 1000, lon_int * 10 * 1000 * 1000));

            Console.WriteLine("East Blocks {0}",a);

            int n = -1;

            while (true)
            {
                n += 1;

                Location locOfBlock = TerrainDataFile.pos_from_file_offset(lat_int, lon_int, n * TerrainDataFile.IO_BLOCK_SIZE);
                if (locOfBlock.lat * 1.0e-7 - lat_int >= 1.0) break;
                grid = new TerrainDataFile.GridBlock((sbyte)lat_int, (short)lon_int, locOfBlock, spacing);
                Console.WriteLine("{0} {1} {2} {3} {4} {5} {6}", a, grid.blocknum(), n * TerrainDataFile.IO_BLOCK_SIZE, grid.GridIdxX, grid.GridIdxY, grid.Lat, grid.Lon);
            }

            Console.WriteLine("Number of blocks in a dat file: {0}", n);

            //We have the max block number in N
            for (int blocknum = 0; blocknum < n; blocknum++)
            {
                string message = string.Format("Making block {0} of {1}", blocknum, n);
                sender.UpdateProgressAndStatus((int)(100.0 * blocknum / n), message);
                if (sender.doWorkArgs.CancelRequested)
                {
                    sender.doWorkArgs.CancelAcknowledged = true;
                    datafileWriter.Close();
                    datafile.Close();
                    //delete the datafile sinc it is incomplete
                    File.Delete(fullpath);
                    return;
                }

                Location loc = TerrainDataFile.pos_from_file_offset(lat_int, lon_int, blocknum * TerrainDataFile.IO_BLOCK_SIZE);
                grid = new TerrainDataFile.GridBlock((sbyte)lat_int, (short)lon_int, loc, spacing);


                int valids = 0;

                for (int gx = 0; gx < TerrainDataFile.TERRAIN_GRID_BLOCK_SIZE_X; gx++)
                {
                    for (int gy = 0; gy < TerrainDataFile.TERRAIN_GRID_BLOCK_SIZE_Y; gy++)
                    {
                        Location pointLoc = grid.blockTopLeft.add_offset_meters(gx * TerrainDataFile.GRID_SPACING, gy * TerrainDataFile.GRID_SPACING);
                        //// Check elevation
                        double lat = pointLoc.lat * 1.0e-7;
                        double lon = pointLoc.lng * 1.0e-7;
                        altresponce = srtm.getAltitude(lat, lon, 20); //get at max zoom

                        if (altresponce.altsource == "GeoTiff") Console.WriteLine("GeoTiff {0} {1} {2}", lat, lon, altresponce.alt);
                        if (altresponce.currenttype == srtm.tiletype.valid && altresponce.alt != 0)
                        {
                            valids++;
                            grid.SetHeight(gx, gy, (short)Math.Round(altresponce.alt));
                        }
                        else
                        {
                            grid.SetHeight(gx, gy, 0);
                        }

                    }
                }

                //Check block validity and fill bitmap
                grid.Bitmap = 0; // Clear bitmap

                for (int x = 0; x < TerrainDataFile.TERRAIN_GRID_BLOCK_MUL_X; x++)
                {
                    for (int y = 0; y < TerrainDataFile.TERRAIN_GRID_BLOCK_MUL_Y; y++)
                    {
                        if (grid.isvalid(x, y))
                        {
                            grid.setBit((byte)(y + TerrainDataFile.TERRAIN_GRID_BLOCK_MUL_Y * x));
                        }

                    }
                }

                byte[] bytes = grid.getPackedBytes();
                datafileWriter.Write(bytes);

            }

            stopwatch.Stop();
            Console.WriteLine("Terrain Data Creator Time elapsed: {0}", stopwatch.Elapsed);
            datafileWriter.Close();
            datafile.Close();

        }

        public override bool Exit()
        {
            return true;
        }


    }
}