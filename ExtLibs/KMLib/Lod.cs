using System;
using System.Collections.Generic;
using System.Text;

namespace KMLib
{
    public class Lod
    {
        private int m_minLodPixels = -1;
        public int minLodPixels {
            get {
                return m_minLodPixels;
            }
            set {
                m_minLodPixels = value;
            }
        }

        private int m_maxLodPixels = -1;
        public int maxLodPixels {
            get {
                return m_maxLodPixels;
            }
            set {
                m_maxLodPixels = value;
            }
        }
    }

    /*
     <Lod>
          <minLodPixels>128</minLodPixels><maxLodPixels>-1</maxLodPixels>
        </Lod>
     */
}
