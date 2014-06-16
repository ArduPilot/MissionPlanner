
namespace GMap.NET.MapProviders
{
   using System;
   using GMap.NET.Projections;
   using System.Net;

   public abstract class NearMapProviderBase : GMapProvider
   {
      public NearMapProviderBase()
      {
         // credentials doesn't work ;/
         //Credential = new NetworkCredential("greatmaps", "greatmaps");
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

      public new static int GetServerNum(GPoint pos, int max)
      {
         // var hostNum=((opts.nodes!==0)?((tileCoords.x&2)%opts.nodes):0)+opts.nodeStart;
         return (int)(pos.X & 2) % max;
      }

      static readonly string SecureStr = "Vk52edzNRYKbGjF8Ur0WhmQlZs4wgipDETyL1oOMXIAvqtxJBuf7H36acCnS9P";

      public string GetSafeString(GPoint pos)
      {
         #region -- source --
         /*
         TileLayer.prototype.differenceEngine=function(s,a)
         {
             var offset=0,result="",alen=a.length,v,p;
             for(var i=0; i<alen; i++)
             {
                 v=parseInt(a.charAt(i),10);
                 if(!isNaN(v))
                 {
                     offset+=v;
                     p=s.charAt(offset%s.length);
                     result+=p
                 }             
             }
             return result
         };    
       
         TileLayer.prototype.getSafeString=function(x,y,nmd)
         {
              var arg=x.toString()+y.toString()+((3*x)+y).toString();
              if(nmd)
              {
                 arg+=nmd
              }
              return this.differenceEngine(TileLayer._substring,arg)
         };  
        */
         #endregion

         var arg = pos.X.ToString() + pos.Y.ToString() + ((3 * pos.X) + pos.Y).ToString();

         string ret = "&s=";
         int offset = 0;
         for(int i = 0; i < arg.Length; i++)
         {
            offset += int.Parse(arg[i].ToString());
            ret += SecureStr[offset % SecureStr.Length];
         }

         return ret;
      }
   }

   /// <summary>
   /// NearMap provider - http://www.nearmap.com/
   /// </summary>
   public class NearMapProvider : NearMapProviderBase
   {
      public static readonly NearMapProvider Instance;

      NearMapProvider()
      {
         RefererUrl = "http://www.nearmap.com/";
      }

      static NearMapProvider()
      {
         Instance = new NearMapProvider();
      }

      #region GMapProvider Members

      readonly Guid id = new Guid("E33803DF-22CB-4FFA-B8E3-15383ED9969D");
      public override Guid Id
      {
         get
         {
            return id;
         }
      }

      readonly string name = "NearMap";
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
         // http://web1.nearmap.com/maps/hl=en&x=18681&y=10415&z=15&nml=Map_&nmg=1&s=kY8lZssipLIJ7c5
         // http://web1.nearmap.com/kh/v=nm&hl=en&x=20&y=8&z=5&nml=Map_&s=55KUZ

         return string.Format(UrlFormat, GetServerNum(pos, 3), pos.X, pos.Y, zoom, GetSafeString(pos));
      }

      static readonly string UrlFormat = "http://web{0}.nearmap.com/kh/v=nm&hl=en&x={1}&y={2}&z={3}&nml=Map_{4}";
   }
}