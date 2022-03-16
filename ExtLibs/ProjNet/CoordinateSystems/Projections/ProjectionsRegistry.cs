using System;
using System.Collections.Generic;
using System.Reflection;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;

namespace ProjNet.CoordinateSystems.Projections
{
    /// <summary>
    /// Registry class for all known <see cref="MapProjection"/>s.
    /// </summary>
    public class ProjectionsRegistry
    {
        private static readonly Dictionary<string, Type> TypeRegistry = new Dictionary<string, Type>();
        private static readonly Dictionary<string, Type> ConstructorRegistry = new Dictionary<string, Type>();

        private static readonly object RegistryLock = new object();

        /// <summary>
        /// Static constructor
        /// </summary>
        static ProjectionsRegistry()
        {
            Register("mercator", typeof(Mercator));
            Register("mercator_1sp", typeof (Mercator));
            Register("mercator_2sp", typeof (Mercator));
            Register("pseudo-mercator", typeof(PseudoMercator));
            Register("popular_visualisation pseudo-mercator", typeof(PseudoMercator));
            Register("google_mercator", typeof(PseudoMercator));
			
            Register("transverse_mercator", typeof(TransverseMercator));

            Register("albers", typeof(AlbersProjection));
			Register("albers_conic_equal_area", typeof(AlbersProjection));

			Register("krovak", typeof(KrovakProjection));

			Register("polyconic", typeof(PolyconicProjection));
			
            Register("lambert_conformal_conic", typeof(LambertConformalConic2SP));
			Register("lambert_conformal_conic_2sp", typeof(LambertConformalConic2SP));
			Register("lambert_conic_conformal_(2sp)", typeof(LambertConformalConic2SP));

            Register("cassini_soldner", typeof(CassiniSoldnerProjection));
            Register("hotine_oblique_mercator", typeof(HotineObliqueMercatorProjection));
            Register("oblique_mercator", typeof(ObliqueMercatorProjection));
            Register("oblique_stereographic", typeof(ObliqueStereographicProjection));
        }

        /// <summary>
        /// Method to register a new Map
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        public static void Register(string name, Type type)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            if (type == null)
                throw new ArgumentNullException("type");

            if (!typeof(IMathTransform).IsAssignableFrom(type))
                throw new ArgumentException("The provided type does not implement 'GeoAPI.CoordinateSystems.Transformations.IMathTransform'!", "type");

            var ci = CheckConstructor(type);
            if (ci == null)
                throw new ArgumentException("The provided type is lacking a suitable constructor", "type");

            var key = name.ToLowerInvariant().Replace(' ', '_');
            lock (RegistryLock)
            {
                if (TypeRegistry.ContainsKey(key))
                {
                    var rt = TypeRegistry[key];
                    if (ReferenceEquals(type, rt))
                        return;
                    throw new ArgumentException("A different projection type has been registered with this name", "name");
                }

                TypeRegistry.Add(key, type);
                ConstructorRegistry.Add(key, ci);
            }
        }

        private static Type CheckConstructor(Type type)
        {
            // find a constructor that accepts exactly one parameter that's an
            // instance of List<ProjectionParameter>, and then return the exact
            // parameter type so that we can create instances of this type with
            // minimal copying in the future, when possible.
            foreach (ConstructorInfo c in type.GetConstructors())
            {
                System.Reflection.ParameterInfo[] parameters = c.GetParameters();
                if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(List<ProjectionParameter>)))
                {
                    return parameters[0].ParameterType;
                }
            }

            return null;
        }

        internal static IMathTransform CreateProjection(string className, IEnumerable<ProjectionParameter> parameters)
        {
            var key = className.ToLowerInvariant().Replace(' ', '_');

            Type projectionType;
            Type ci;

            lock (RegistryLock)
            {
                if (!TypeRegistry.TryGetValue(key, out projectionType))
                    throw new NotSupportedException(String.Format("Projection {0} is not supported.", className));
                ci = ConstructorRegistry[key];
            }

            if (!ci.IsAssignableFrom(parameters.GetType()))
            {
                parameters = new List<ProjectionParameter>(parameters);
            }

            return (IMathTransform) Activator.CreateInstance(projectionType, parameters);
        }
    }
}