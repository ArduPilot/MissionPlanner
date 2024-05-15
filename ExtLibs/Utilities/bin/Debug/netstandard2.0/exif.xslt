<?xml version="1.0" encoding="ISO-8859-1"?>
<!--
	XSLT writen by Eric
-->
<!-- Edited with XML Spy v2006 (http://www.altova.com) -->
<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
<xsl:template match="file/directory">
  <html>
  <body>
    <style> td {font-size:13px;}</style>
    <h2 style="font-size:15px;">
      <xsl:value-of select="@name" />
    </h2>
    <table border="0" width="300">
     <!-- <tr bgcolor="#9acd32">
        <th>Label</th>
        <th>Value</th>
      </tr> -->
      <xsl:for-each select="tag">
      <tr>
        <td><xsl:value-of select="tagLabel" /></td>
        <td><xsl:value-of select="tagDescription" /></td>
      </tr>
      </xsl:for-each>
    </table>
  </body>
  </html>
</xsl:template>
</xsl:stylesheet>