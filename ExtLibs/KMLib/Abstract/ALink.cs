using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace KMLib.Abstract
{
    //--note: this abstract class is not part of the official KML class diagram, but it makes sense given that "<Icon> has the same child elements as <Link>"
    public class ALink
    {
        private string m_href;
        public string href {
            get {
                return m_href;
            }
            set {
                m_href = value;
            }
        }

        private RefreshMode m_refreshMode = RefreshMode.onChange;
        public RefreshMode refreshMode {
            get {
                return m_refreshMode;
            }
            set {
                m_refreshMode = value;
                refreshModeSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool refreshModeSpecified = false;

        private float m_refreshInterval = 4;
        public float refreshInterval {
            get {
                return m_refreshInterval;
            }
            set {
                m_refreshInterval = value;
                refreshIntervalSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool refreshIntervalSpecified = false;

        private ViewRefreshMode m_viewRefreshMode = ViewRefreshMode.never;
        public ViewRefreshMode viewRefreshMode {
            get {
                return m_viewRefreshMode;
            }
            set {
                m_viewRefreshMode = value;
                viewRefreshModeSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool viewRefreshModeSpecified = false;

        private float m_viewRefreshTime = 4;
        public float viewRefreshTime {
            get {
                return m_viewRefreshTime;
            }
            set {
                m_viewRefreshTime = value;
                viewRefreshTimeSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool viewRefreshTimeSpecified = false;

        private float m_viewBoundScale = 1;
        public float viewBoundScale {
            get {
                return m_viewBoundScale;
            }
            set {
                m_viewBoundScale = value;
                viewBoundScaleSpecified = true;
            }
        }
        [XmlIgnore()]
        public bool viewBoundScaleSpecified = false;

        private string m_viewFormat;
        public string viewFormat {
            get {
                return m_viewFormat;
            }
            set {
                m_viewFormat = value;
            }
        }

        private string m_httpQuery;
        public string httpQuery {
            get {
                return m_httpQuery;
            }
            set {
                m_httpQuery = value;
            }
        }
    }
}
