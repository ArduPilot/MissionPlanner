using System;
using System.Collections.Generic;
using System.Text;
using KMLib.Abstract;

namespace KMLib
{
    public class Icon : ALink
    {
    }

    /*
  <Icon id="ID">
  <!-- specific to Icon -->
  <href>...</href>                      <!-- anyURI -->
  <refreshMode>onChange</refreshMode>   
    <!-- kml:refreshModeEnum: onChange, OnInterval, or OnExpire -->   
  <refreshInterval>4</refreshInterval>  <!-- float -->
  <viewRefreshMode>never</viewRefreshMode> 
    <!-- kml:viewRefreshModeEnum: never, onStop, onRequest, onRegion -->
  <viewRefreshTime>4</viewRefreshTime>  <!-- float -->
  <viewBoundScale>1</viewBoundScale>    <!-- float -->
  <viewFormat>...</viewFormat>          <!-- string -->
  <httpQuery>...</httpQuery>            <!-- string -->
  </Icon>
     */
}
