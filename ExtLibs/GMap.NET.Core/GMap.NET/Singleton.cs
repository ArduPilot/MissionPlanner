
namespace GMap.NET
{
   using System;

   /// <summary>
   /// generic for singletons
   /// </summary>
   /// <typeparam name="T"></typeparam>
   public class Singleton<T> where T : new()
   {
      // ctor
      protected Singleton()
      {
         if(Instance != null)
         {
            throw (new Exception("You have tried to create a new singleton class where you should have instanced it. Replace your \"new class()\" with \"class.Instance\""));
         }
      }

      public static T Instance
      {
         get
         {
            return SingletonCreator.instance;
         }
      }

      class SingletonCreator
      {
         static SingletonCreator()
         {

         }
         internal static readonly T instance = new T();
      }
   }
}
