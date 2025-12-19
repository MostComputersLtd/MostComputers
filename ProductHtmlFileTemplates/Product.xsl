<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" omit-xml-declaration="yes" indent="no"/>

	<xsl:template match="data">
		<xsl:for-each select="productlist/product">
			<body>
				<p>
					<span style="font-size: medium;">
						<b>
							<xsl:value-of select="name"/>
						</b>
						<br />
						Product ID: <xsl:value-of select="@id"/>
					</span>
				</p>
			
				<xsl:for-each select="properties/property">
					<p></p>
					<p>
						&#8226; <b><xsl:value-of select="@name"/> </b>: <b><xsl:value-of select="text()"/> </b>
					</p>
				</xsl:for-each>

			</body>
		</xsl:for-each>
	</xsl:template>
	<xsl:strip-space elements="*"/>
</xsl:stylesheet>