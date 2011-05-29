<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:import href="./xpaths_all.xsl" />
  <xsl:variable name="debug" select="true()" />
  <xsl:output method="xml" omit-xml-declaration="yes" />
  <!--  match the root of the document -->
  <xsl:template match="/">
    <html>
      <head>
        <title>Affiliate</title>
        <style>

          BODY {
          PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px; BACKGROUND-COLOR: #FFFFFF;
          font-family: Arial, Helvetica, sans-serif;
          font-size:11px;
          color:#000000;
          }

          INPUT {
          font-family: Arial, Helvetica, sans-serif;
          font-size:11px;
          }

          SELECT {
          font-family: Arial, Helvetica, sans-serif;
          font-size:11px;
          }

          A:link {
          font-family: Arial, Helvetica, sans-serif;
          font-size:11px;
          color:#20436B;
          }

          A:visited {
          font-family: Arial, Helvetica, sans-serif;
          font-size:11px;
          color:#20436B;
          }

          A:active {
          font-family: Arial, Helvetica, sans-serif;
          font-size:11px;
          color:#20436B;
          }

          A:hover {
          font-family: Arial, Helvetica, sans-serif;
          font-size:11px;
          color:#0099FF;
          }

   	  font.val { color: #4ebaff; }
	  font.val1 { color: #b2d40a; }
	  font.val2 { color: #4ebaff; font-size:36px; }
	  font.strategies { color: #66cc9a; }
	  font.header { color: #ff5d38; }

        </style>
      </head>
      <body>
		<table width="92%" align="center">
			<tr>
				<td align="right"><img src="http://www.abel-perez.com/alx/logo.gif" /></td>
			</tr>
		</table>
		<hr />
		<br />
		<table width="92%" align="center">
			<tr>
				<td><h1><font class="header">Summary</font></h1></td>
			</tr>
			<tr>
				<td><h1>Vertical: <font class="val"><xsl:value-of select="$ALX_VERTICAL" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Status: <font class="val"><xsl:value-of select="$ALX_LEAD_STATUS" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Publisher: <font class="val"><xsl:value-of select="$ALX_LEAD_AID" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Campaign: <font class="val"><xsl:value-of select="$ALX_LEAD_CID" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Email: <font class="val"><xsl:value-of select="$ALX_LEAD_EMAIL" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Acquired: <font class="val"><xsl:value-of select="$ALX_LEAD_CREATED" /></font></h1></td>
			</tr>
		</table>
		<hr />
		<br />
		<br />
		<table width="92%" align="center">
			<tr>
				<td><h1><font class="header">Lead Details</font></h1></td>
			</tr>
			<tr>
				<td><h1>First Name: <font class="val1"><xsl:value-of select="$ALX_LEAD_FIRSTNAME" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Last Name: <font class="val1"><xsl:value-of select="$ALX_LEAD_LASTNAME" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Phone: <font class="val1"><xsl:value-of select="$ALX_LEAD_PHONE" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Secondary Phone: <font class="val1"><xsl:value-of select="$ALX_LEAD_SECONDARYPHONE" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Street: <font class="val1"><xsl:value-of select="$ALX_LEAD_STREET" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>City: <font class="val1"><xsl:value-of select="$ALX_LEAD_CITY" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Zip: <font class="val1"><xsl:value-of select="$ALX_LEAD_ZIP" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>State: <font class="val1"><xsl:value-of select="$ALX_LEAD_STATE" /></font></h1></td>
			</tr>
			<tr>
				<td><h1>Created: <font class="val1"><xsl:value-of select="$ALX_LEAD_CREATED" /></font></h1></td>
			</tr>
		</table>
		<hr />
		<br />
		<br />
		<table width="92%" align="center">
			<tr>
				<td><h1><font class="header">Exchange Engine</font></h1></td>
			</tr>
			<xsl:for-each select="$ALX_RUNTIME_STRATEGIES">
			<tr>
				<td><h1>Actor/Time (ms): <font class="strategies"><xsl:value-of select="./actor" /></font> : <font class="strategies"><xsl:value-of select="./elapsed" /></font></h1></td>
			</tr>
			</xsl:for-each>
		</table>
		<hr />
		<br />
		<br />
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>