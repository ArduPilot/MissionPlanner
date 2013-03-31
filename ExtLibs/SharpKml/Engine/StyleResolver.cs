using System;
using System.Collections.Generic;
using System.Linq;
using SharpKml.Dom;

namespace SharpKml.Engine
{
    /// <summary>
    /// Merges and resolves <see cref="Style"/> links.
    /// </summary>
    public sealed class StyleResolver
    {
        // This is the maximum number of styleUrls followed in resolving a style
        // selector.  Note: the KML standard specifies no such limit.  This is used
        // primarily to inhibit infinite loops on styleUrls that are self referencing.
        private const int MaximumNestingDepth = 5;

        private IDictionary<string, StyleSelector> _styleMap;
        private Style _style = new Style();
        private StyleState _state;

        private int _nestedDepth;
        private int _styleId;
        private bool _resolveExternal;

        private StyleResolver(IDictionary<string, StyleSelector> map)
        {
            _styleMap = map;
        }

        /// <summary>Resolves all the styles in the specified Feature.</summary>
        /// <param name="feature">The Feature to search for Styles.</param>
        /// <param name="file">The KmlFile the feature belongs to.</param>
        /// <param name="state">The StyleState of the styles to look for.</param>
        /// <param name="resolve">
        /// Specifies whether to resolve external styles by opening the linked
        /// file and loading the style from it.
        /// </param>
        /// <returns>A new Style that has been resolved.</returns>
        /// <exception cref="ArgumentNullException">feature/file is null.</exception>
        /// <remarks>
        /// If resolve is set to true, the method will block while loading the
        /// linked file, however, any errors opening the file are ignored.
        /// </remarks>
        public static Style CreateResolvedStyle(Feature feature, KmlFile file, StyleState state, bool resolve)
        {
            if (feature == null)
            {
                throw new ArgumentNullException("feature");
            }
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var instance = new StyleResolver(file.StyleMap);
            instance._resolveExternal = resolve;
            instance._state = state;
            instance.Merge(feature.StyleUrl, feature.StyleSelector);
            return instance._style;
        }

        /// <summary>
        /// Inlines the shared Style of the features in the specified element.
        /// </summary>
        /// <typeparam name="T">
        /// A class deriving from <see cref="Element"/>.
        /// </typeparam>
        /// <param name="element">The element instance.</param>
        /// <returns>A new element with the shared styles inlined.</returns>
        public static T InlineStyles<T>(T element) where T : Element
        {
            var instance = new StyleResolver(new Dictionary<string, StyleSelector>());

            // Don't modify the original but create a copy instead
            T clone = element.Clone();
            foreach (var e in clone.Flatten())
            {
                instance.InlineElement(e);
            }
            return clone;
        }

        /// <summary>
        /// Changes inlined styles to shared styles in the closest Document parent.
        /// </summary>
        /// <typeparam name="T">
        /// A class deriving from <see cref="Element"/>.
        /// </typeparam>
        /// <param name="element">The element instance.</param>
        /// <returns>
        /// A new element with the inlined styles changed to shared styles.
        /// </returns>
        public static T SplitStyles<T>(T element) where T : Element
        {
            var instance = new StyleResolver(new Dictionary<string, StyleSelector>());

            // Can't modify the Children collection while we're iterating so
            // create a temporary list of styles to add
            var sharedStyles = new List<Tuple<Document, StyleSelector>>();

            T clone = element.Clone();
            foreach (var e in clone.Flatten())
            {
                var tuple = instance.Split(e);
                if (tuple != null)
                {
                    sharedStyles.Add(tuple);
                }
            }

            // Finished iterating the flattened hierarchy (which incudes
            // Children) so we can now add the shared styles
            foreach (var style in sharedStyles)
            {
                style.Item1.AddStyle(style.Item2);
            }
            return clone;
        }

        private Pair CreatePair(StyleState state, Uri url)
        {
            this.Reset();

            _state = state;
            this.Merge(url);

            var pair = new Pair();
            pair.State = state;
            pair.Selector = _style;
            return pair;
        }

        private StyleSelector CreateStyleMap(Uri url)
        {
            // This is the same order as the C++ version - Normal then Highlight
            var map = new StyleMapCollection();
            map.Add(this.CreatePair(StyleState.Normal, url));
            map.Add(this.CreatePair(StyleState.Highlight, url));
            return map;
        }

