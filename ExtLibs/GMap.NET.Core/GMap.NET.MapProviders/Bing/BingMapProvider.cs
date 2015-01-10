﻿
namespace GMap.NET.MapProviders
{
   using System;
   using System.Text;
   using GMap.NET.Projections;
   using System.Diagnostics;
   using System.Net;
   using System.IO;
   using System.Text.RegularExpressions;
   using System.Threading;
   using GMap.NET.Internals;
   using System.Collections.Generic;
   using System.Globalization;
   using System.Xml;

   public abstract class BingMapProviderBase : GMapProvider, RoutingProvider, GeocodingProvider
   {
      public BingMapProviderBase()
      {
         MaxZoom = null;
         RefererUrl = "http://www.bing.com/maps/";
         Copyright = string.Format("©{0} Microsoft Corporation, ©{0} NAVTEQ, ©{0} Image courtesy of NASA", DateTime.Today.Year);
      }

      public string Version = "1173";

      /// <summary>
      /// Bing Maps Customer Identification, more info here
      /// http://msdn.microsoft.com/en-us/library/bb924353.aspx
      /// </summary>
      public string ClientKey = null;

      /// <summary>
      /// Converts tile XY coordinates into a QuadKey at a specified level of detail.
      /// </summary>
      /// <param name="tileX">Tile X coordinate.</param>
      /// <param name="tileY">Tile Y coordinate.</param>
      /// <param name="levelOfDetail">Level of detail, from 1 (lowest detail)
      /// to 23 (highest detail).</param>
      /// <returns>A string containing the QuadKey.</returns>
      internal string TileXYToQuadKey(long tileX, long tileY, int levelOfDetail)
      {
         StringBuilder quadKey = new StringBuilder();
         for(int i = levelOfDetail; i > 0; i--)
         {
            char digit = '0';
            int mask = 1 << (i - 1);
            if((tileX & mask) != 0)
            {
               digit++;
            }
            if((tileY & mask) != 0)
            {
               digit++;
               digit++;
            }
            quadKey.Append(digit);
         }
         return quadKey.ToString();
      }

      /// <summary>
      /// Converts a QuadKey into tile XY coordinates.
      /// </summary>
      /// <param name="quadKey">QuadKey of the tile.</param>
      /// <param name="tileX">Output parameter receiving the tile X coordinate.</param>
      /// <param name="tileY">Output parameter receiving the tile Y coordinate.</param>
      /// <param name="levelOfDetail">Output parameter receiving the level of detail.</param>
      internal void QuadKeyToTileXY(string quadKey, out int tileX, out int tileY, out int levelOfDetail)
      {
          tileX = tileY = 0;
          levelOfDetail = quadKey.Length;
          for (int i = levelOfDetail; i > 0; i--)
          {
              int mask = 1 << (i - 1);
              switch (quadKey[levelOfDetail - i])
              {
                  case '0':
                  break;

                  case '1':
                  tileX |= mask;
                  break;

                  case '2':
                  tileY |= mask;
                  break;

                  case '3':
                  tileX |= mask;
                  tileY |= mask;
                  break;

                  default:
                  throw new ArgumentException("Invalid QuadKey digit sequence.");
              }
          }
      }

      #region GMapProvider Members
      public override Guid Id
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override string Name
      {
         get
         {
            throw new NotImplementedException();
         }
      }

      public override PureProjection Projection
      {
         get
         {
            return MercatorProjection.Instance;
         }
      }

