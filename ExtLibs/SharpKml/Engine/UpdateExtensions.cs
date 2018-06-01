using System;
using SharpKml.Dom;

namespace SharpKml.Engine
{
    /// <summary>
    /// Provides extension methods for <see cref="Update"/> objects.
    /// </summary>
    public static class UpdateExtensions
    {
        /// <summary>
        /// Provides in-place (destructive) processing of the <see cref="Update"/>.
        /// </summary>
        /// <param name="update">The update instance.</param>
        /// <param name="file">
        /// A KmlFile containing the <c>Update</c> and the update targets.
        /// </param>
        /// <exception cref="ArgumentNullException">file is null.</exception>
        public static void Process(this Update update, KmlFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            foreach (var child in update.Updates)
            {
                ChangeCollection change = child as ChangeCollection;
                if (change != null)
                {
                    ProcessChange(change, file);
                    continue;
                }

                CreateCollection create = child as CreateCollection;
                if (create != null)
                {
                    ProcessCreate(create, file);
                    continue;
                }

                DeleteCollection delete = child as DeleteCollection;
                if (delete != null)
                {
                    ProcessDelete(delete, file);
                }
            }
        }

        private static void ProcessChange(ChangeCollection change, KmlFile file)
        {
            foreach (var source in change)
            {
                if (source.TargetId != null)
                {
                    KmlObject target = file.FindObject(source.TargetId);
                    if (target != null)
                    {
                        target.Merge(source);
                        target.TargetId = null; // Merge copied the TargetId from the source, but target shouldn't have it set
                    }
                }
            }
        }

        private static void ProcessCreate(CreateCollection create, KmlFile file)
        {
            foreach (var source in create)
            {
                if (source.TargetId != null)
                {
                    Container target = file.FindObject(source.TargetId) as Container;
                    if (target != null) // Make sure it was found and that the target was a Container
                    {
                        foreach (var feature in source.Features)
                        {
                            var clone = feature.Clone(); // We never give the original source.
                            target.AddFeature(clone);
                            file.AddFeature(clone);
                        }
                    }
                }
            }
        }

        private static void ProcessDelete(DeleteCollection delete, KmlFile file)
        {
            foreach (var source in delete)
            {
                if (source.TargetId != null)
                {
                    Feature feature = file.FindObject(source.TargetId) as Feature;
                    if (feature != null)
                    {
                        // Remove the Feature from the parent, which is either
                        // a Container or Kml
                        Container container = feature.Parent as Container;
                        if (container != null)
                        {
                            container.RemoveFeature(source.TargetId);
                        }
                        else
                        {
                            Kml kml = feature.Parent as Kml;
                            if (kml != null)
                            {
                                kml.Feature = null;
                            }
                        }

                        // Also remove it from the file
                        file.RemoveFeature(feature);
                    }
                }
            }
        }
    }
}