        private string CreateUniqueId()
        {
            while (true) // Keep trying until we find a unique identifier
            {
                string id = "_" + _styleId++;
                if (!_styleMap.ContainsKey(id))
                {
                    return id;
                }
            }
        }

        private void InlineElement(Element element)
        {
            if (element.IsChildOf<Update>())
            {
                return;
            }

            var feature = element as Feature;
            if (feature != null)
            {
                // Check if it's a Document, which inherits from Feature, as
                // Documents contain shared styles
                var document = element as Document;
                if (document != null)
                {
                    // Create a copy of the styles so we can iterate the copy
                    // and remove them from the Document.
                    var styles = document.Styles.ToList();
                    foreach (var style in styles)
                    {
                        if (style.Id != null)
                        {
                            _styleMap[style.Id] = style;
                            style.Id = null; // The C++ version clears the id, so we will too...
                            document.RemoveChild(style);
                        }
                    }
                }
                else
                {
                    // If it's a local reference and we've captured the shared style
                    // give a copy of that and clear the StyleUrl
                    if ((feature.StyleUrl != null) && !feature.StyleUrl.IsAbsoluteUri)
                    {
                        string id = feature.StyleUrl.GetFragment();
                        if (_styleMap.ContainsKey(id))
                        {
                            feature.StyleSelector = this.CreateStyleMap(feature.StyleUrl);
                            feature.StyleUrl = null;
                        }
                    }
                }
            }
        }

        private void Merge(Uri url, StyleSelector selector)
        {
            // If there's a url to a shared style merge that in first.
            this.Merge(url);

            // If there's an inline style that takes priority so merge that over.
            this.Merge(selector);
        }

        private void Merge(StyleSelector selector)
        {
            var style = selector as Style;
            if (style != null)
            {
                _style.Merge(style);
            }
            else
            {
                var styleMap = selector as StyleMapCollection;
                if (styleMap != null)
                {
                    foreach (var pair in styleMap)
                    {
                        if (pair.State == _state)
                        {
                            this.Merge(pair.StyleUrl, pair.Selector);
                        }
                    }
                }
            }
        }

        private void Merge(Uri url)
        {
            if ((_nestedDepth++ >= MaximumNestingDepth) || (url == null))
            {
                return; // Silently fail
            }

            string id = url.GetFragment();
            if (!string.IsNullOrEmpty(id))
            {
                // If there's no path this is a StyleSelector within this file.
                string path = url.GetPath();
                if (string.IsNullOrEmpty(path))
                {
                    StyleSelector style;
                    if (_styleMap.TryGetValue(id, out style))
                    {
                        this.Merge(style);
                    }
                }
                else if (_resolveExternal)
                {
                    KmlFile file = FileHandler.ReadFile(path);
                    if (file != null)
                    {
                        StyleSelector style;
                        if (file.StyleMap.TryGetValue(id, out style))
                        {
                            this.Merge(style);
                        }
                    }
                }
            }
        }

        private void Reset()
        {
            _nestedDepth = 0;
            _style = new Style();
        }

        private Tuple<Document, StyleSelector> Split(Element element)
        {
            if (element.IsChildOf<Update>())
            {
                return null;
            }

            var style = element as StyleSelector;
            if (style != null)
            {
                // Add the style to the map so we generate an unique id
                if (style.Id != null)
                {
                    _styleMap[style.Id] = style;
                }

                // Find the Document to put the Style in, making sure it doesn't
                // already have one as a Parent.
                var document = element.GetParent<Document>();
                if ((document != null) && (element.Parent != document))
                {
                    // Is the style in a Feature that doesn't have a StyleUrl?
                    var feature = element.Parent as Feature;
                    if ((feature != null) && (feature.StyleUrl == null))
                    {
                        // Create a copy of the style, using a new id
                        StyleSelector shared = (StyleSelector)Activator.CreateInstance(style.GetType());
                        shared.Merge(style);
                        shared.Id = this.CreateUniqueId();

                        // Tell the feature to use the new shared style and
                        // remove the old style.
                        _styleMap.Add(shared.Id, shared);
                        feature.StyleUrl = new Uri("#" + shared.Id, UriKind.Relative);
                        feature.StyleSelector = null;

                        // This will be added to the Document later when we've
                        // finished iterating through the Children
                        return Tuple.Create(document, shared);
                    }
                }
            }
            return null; // Nothing to add
        }
    }
}