      GMapProvider[] overlays;
      public override GMapProvider[] Overlays
      {
         get
         {
            if(overlays == null)
            {
               overlays = new GMapProvider[] { this };
            }
            return overlays;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         throw new NotImplementedException();
      }
      #endregion

      public bool TryCorrectVersion = true;
      public bool TryGetDefaultKey = true;
      static bool init = false;

      public override void OnInitialized()
      {
         if(!init)
         {
            try
            {
               if(TryCorrectVersion)
               {
                  #region -- get the version --
                  string url = @"http://www.bing.com/maps";
                  string html = GMaps.Instance.UseUrlCache ? Cache.Instance.GetContent(url, CacheType.UrlCache, TimeSpan.FromHours(8)) : string.Empty;

                  if(string.IsNullOrEmpty(html))
                  {
                     html = GetContentUsingHttp(url);
                     if(!string.IsNullOrEmpty(html))
                     {
                        if(GMaps.Instance.UseUrlCache)
                        {
                           Cache.Instance.SaveContent(url, CacheType.UrlCache, html);
                        }
                     }
                  }

                  if(!string.IsNullOrEmpty(html))
                  {
                     #region -- match versions --                      

                     Regex reg = new Regex("tilegeneration:(\\d*)", RegexOptions.IgnoreCase);
                     Match mat = reg.Match(html);
                     if(mat.Success)
                     {
                        GroupCollection gc = mat.Groups;
                        int count = gc.Count;
                        if(count == 2)
                        {
                           string ver = gc[1].Value;
                           string old = GMapProviders.BingMap.Version;
                           if(ver != old)
                           {
                              GMapProviders.BingMap.Version = ver;
                              GMapProviders.BingSatelliteMap.Version = ver;
                              GMapProviders.BingHybridMap.Version = ver;
#if DEBUG
                              Debug.WriteLine("GMapProviders.BingMap.Version: " + ver + ", old: " + old + ", consider updating source");
                              if(Debugger.IsAttached)
                              {
                                 Thread.Sleep(5555);
                              }
#endif
                           }
                           else
                           {
                              Debug.WriteLine("GMapProviders.BingMap.Version: " + ver + ", OK");
                           }
                        }
                     }
                     #endregion
                  }
                  #endregion
               }

               init = true; // try it only once 

               #region -- try get default key --
               if(TryGetDefaultKey && string.IsNullOrEmpty(ClientKey))
               {
                  string keyUrl = "http://dev.virtualearth.net/webservices/v1/LoggingService/LoggingService.svc/Log?entry=0&fmt=1&type=3&group=MapControl&name=AJAX&mkt=en-us&auth=Akw4XWHH0ngzzB_4DmHOv_XByRBtX5qwLAS9RgRYDamxvLeIxRfSzmuvWFB9RF7d&jsonp=microsoftMapsNetworkCallback";

                  // Bing Maps WPF Control
                  // http://dev.virtualearth.net/webservices/v1/LoggingService/LoggingService.svc/Log?entry=0&auth=Akw4XWHH0ngzzB_4DmHOv_XByRBtX5qwLAS9RgRYDamxvLeIxRfSzmuvWFB9RF7d&fmt=1&type=3&group=MapControl&name=WPF&version=1.0.0.0&session=00000000-0000-0000-0000-000000000000&mkt=en-US

                  string keyResponse = GMaps.Instance.UseUrlCache ? Cache.Instance.GetContent("BingLoggingServiceV1", CacheType.UrlCache, TimeSpan.FromHours(8)) : string.Empty;

                  if(string.IsNullOrEmpty(keyResponse))
                  {
                     keyResponse = GetContentUsingHttp(keyUrl);
                     if(!string.IsNullOrEmpty(keyResponse) && keyResponse.Contains("ValidCredentials"))
                     {
                        if(GMaps.Instance.UseUrlCache)
                        {
                           Cache.Instance.SaveContent("BingLoggingServiceV1", CacheType.UrlCache, keyResponse);
                        }
                     }
                  }

                  if(!string.IsNullOrEmpty(keyResponse) && keyResponse.Contains("sessionId") && keyResponse.Contains("ValidCredentials"))
                  {
                     // microsoftMapsNetworkCallback({"sessionId" : "xxx", "authenticationResultCode" : "ValidCredentials"})

                     ClientKey = keyResponse.Split(',')[0].Split(':')[1].Replace("\"", string.Empty).Replace(" ", string.Empty);
                     Debug.WriteLine("GMapProviders.BingMap.ClientKey: " + ClientKey);
                  }
               }
               #endregion
            }
            catch(Exception ex)
            {
               Debug.WriteLine("TryCorrectBingVersions failed: " + ex.ToString());
            }
         }
      }

      protected override bool CheckTileImageHttpResponse(WebResponse response)
      {
         var pass = base.CheckTileImageHttpResponse(response);
         if(pass)
         {
            var tileInfo = response.Headers.Get("X-VE-Tile-Info");
            if(tileInfo != null)
            {
               return !tileInfo.Equals("no-tile");
            }
         }
         return pass;
      }

      #region RoutingProvider

      public MapRoute GetRoute(PointLatLng start, PointLatLng end, bool avoidHighways, bool walkingMode, int Zoom)
      {
         string tooltip;
         int numLevels;
         int zoomFactor;
         MapRoute ret = null;
         List<PointLatLng> points = GetRoutePoints(MakeRouteUrl(start, end, LanguageStr, avoidHighways, walkingMode), Zoom, out tooltip, out numLevels, out zoomFactor);
         if(points != null)
         {
            ret = new MapRoute(points, tooltip);
         }
         return ret;
      }

      public MapRoute GetRoute(string start, string end, bool avoidHighways, bool walkingMode, int Zoom)
      {
         throw new NotImplementedException("check GetRoute(PointLatLng start...");
      }

      string MakeRouteUrl(PointLatLng start, PointLatLng end, string language, bool avoidHighways, bool walkingMode)
      {
         string addition = avoidHighways ? "&avoid=highways" : string.Empty;
         string mode = walkingMode ? "Walking" : "Driving";

         return string.Format(CultureInfo.InvariantCulture, RouteUrlFormatPointLatLng, mode, start.Lat, start.Lng, end.Lat, end.Lng, addition, ClientKey);
      }

      List<PointLatLng> GetRoutePoints(string url, int zoom, out string tooltipHtml, out int numLevel, out int zoomFactor)
      {
         List<PointLatLng> points = null;
         tooltipHtml = string.Empty;
         numLevel = -1;
         zoomFactor = -1;
         try
         {
            string route = GMaps.Instance.UseRouteCache ? Cache.Instance.GetContent(url, CacheType.RouteCache) : string.Empty;

            if(string.IsNullOrEmpty(route))
            {
               route = GetContentUsingHttp(url);

               if(!string.IsNullOrEmpty(route))
               {
                  if(GMaps.Instance.UseRouteCache)
                  {
                     Cache.Instance.SaveContent(url, CacheType.RouteCache, route);
                  }
               }
            }

            // parse values
            if(!string.IsNullOrEmpty(route))
            {
               #region -- title --
               int tooltipEnd = 0;
               {
                  int x = route.IndexOf("<RoutePath><Line>") + 17;
                  if(x >= 17)
                  {
                     tooltipEnd = route.IndexOf("</Line></RoutePath>", x + 1);
                     if(tooltipEnd > 0)
                     {
                        int l = tooltipEnd - x;
                        if(l > 0)
                        {
                           //tooltipHtml = route.Substring(x, l).Replace(@"\x26#160;", " ");
                           tooltipHtml = route.Substring(x, l);
                        }
                     }
                  }
               }
               #endregion

               #region -- points --
               XmlDocument doc = new XmlDocument();
               doc.LoadXml(route);
               XmlNode xn = doc["Response"];
               string statuscode = xn["StatusCode"].InnerText;
               switch(statuscode)
               {
                  case "200":
                  {
                     xn = xn["ResourceSets"]["ResourceSet"]["Resources"]["Route"]["RoutePath"]["Line"];
                     XmlNodeList xnl = xn.ChildNodes;
                     if(xnl.Count > 0)
                     {
                        points = new List<PointLatLng>();
                        foreach(XmlNode xno in xnl)
                        {
                           XmlNode latitude = xno["Latitude"];
                           XmlNode longitude = xno["Longitude"];
                           points.Add(new PointLatLng(double.Parse(latitude.InnerText, CultureInfo.InvariantCulture),
                                                      double.Parse(longitude.InnerText, CultureInfo.InvariantCulture)));
                        }
                     }
                     break;
                  }
                  // no status implementation on routes yet although when introduced these are the codes. Exception will be catched.
                  case "400":
                  throw new Exception("Bad Request, The request contained an error.");
                  case "401":
                  throw new Exception("Unauthorized, Access was denied. You may have entered your credentials incorrectly, or you might not have access to the requested resource or operation.");
                  case "403":
                  throw new Exception("Forbidden, The request is for something forbidden. Authorization will not help.");
                  case "404":
                  throw new Exception("Not Found, The requested resource was not found.");
                  case "500":
                  throw new Exception("Internal Server Error, Your request could not be completed because there was a problem with the service.");
                  case "501":
                  throw new Exception("Service Unavailable, There's a problem with the service right now. Please try again later.");
                  default:
                  points = null;
                  break; // unknown, for possible future error codes
               }
               #endregion
            }
         }
         catch(Exception ex)
         {
            points = null;
            Debug.WriteLine("GetRoutePoints: " + ex);
         }
         return points;
      }

      // example : http://dev.virtualearth.net/REST/V1/Routes/Driving?o=xml&wp.0=44.979035,-93.26493&wp.1=44.943828508257866,-93.09332862496376&optmz=distance&rpo=Points&key=[PROVIDEYOUROWNKEY!!]
      static readonly string RouteUrlFormatPointLatLng = "http://dev.virtualearth.net/REST/V1/Routes/{0}?o=xml&wp.0={1},{2}&wp.1={3},{4}{5}&optmz=distance&rpo=Points&key={6}";

      #endregion RoutingProvider

      #region GeocodingProvider

      public GeoCoderStatusCode GetPoints(string keywords, out List<PointLatLng> pointList)
      {
         return GetLatLngFromGeocoderUrl(MakeGeocoderUrl("q=" + keywords), out pointList);
      }

      public PointLatLng? GetPoint(string keywords, out GeoCoderStatusCode status)
      {
         List<PointLatLng> pointList;
         status = GetPoints(keywords, out pointList);
         return pointList != null && pointList.Count > 0 ? pointList[0] : (PointLatLng?)null;
      }

      public GeoCoderStatusCode GetPoints(Placemark placemark, out List<PointLatLng> pointList)
      {
         return GetLatLngFromGeocoderUrl(MakeGeocoderDetailedUrl(placemark), out pointList);
      }

      public PointLatLng? GetPoint(Placemark placemark, out GeoCoderStatusCode status)
      {
         List<PointLatLng> pointList;
         status = GetLatLngFromGeocoderUrl(MakeGeocoderDetailedUrl(placemark), out pointList);
         return pointList != null && pointList.Count > 0 ? pointList[0] : (PointLatLng?)null;
      }

      string MakeGeocoderDetailedUrl(Placemark placemark)
      {
         string parameters = string.Empty;

         if(!AddFieldIfNotEmpty(ref parameters, "countryRegion", placemark.CountryNameCode))
            AddFieldIfNotEmpty(ref parameters, "countryRegion", placemark.CountryName);

         AddFieldIfNotEmpty(ref parameters, "adminDistrict", placemark.DistrictName);
         AddFieldIfNotEmpty(ref parameters, "locality", placemark.LocalityName);
         AddFieldIfNotEmpty(ref parameters, "postalCode", placemark.PostalCodeNumber);

         if(!string.IsNullOrEmpty(placemark.HouseNo))
            AddFieldIfNotEmpty(ref parameters, "addressLine", placemark.ThoroughfareName + " " + placemark.HouseNo);
         else
            AddFieldIfNotEmpty(ref parameters, "addressLine", placemark.ThoroughfareName);

         return MakeGeocoderUrl(parameters);
      }

      bool AddFieldIfNotEmpty(ref string Input, string FieldName, string Value)
      {
         if(!string.IsNullOrEmpty(Value))
         {
            if(string.IsNullOrEmpty(Input))
               Input = string.Empty;
            else
               Input = Input + "&";

            Input = Input + FieldName + "=" + Value;

            return true;
         }
         return false;
      }

      public GeoCoderStatusCode GetPlacemarks(PointLatLng location, out List<Placemark> placemarkList)
      {
         // http://msdn.microsoft.com/en-us/library/ff701713.aspx
         throw new NotImplementedException();
      }

      public Placemark? GetPlacemark(PointLatLng location, out GeoCoderStatusCode status)
      {
         // http://msdn.microsoft.com/en-us/library/ff701713.aspx
         throw new NotImplementedException();
      }

      string MakeGeocoderUrl(string keywords)
      {
         return string.Format(CultureInfo.InvariantCulture, GeocoderUrlFormat, keywords, ClientKey);
      }

      GeoCoderStatusCode GetLatLngFromGeocoderUrl(string url, out List<PointLatLng> pointList)
      {
         var status = GeoCoderStatusCode.Unknow;
         pointList = null;

         try
         {
            string geo = GMaps.Instance.UseGeocoderCache ? Cache.Instance.GetContent(url, CacheType.GeocoderCache) : string.Empty;

            bool cache = false;

            if(string.IsNullOrEmpty(geo))
            {
               geo = GetContentUsingHttp(url);

               if(!string.IsNullOrEmpty(geo))
               {
                  cache = true;
               }
            }

            status = GeoCoderStatusCode.Unknow;
            if(!string.IsNullOrEmpty(geo))
            {
               if(geo.StartsWith("<?xml") && geo.Contains("<Response"))
               {
                  XmlDocument doc = new XmlDocument();
                  doc.LoadXml(geo);
                  XmlNode xn = doc["Response"];
                  string statuscode = xn["StatusCode"].InnerText;
                  switch(statuscode)
                  {
                     case "200":
                     {
                        pointList = new List<PointLatLng>();
                        xn = xn["ResourceSets"]["ResourceSet"]["Resources"];
                        XmlNodeList xnl = xn.ChildNodes;
                        foreach(XmlNode xno in xnl)
                        {
                           XmlNode latitude = xno["Point"]["Latitude"];
                           XmlNode longitude = xno["Point"]["Longitude"];
                           pointList.Add(new PointLatLng(Double.Parse(latitude.InnerText, CultureInfo.InvariantCulture),
                                                         Double.Parse(longitude.InnerText, CultureInfo.InvariantCulture)));
                        }

                        if(pointList.Count > 0)
                        {
                           status = GeoCoderStatusCode.G_GEO_SUCCESS;
                           if(cache && GMaps.Instance.UseGeocoderCache)
                           {
                              Cache.Instance.SaveContent(url, CacheType.GeocoderCache, geo);
                           }
                           break;
                        }

                        status = GeoCoderStatusCode.G_GEO_UNKNOWN_ADDRESS;
                        break;
                     }

                     case "400":
                     status = GeoCoderStatusCode.G_GEO_BAD_REQUEST;
                     break; // bad request, The request contained an error.
                     case "401":
                     status = GeoCoderStatusCode.G_GEO_BAD_KEY;
                     break; // Unauthorized, Access was denied. You may have entered your credentials incorrectly, or you might not have access to the requested resource or operation.
                     case "403":
                     status = GeoCoderStatusCode.G_GEO_BAD_REQUEST;
                     break; // Forbidden, The request is for something forbidden. Authorization will not help.
                     case "404":
                     status = GeoCoderStatusCode.G_GEO_UNKNOWN_ADDRESS;
                     break; // Not Found, The requested resource was not found. 
                     case "500":
                     status = GeoCoderStatusCode.G_GEO_SERVER_ERROR;
                     break; // Internal Server Error, Your request could not be completed because there was a problem with the service.
                     case "501":
                     status = GeoCoderStatusCode.Unknow;
                     break; // Service Unavailable, There's a problem with the service right now. Please try again later.
                     default:
                     status = GeoCoderStatusCode.Unknow;
                     break; // unknown, for possible future error codes
                  }
               }
            }
         }
         catch(Exception ex)
         {
            status = GeoCoderStatusCode.ExceptionInCode;
            Debug.WriteLine("GetLatLngFromGeocoderUrl: " + ex);
         }

         return status;
      }

      // http://dev.virtualearth.net/REST/v1/Locations/1%20Microsoft%20Way%20Redmond%20WA%2098052?o=xml&key=BingMapsKey
      static readonly string GeocoderUrlFormat = "http://dev.virtualearth.net/REST/v1/Locations?{0}&o=xml&key={1}";

      #endregion GeocodingProvider
   }

