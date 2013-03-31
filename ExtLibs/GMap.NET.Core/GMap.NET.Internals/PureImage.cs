
namespace GMap.NET
{
   using System;
   using System.IO;

   /// <summary>
   /// image abstraction proxy
   /// </summary>
   public abstract class PureImageProxy
   {
      abstract public PureImage FromStream(Stream stream);
      abstract public bool Save(Stream stream, PureImage image);
   }

   /// <summary>
   /// image abstraction
   /// </summary>
   public abstract class PureImage : IDisposable
   {
      public MemoryStream Data;

      #region IDisposable Members

      abstract public void Dispose();

      #endregion
   }
}