   /// <summary>
   /// BingMapProvider provider
   /// </summary>
   public class BingMapProvider : BingMapProviderBase
   {
      public static readonly BingMapProvider Instance;

      BingMapProvider()
      {
      }

      static BingMapProvider()
      {
         Instance = new BingMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("D0CEB371-F10A-4E12-A2C1-DF617D6674A8");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = Resources.Strings.BingMap;
      public override string Name
      {
         get
         {
            return name;
         }
      }

      public override PureImage GetTileImage(GPoint pos, int zoom)
      {
         string url = MakeTileImageUrl(pos, zoom, LanguageStr);

         return GetTileImageUsingHttp(url);
      }

      #endregion

      string MakeTileImageUrl(GPoint pos, int zoom, string language)
      {
         string key = TileXYToQuadKey(pos.X, pos.Y, zoom);
         return string.Format(UrlFormat, GetServerNum(pos, 4), key, Version, language, (!string.IsNullOrEmpty(ClientKey) ? "&key=" + ClientKey : string.Empty));
      }

      // http://ecn.t0.tiles.virtualearth.net/tiles/r120030?g=875&mkt=en-us&lbl=l1&stl=h&shading=hill&n=z

      static readonly string UrlFormat = "http://ecn.t{0}.tiles.virtualearth.net/tiles/r{1}?g={2}&mkt={3}&lbl=l1&stl=h&shading=hill&n=z{4}";
   }
}
